using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

[System.Serializable]
public class TerrainChunkGenerator : MonoBehaviour
{
    public GameObject nodePrefab;
    
    // Mesh needs to be regenerated if these fields change.
    [Range(1, 10)] public int width;
    private int _width;
    
    [Range(1, 10)] public int height;
    private int _height;
    
    [Range(1, 10)] public int depth;
    private int _depth;
    
    public uint terrainSeed;
    private uint _terrainSeed;
    
    [Range(-1, 1)] public float surfaceLevel;
    private float _surfaceLevel;
    
    [Range(0, 15)] public float noiseDensity;
    private float _noiseDensity;
    
    [Range(1, 8)] public int numNoiseOctaves;
    private float _numNoiseOctaves;
    
    [Range(0, 1)] public float persistence;
    private float _persistence;
    
    [Range(1, 2)] public float lacunarity;
    private float _lacunarity;
    
    public bool terrainSmoothing;
    private bool _terrainSmoothing;
    
    // Mesh does not need to be regenerated if these fields change.
    public Color terrainColor;
    private Color _terrainColor;
    
    // Debug draw portion
    public bool drawBoundingBox;
    
    public enum NodePosition { All, InsideTerrain, None } // Position at which to show terrain nodes.
    public NodePosition nodePosition;
    private NodePosition _nodePosition;

    private GameObject[] _nodes;
    private GameObject _nodeParent;

    [HideInInspector] public Vector3[] boundingBoxCorners;
    private Vector3[] _marchingCubeCorners;

    private NativeArray<float> _chunkHeightMap;
    private NativeArray<int> _chunkNumElements;
    private NativeArray<float3> _chunkMeshVertices;

    private GameObject _chunk;
    private MeshFilter _chunkMeshFilter;
    private MeshRenderer _chunkMeshRenderer;

    private void Start()
    {
        int totalNumNodes = width * height * depth;
        
        _chunkHeightMap = new NativeArray<float>(totalNumNodes, Allocator.Persistent);
        _chunkNumElements = new NativeArray<int>(1, Allocator.Persistent);
        _chunkMeshVertices = new NativeArray<float3>(totalNumNodes * 15, Allocator.Persistent);

        // Configure chunk game object.
        _chunk = new GameObject();
        _chunk.transform.parent = transform;
        _chunk.name = "TerrainMesh";
        _chunk.isStatic = true;
        
        _chunkMeshFilter = _chunk.AddComponent<MeshFilter>();
        _chunkMeshRenderer = _chunk.AddComponent<MeshRenderer>();
        
        Material chunkMaterial = new Material(Shader.Find("Diffuse"));
        chunkMaterial.color = terrainColor;
        _chunkMeshRenderer.material = chunkMaterial;
        
        // Debug
        // Configure parent to all debug nodes.
        _nodeParent = new GameObject();
        _nodeParent.transform.parent = _chunk.transform;
        _nodeParent.name = "NodeParent";
        _nodeParent.isStatic = true;
        
        _nodes = new GameObject[totalNumNodes];
        
        // Configure bounding box and unit cube.
        boundingBoxCorners = new Vector3[8];
        _marchingCubeCorners = new Vector3[8];
        ConstructMarchingCube();
    }

    private void Update()
    {
        // Update values that require regenerating the mesh.
        if (ImportantValuesChanged())
        {
            UpdateImportantValues();
            
            // Reconstruct terrain mesh.
            GenerateChunkHeightMap();
            GenerateChunkMesh();
            ConstructChunkMesh();
            GenerateNodes();
        }

        // Update values that don't require regenerating the mesh.
        if (UnimportantValuesChanged())
        {
            UpdateUnimportantValues();
        }
    }

    private void OnDestroy()
    {
        _chunkHeightMap.Dispose();
        _chunkNumElements.Dispose();
        _chunkMeshVertices.Dispose();
    }

    private void GenerateChunkHeightMap()
    {
        ChunkHeightGenerationJob heightGenerationJob = new ChunkHeightGenerationJob
        {
            numNodesPerAxis = new int3(width, height, depth),
            numNoiseOctaves = numNoiseOctaves,
            noiseScale = noiseDensity,
            persistence = persistence,
            lacunarity = lacunarity,
            terrainHeightMap = _chunkHeightMap,
            seededGenerator = new Unity.Mathematics.Random(terrainSeed)
        };

        heightGenerationJob.Schedule().Complete();
    }

    private void GenerateChunkMesh()
    {
        ChunkMeshGenerationJob meshGenerationJob = new ChunkMeshGenerationJob
        {
            cornerTable = MarchingCubes.cornerTable,
            edgeTable = MarchingCubes.edgeTable,
            triangleTable = MarchingCubes.triangleTable,
            vertices = _chunkMeshVertices,
            numElements = _chunkNumElements,
            terrainHeightMap = _chunkHeightMap,
            terrainSurfaceLevel = surfaceLevel,
            terrainSmoothing = terrainSmoothing,
            axisDimensionsInCubes = new int3(width - 1, height - 1, depth - 1),
            numNodesPerAxis = new int3(width, height, depth),
            numCubes = (width - 1) * (height - 1) * (depth - 1),
        };

        meshGenerationJob.Schedule().Complete();
    }

    private void GenerateNodes()
    {
        // Clear all game object instances first.
        for (int i = 0; i < _nodes.Length; ++i)
        {
            Destroy(_nodes[i]);
        }
        
        _nodes = new GameObject[width * height * depth];
         for (int x = 0; x < width; ++x)
         {
             for (int z = 0; z < depth; ++z)
             {
                 for (int y = 0; y < height; ++y)
                 {
                     int index = x + z * width + y * width * depth;
                     float noiseValue = _chunkHeightMap[index];
                     GameObject node = Instantiate(nodePrefab, new Vector3(x, y, z), Quaternion.identity, _nodeParent.transform);
                     MeshRenderer nodeMeshRenderer = node.GetComponent<MeshRenderer>();

                     // Update node color.
                     if (noiseValue > surfaceLevel)
                     {
                         nodeMeshRenderer.material.color = new Color(0.9f, 0.9f, 0.9f);
                     }
                     else
                     {
                         nodeMeshRenderer.material.color = Color.black;
                     }

                     _nodes[index] = node;
                 }
             }
         }

         UpdateNodes();
    }

    private void UpdateNodes()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < depth; ++z)
            {
                for (int y = 0; y < height; ++y)
                {
                    int index = x + z * width + y * width * depth;
                    float noiseValue = _chunkHeightMap[index];
                    GameObject node = _nodes[index];

                    switch (nodePosition)
                    {
                        case NodePosition.All:
                            node.SetActive(true);
                            break;
                        case NodePosition.InsideTerrain:
                            if (noiseValue > surfaceLevel)
                            {
                                node.SetActive(false);
                            }
                            else
                            {
                                node.SetActive(true);
                            }

                            break;
                        case NodePosition.None:
                            node.SetActive(false);
                            break;
                    }
                }
            }
        }
    }

    private void ConstructChunkMesh()
    {
        Mesh mesh = new Mesh();

        // Allocate mesh data.
        int size = _chunkNumElements[0];
        Vector3[] meshVertices = new Vector3[size];
        int[] triangles = new int[size];

        // Transfer data over.
        for (int i = 0; i < size; ++i)
        {
            meshVertices[i] = _chunkMeshVertices[i];
            triangles[i] = i;
        }

        // Create mesh.
        mesh.vertices = meshVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _chunkMeshFilter.mesh = mesh;
    }

    private bool ImportantValuesChanged()
    {
        // Return true if any of the public facing values differ from the previously tracked value.
        return _width != width ||
               _height != height ||
               _depth != depth ||
               _terrainSeed != terrainSeed ||
               _surfaceLevel != surfaceLevel ||
               _noiseDensity != noiseDensity ||
               _numNoiseOctaves != numNoiseOctaves ||
               _persistence != persistence ||
               _lacunarity != lacunarity ||
               _terrainSmoothing != terrainSmoothing;

    }

    private void UpdateImportantValues()
    {
        // Update all the previously tracked values to be the public facing ones.
        _width = width;
        _height = height;
        _depth = depth;

        _chunkHeightMap.Dispose();
        _chunkHeightMap = new NativeArray<float>(width * height * depth, Allocator.Persistent);
        _chunkMeshVertices.Dispose();
        _chunkMeshVertices = new NativeArray<float3>(width * height * depth * 15, Allocator.Persistent);
        
        _terrainSeed = terrainSeed;
        _surfaceLevel = surfaceLevel;
        _noiseDensity = noiseDensity;
        _numNoiseOctaves = numNoiseOctaves;
        _persistence = persistence;
        _lacunarity = lacunarity;
        _terrainSmoothing = terrainSmoothing;

        UpdateBoundingBoxDimensions();
    }

    private bool UnimportantValuesChanged()
    {
        return _nodePosition != nodePosition ||
               _terrainColor != terrainColor;
    }

    private void UpdateUnimportantValues()
    {
        _nodePosition = nodePosition;
        _terrainColor = terrainColor;
        _chunkMeshRenderer.material.color = terrainColor;
        UpdateNodes();
    }

    private void ConstructMarchingCube()
    {
        _marchingCubeCorners[0] = new Vector3(0, 0, 0);
        _marchingCubeCorners[1] = new Vector3(1, 0, 0);
        _marchingCubeCorners[2] = new Vector3(0, 0, 1);
        _marchingCubeCorners[3] = new Vector3(1, 0, 1);
        _marchingCubeCorners[4] = new Vector3(0, 1, 0);
        _marchingCubeCorners[5] = new Vector3(1, 1, 0);
        _marchingCubeCorners[6] = new Vector3(0, 1, 1);
        _marchingCubeCorners[7] = new Vector3(1, 1, 1);
    }
    
    private void UpdateBoundingBoxDimensions()
    {
        Vector3 chunkPosition = _chunk.transform.position;
        boundingBoxCorners[0] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z - 0.5f);
        boundingBoxCorners[1] = new Vector3(chunkPosition.x + width - 1.0f, chunkPosition.y - 1.0f, chunkPosition.z - 0.5f);
        boundingBoxCorners[2] = new Vector3(chunkPosition.x + width - 1.0f, chunkPosition.y - 1.0f, chunkPosition.z + depth - 1.0f);
        boundingBoxCorners[3] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z + depth - 1.0f);

        boundingBoxCorners[4] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + height - 1.0f, chunkPosition.z - 0.5f);
        boundingBoxCorners[5] = new Vector3(chunkPosition.x + width - 1.0f, chunkPosition.y + height - 1.0f, chunkPosition.z - 0.5f);
        boundingBoxCorners[6] = new Vector3(chunkPosition.x + width - 1.0f, chunkPosition.y + height - 1.0f, chunkPosition.z + depth - 1.0f);
        boundingBoxCorners[7] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + height - 1.0f, chunkPosition.z + depth - 1.0f);
    }

    private void DrawBoundingBox()
    {
        
    }
}    
