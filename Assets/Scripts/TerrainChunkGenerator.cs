using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

[Serializable]
public class TerrainChunkGenerator : MonoBehaviour
{
    public GameObject nodePrefab;
    
    // Mesh needs to be regenerated if these fields change.
    [Range(3, 10)] public int width;
    private int _width;
    
    [Range(3, 10)] public int height;
    private int _height;
    
    [Range(3, 10)] public int depth;
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
    
    // Mesh does not need to be regenerated if these fields change.
    public Color terrainColor;
    private Color _terrainColor;
    
    // Debug draw portion
    public bool drawTerrainNodes;
    private bool _drawNodes;
    public bool drawBoundingBox;
    
    // Cube marching animation
    public bool animateCubeMarching;
    public bool drawMarchingCube;
    [Range(0, 1)] public float cubeMarchInterval;
    private float _cubeMarchInterval;
    [HideInInspector] public bool generatingTerrain;
    [HideInInspector] public bool pauseGeneration;
    private int _currentMarchingCubeCounter;
    
    private GameObject[] _nodes;
    private GameObject _nodeParent;

    [HideInInspector] public Vector3[] boundingBoxCorners;
    [HideInInspector] public Vector3[] marchingCubeCorners;
    private Vector3[] _marchingCubeCorners;

    private int _totalNumNodes;
    private int _totalNumCubes;
    
    private NativeArray<float> _chunkHeightMap;
    private NativeArray<int> _chunkNumElements;
    private NativeArray<float3> _chunkMeshVertices;

    private GameObject _chunk;
    private MeshFilter _chunkMeshFilter;
    private MeshRenderer _chunkMeshRenderer;

    private GameObject _cameraFocus;

    private void Start()
    {
        _totalNumNodes = (width + 2) * (height + 2) * (depth + 2);
        _totalNumCubes = (width + 1) * (height + 1) * (depth + 1);
        
        _chunkHeightMap = new NativeArray<float>(_totalNumNodes, Allocator.Persistent);
        _chunkNumElements = new NativeArray<int>(1, Allocator.Persistent);
        _chunkMeshVertices = new NativeArray<float3>(_totalNumCubes * 15, Allocator.Persistent);

        // Configure chunk game object.
        _chunk = new GameObject();
        _chunk.transform.parent = transform;
        _chunk.name = "TerrainMesh";
        
        _chunkMeshFilter = _chunk.AddComponent<MeshFilter>();
        _chunkMeshRenderer = _chunk.AddComponent<MeshRenderer>();
        
        Material chunkMaterial = new Material(Shader.Find("Diffuse"));
        chunkMaterial.color = terrainColor;
        _chunkMeshRenderer.material = chunkMaterial;
        
        // Debug
        // Configure parent to all debug nodes.
        _nodeParent = new GameObject();
        _nodeParent.transform.parent = transform;
        _nodeParent.name = "NodeParent";
        
        _nodes = new GameObject[_totalNumNodes];
        _drawNodes = drawTerrainNodes;
        
        _cameraFocus = GameObject.Find("CameraFocus");
        _cameraFocus.transform.position = Vector3.zero;
        _cameraFocus.isStatic = true;
        
        // Configure bounding box and unit cube.
        boundingBoxCorners = new Vector3[8];
        marchingCubeCorners = new Vector3[8];
        
        // Marching cube animation
        _currentMarchingCubeCounter = 0;
        _cubeMarchInterval = cubeMarchInterval;
        _marchingCubeCorners = new Vector3[8];
        ConstructMarchingCube();
        UpdateMarchingCubeCorners(new Vector3Int(0, 0, 0));
        ToggleMarchingCubesPaused(true);
    }

    private void Update()
    {
        // Update values that require regenerating the mesh.
        if (ImportantValuesChanged())
        {
            UpdateImportantValues();
            
            // Reconstruct terrain mesh.
            GenerateChunkHeightMap();
            GenerateNodes();
        }

        // Update values that don't require regenerating the mesh.
        if (UnimportantValuesChanged())
        {
            UpdateUnimportantValues();
        }

        if (animateCubeMarching)
        {
            MarchCube();
        }
        else
        {
            GenerateChunkMesh();
            ConstructChunkMesh();
        }
    }

    private void OnDestroy()
    {
        if (_chunkHeightMap.IsCreated)
        {
            _chunkHeightMap.Dispose();
        }

        if (_chunkNumElements.IsCreated)
        {
            _chunkNumElements.Dispose();
        }

        if (_chunkMeshVertices.IsCreated)
        {
            _chunkMeshVertices.Dispose();
        }
    }

    private void GenerateChunkHeightMap()
    {
        ChunkHeightGenerationJob heightGenerationJob = new ChunkHeightGenerationJob
        {
            numNodesPerAxis = new int3(width + 2, height + 2, depth + 2),
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
        ChunkMeshGenerationJob meshGenerationJob = new ChunkMeshGenerationJob()
        {
            cornerTable = MarchingCubes.cornerTable,
            edgeTable = MarchingCubes.edgeTable,
            triangleTable = MarchingCubes.triangleTable,
            vertices = _chunkMeshVertices,
            numElements = _chunkNumElements,
            terrainHeightMap = _chunkHeightMap,
            terrainSurfaceLevel = surfaceLevel,
            axisDimensionsInCubes = new int3(width + 1, height + 1, depth + 1),
            numNodesPerAxis = new int3(width + 2, height + 2, depth + 2),
        };
        
        if (animateCubeMarching)
        {
            meshGenerationJob.numCubes = _currentMarchingCubeCounter;
        }
        else
        {
            meshGenerationJob.numCubes = _totalNumCubes;
        }
        
        meshGenerationJob.Schedule().Complete();
    }

    private void GenerateNodes()
    {
        // Clear all game object instances first.
        for (int i = 0; i < _nodes.Length; ++i)
        {
            Destroy(_nodes[i]);
        }
        
        _nodes = new GameObject[_totalNumNodes];
        
        for (int y = 0; y < height + 2; ++y)
        {
            for (int x = 0; x < width + 2; ++x)
            {
                for (int z = 0; z < depth + 2; ++z)
                {
                    int index = x + z * (width + 2) + y * (width + 2) * (depth + 2);
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
        for (int y = 0; y < height + 2; ++y)
        {
            for (int x = 0; x < width + 2; ++x)
            {
                for (int z = 0; z < depth + 2; ++z)
                {
                    int index = x + z * (width + 2) + y * (width + 2) * (depth + 2);
                    _nodes[index].SetActive(drawTerrainNodes);
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
               _lacunarity != lacunarity;
    }

    private void UpdateImportantValues()
    {
        // Update all the previously tracked values to be the public facing ones.
        _width = width;
        _height = height;
        _depth = depth;

        _totalNumNodes = (width + 2) * (height + 2) * (depth + 2);
        _totalNumCubes = (width + 1) * (height + 1) * (depth + 1);

        _cameraFocus.transform.position = new Vector3((_width + 1) / 2.0f, (_height + 1) / 2.0f, (_depth + 1) / 2.0f);
        
        _chunkHeightMap.Dispose();
        _chunkHeightMap = new NativeArray<float>(_totalNumNodes, Allocator.Persistent);
        _chunkMeshVertices.Dispose();
        _chunkMeshVertices = new NativeArray<float3>(_totalNumNodes * 15, Allocator.Persistent);
        
        _terrainSeed = terrainSeed;
        _surfaceLevel = surfaceLevel;
        _noiseDensity = noiseDensity;
        _numNoiseOctaves = numNoiseOctaves;
        _persistence = persistence;
        _lacunarity = lacunarity;

        UpdateBoundingBoxDimensions();
        ResetMarchingCubes();
    }

    private bool UnimportantValuesChanged()
    {
        return _drawNodes != drawTerrainNodes ||
               _terrainColor != terrainColor;
    }

    private void UpdateUnimportantValues()
    {
        _drawNodes = drawTerrainNodes;
        _terrainColor = terrainColor;
        _chunkMeshRenderer.material.color = terrainColor;
        UpdateNodes();
    }

    private void MarchCube()
    {
        // Reset the cube marching if it has finished marching the entire chunk.
        if (_currentMarchingCubeCounter > _totalNumCubes - 1)
        {
            // Reset the cube position back to 0.
            _currentMarchingCubeCounter = 0;
            UpdateMarchingCubeCorners(PositionFromIndex(_currentMarchingCubeCounter));
            
            pauseGeneration = true;
        }
        
        if (!pauseGeneration)
        {
            _cubeMarchInterval -= Time.deltaTime;
            if (_cubeMarchInterval < 0.0f)
            {
                // Reset interval.
                _cubeMarchInterval = cubeMarchInterval;
                UpdateMarchingCubeCorners(PositionFromIndex(++_currentMarchingCubeCounter));
                
                // Generate mesh.
                GenerateChunkMesh();
                ConstructChunkMesh();
            }
        }
    }

    private Vector3Int PositionFromIndex(int index)
    {
        return new Vector3Int((index / ((_depth + 2) - 1)) % ((_width + 2) - 1), index / (((_width + 2) - 1) * ((_depth + 2) - 1)) , index % ((_depth + 2) - 1));
    }

    private void UpdateMarchingCubeCorners(Vector3Int normalizedCubePosition)
    {
        for (int i = 0; i < 8; ++i)
        {
            marchingCubeCorners[i] = _marchingCubeCorners[i] + normalizedCubePosition;
        }
    }

    public void StartMarchingCubes()
    {
        if (!generatingTerrain)
        {
            pauseGeneration = false;
            generatingTerrain = true;
            GenerateChunkMesh();
            ConstructChunkMesh();
        }
    }

    public void ToggleMarchingCubesPaused(bool pause)
    {
        pauseGeneration = pause;
    }

    public void StepMarchingCubes(bool forward)
    {
        if (pauseGeneration)
        {
            if (forward)
            {
                ++_currentMarchingCubeCounter;
            }
            else
            {
                --_currentMarchingCubeCounter;
                _currentMarchingCubeCounter = Math.Max(0, _currentMarchingCubeCounter);
            }
            
            UpdateMarchingCubeCorners(PositionFromIndex(_currentMarchingCubeCounter));
            GenerateChunkMesh();
            ConstructChunkMesh();
        }
    }

    public void ResetMarchingCubes()
    {
        // Reset generation values.
        _chunkMeshFilter.mesh.Clear();
        generatingTerrain = false;
        pauseGeneration = true;
        _currentMarchingCubeCounter = 0;
        
        // Reset debug cube to the start.
        UpdateMarchingCubeCorners(PositionFromIndex(_currentMarchingCubeCounter));
    }
    
    private void UpdateBoundingBoxDimensions()
    {
        Vector3 chunkPosition = _chunk.transform.position;
        boundingBoxCorners[0] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z - 0.5f);
        boundingBoxCorners[1] = new Vector3(chunkPosition.x + width + 2 - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z - 0.5f);
        boundingBoxCorners[2] = new Vector3(chunkPosition.x + width + 2 - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z + depth + 2 - 0.5f);
        boundingBoxCorners[3] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z + depth + 2 - 0.5f);
        
        boundingBoxCorners[4] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + height + 2 - 0.5f, chunkPosition.z - 0.5f);
        boundingBoxCorners[5] = new Vector3(chunkPosition.x + width + 2 - 0.5f, chunkPosition.y + height + 2 - 0.5f, chunkPosition.z - 0.5f);
        boundingBoxCorners[6] = new Vector3(chunkPosition.x + width + 2 - 0.5f, chunkPosition.y + height + 2 - 0.5f, chunkPosition.z + depth + 2 - 0.5f);
        boundingBoxCorners[7] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + height + 2 - 0.5f, chunkPosition.z + depth + 2 - 0.5f);
    }
    
    private void ConstructMarchingCube()
    {
        _marchingCubeCorners[0] = new Vector3(0, 0, 0);
        _marchingCubeCorners[1] = new Vector3(1, 0, 0);
        _marchingCubeCorners[2] = new Vector3(1, 0, 1);
        _marchingCubeCorners[3] = new Vector3(0, 0, 1);
        _marchingCubeCorners[4] = new Vector3(0, 1, 0);
        _marchingCubeCorners[5] = new Vector3(1, 1, 0);
        _marchingCubeCorners[6] = new Vector3(1, 1, 1);
        _marchingCubeCorners[7] = new Vector3(0, 1, 1);
    }
}    
