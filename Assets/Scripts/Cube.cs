using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;

public class Cube : MonoBehaviour
{
    public Color color;
    public float nodeRadius;
    private float _nodeRadius;
    
    public float nodeDistance;
    private float _nodeDistance;
    
    public GameObject nodePrefab;
    private KeyValuePair<Vector3, GameObject>[] _cubeCorners;
    private GameObject _nodeParent;
    
    // Cube marching
    private NativeArray<float> _heightMap;
    private NativeArray<int> _numElements;
    private NativeArray<float3> _meshVertices;
    
    // Cube game object
    private GameObject _cube;
    private MeshFilter _cubeMeshFilter;
    private MeshRenderer _cubeMeshRendeder;
    
    private GameObject _backCube;
    private MeshFilter _backcubeMeshFilter;
    private MeshRenderer _backcubeMeshRendeder;

    private GameObject _cameraFocus;
    
    private void Start()
    {
        _cameraFocus = GameObject.Find("CameraFocus");
        _cameraFocus.transform.position = new Vector3(nodeDistance / 2.0f, nodeDistance / 2.0f, nodeDistance / 2.0f);
        
        _nodeParent = new GameObject();
        _nodeParent.transform.parent = transform;
        _nodeParent.name = "NodeParent";

        // Construct data for cube corners
        _cubeCorners = new KeyValuePair<Vector3, GameObject>[8];
        ConstructCubeCorners();

        _heightMap = new NativeArray<float>(8, Allocator.Persistent);
        for (int i = 0; i < 8; ++i)
        {
            _heightMap[i] = -1.0f;
        }
        _numElements = new NativeArray<int>(1, Allocator.Persistent);
        _meshVertices = new NativeArray<float3>(15, Allocator.Persistent); // Maximum of 15 elements in any one configuration.
        
        // Configure chunk game object.
        _cube = new GameObject();
        _cube.transform.parent = transform;
        _cube.name = "TerrainMesh";
        
        _cubeMeshFilter = _cube.AddComponent<MeshFilter>();
        _cubeMeshRendeder = _cube.AddComponent<MeshRenderer>();

        Material frontMaterial = new Material(Shader.Find("Diffuse"));
        frontMaterial.color = color;
        _cubeMeshRendeder.material = frontMaterial;
        _cubeMeshRendeder.receiveShadows = false;
        
        // Construct back face.
        _backCube = new GameObject();
        _backCube.transform.parent = transform;
        _backCube.name = "TerrainMesh";
        
        _backcubeMeshFilter = _backCube.AddComponent<MeshFilter>();
        _backcubeMeshRendeder = _backCube.AddComponent<MeshRenderer>();

        Material backMaterial = new Material(Shader.Find("Diffuse"));
        backMaterial.color = color;
        _backcubeMeshRendeder.material = backMaterial;
        _backcubeMeshRendeder.receiveShadows = false;
        
        UpdateCubeCorners(true, true, true);
    }

    private void Update()
    {
        HandleInput();

        // Update values that require regenerating the mesh.
        if (ImportantValuesChanged())
        {
            UpdateImportantValues();
            GenerateChunkMesh();
            UpdateCubeCorners(true, false, false);
        }

        // Update values that don't require regenerating the mesh.
        if (UnimportantValuesChanged())
        {
            UpdateUnimportantValues();
            UpdateCubeCorners(false, false, true);
        }
    }

    private void OnDestroy()
    {
        if (_heightMap.IsCreated)
        {
            _heightMap.Dispose();
        }

        if (_numElements.IsCreated)
        {
            _numElements.Dispose();
        }

        if (_meshVertices.IsCreated)
        {
            _meshVertices.Dispose();
        }
    }
    
    private void GenerateChunkMesh()
    {
        ChunkMeshGenerationJob meshGenerationJob = new ChunkMeshGenerationJob()
        {
            cornerTable = MarchingCubes.cornerTable,
            edgeTable = MarchingCubes.edgeTable,
            triangleTable = MarchingCubes.triangleTable,
            vertices = _meshVertices,
            numElements = _numElements,
            terrainHeightMap = _heightMap,
            terrainSurfaceLevel = 0,
            axisDimensionsInCubes = new int3(1, 1, 1),
            numNodesPerAxis = new int3(2, 2, 2),
        };
        
        meshGenerationJob.Schedule().Complete();
        ConstructChunkMesh();
    }
    
    private void ConstructChunkMesh()
    {
        Mesh frontMesh = new Mesh();
        Mesh backMesh = new Mesh();

        // Allocate mesh data.
        int size = _numElements[0];
        Vector3[] meshVertices = new Vector3[size];
        int[] triangles = new int[size];

        // Transfer data over.
        for (int i = 0; i < size; ++i)
        {
            meshVertices[i] = _meshVertices[i] * nodeDistance;
            triangles[i] = i;
        }
        
        // meshVertices = meshVertices.Concat(meshVertices).ToArray();
        // triangles = triangles.Concat(triangles.Reverse().ToArray()).ToArray();

        // Create mesh.
        frontMesh.vertices = meshVertices;
        frontMesh.triangles = triangles;

        backMesh.vertices = meshVertices;
        backMesh.triangles = triangles.Reverse().ToArray();
        
        frontMesh.RecalculateNormals();
        backMesh.RecalculateNormals();

        _cubeMeshFilter.mesh = frontMesh;
        _backcubeMeshFilter.mesh = backMesh;
    }

    private void HandleInput()
    {
        // Set height level of object at clicked position to be -1 (inside surface)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    GameObject hitNode = hit.transform.gameObject;
                    int index = IndexFromCoordinate(hitNode.transform.position);
                    float value = _heightMap[index];
                    if (value > 0.0f)
                    {
                        _heightMap[index] = -1.0f;
                    }
                    else
                    {
                        _heightMap[index] = 1.0f;
                    }
                    
                    GenerateChunkMesh();
                    UpdateCubeCorners(false, true, false);
                }
            }
        }
    }
    
    int IndexFromCoordinate(Vector3 position)
    {
        int index = 0;
        if (position.x != 0)
        {
            index += 1;
        }

        if (position.y != 0)
        {
            index += 4;
        }

        if (position.z != 0)
        {
            index += 2;
        }

        return index;
    }

    private void UpdateCubeCorners(bool updatePosition, bool updateColor, bool updateScale)
    {
        // Don't do anything if values are all false.
        if (!updatePosition && !updateScale && !updateColor)
        {
            return;
        }
        
        foreach (KeyValuePair<Vector3, GameObject> cubeCorner in _cubeCorners)
        {
            if (updatePosition)
            {
                UpdateCubeCornerPosition(cubeCorner);
            }

            if (updateColor)
            {
                UpdateCubeCornerColor(cubeCorner);
            }
            
            if (updateScale)
            {
                UpdateCubeCornerScale(cubeCorner);
            }
        }
    }

    private void UpdateCubeCornerPosition(KeyValuePair<Vector3, GameObject> cubeCorner)
    {
        Vector3 position = cubeCorner.Key;
        GameObject node = cubeCorner.Value;
        
        // Update node position.
        if (position.x != 0)
        {
            position.x = nodeDistance;
        }

        if (position.y != 0)
        {
            position.y = nodeDistance;
        }

        if (position.z != 0)
        {
            position.z = nodeDistance;
        }

        node.transform.position = position;
    }

    private void UpdateCubeCornerColor(KeyValuePair<Vector3, GameObject> cubeCorner)
    {
        Vector3 position = cubeCorner.Key;
        GameObject node = cubeCorner.Value;
        
        // Update node colors.
        int index = IndexFromCoordinate(position);
        float heightValue = _heightMap[index];
        MeshRenderer nodeMeshRenderer = node.GetComponent<MeshRenderer>();

        if (heightValue > 0.0f)
        {
            nodeMeshRenderer.material.color = new Color(0.9f, 0.9f, 0.9f);
        }
        else
        {
            nodeMeshRenderer.material.color = Color.black;
        }
    }

    private void UpdateCubeCornerScale(KeyValuePair<Vector3, GameObject> cubeCorner)
    {
        GameObject node = cubeCorner.Value;
        SphereCollider sphereCollider = node.GetComponent<SphereCollider>();
        sphereCollider.radius = nodeRadius;
        node.transform.localScale = new Vector3(nodeRadius, nodeRadius, nodeRadius);
    }
    
    private void ConstructCubeCorners()
    {
        // 0, 0, 0
        ConstructCubeCorner(new Vector3(0, 0, 0), 0);
        
        // 1, 0, 0
        ConstructCubeCorner(new Vector3(1, 0, 0), 1);
        
        // 1, 0, 1
        ConstructCubeCorner(new Vector3(1, 0, 1), 2);
        
        // 0, 0, 1
        ConstructCubeCorner(new Vector3(0, 0, 1), 3);
        
        // 0, 1, 0
        ConstructCubeCorner(new Vector3(0, 1, 0), 4);
        
        // 1, 1, 0
        ConstructCubeCorner(new Vector3(1, 1, 0), 5);
        
        // 1, 1, 1
        ConstructCubeCorner(new Vector3(1, 1, 1), 6);
        
        // 0, 1, 1
        ConstructCubeCorner(new Vector3(0, 1, 1), 7);
    }

    private void ConstructCubeCorner(Vector3 position, int index)
    {
        GameObject node = Instantiate(nodePrefab, position, Quaternion.identity, _nodeParent.transform);
        SphereCollider sphereCollider = node.AddComponent<SphereCollider>();
        sphereCollider.enabled = true;
        sphereCollider.radius = nodeRadius;
            
        _cubeCorners[index] = new KeyValuePair<Vector3, GameObject>(position, node);
    }

    private bool ImportantValuesChanged()
    {
        return _nodeDistance != nodeDistance;
    }

    private void UpdateImportantValues()
    {
        _nodeDistance = nodeDistance;
        _cameraFocus.transform.position = new Vector3(nodeDistance / 2.0f, nodeDistance / 2.0f, nodeDistance / 2.0f);
    }

    private bool UnimportantValuesChanged()
    {
        return _nodeRadius != nodeRadius;
    }

    private void UpdateUnimportantValues()
    {
        _nodeRadius = nodeRadius;
    }
}
