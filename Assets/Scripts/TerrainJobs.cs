using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct ChunkHeightGenerationJob : IJob
{
    [ReadOnly] public int3 numNodesPerAxis;
    [ReadOnly] public int numNoiseOctaves;
    [ReadOnly] public float noiseScale;
    [ReadOnly] public float persistence;
    [ReadOnly] public float lacunarity;
    [ReadOnly] public float3 offset;
    [ReadOnly] public Random seededGenerator;
    
    [WriteOnly] public NativeArray<float> terrainHeightMap;
    
    public void Execute()
    {
        // Generate terrain offsets.
        NativeArray<float3> octaveOffsets = new NativeArray<float3>(numNoiseOctaves, Allocator.Temp);
        
        for (int i = 0; i < numNoiseOctaves; ++i)
        {
            float offsetX = seededGenerator.NextFloat(-100000, 100000) + offset.x;
            float offsetY = seededGenerator.NextFloat(-100000, 100000) + offset.y;
            float offsetZ = seededGenerator.NextFloat(-100000, 100000) + offset.z;
            
            octaveOffsets[i] = new float3(offsetX, offsetY, offsetZ);
        }
        
        // Offset noise scale for it not to be a whole number.
        if (noiseScale <= 0)
        {
            noiseScale = 0.0001f;
        }
        
        // Generate perlin noise values in the map.
        for (int y = 0; y < numNodesPerAxis.y; ++y)
        {
            for (int x = 0; x < numNodesPerAxis.x; ++x)
            {
                for (int z = 0; z < numNodesPerAxis.z; ++z)
                {
                    int index = ExpandIndex(x, y, z);
                    float noiseHeight = 0.0f;

                    if (x == 0 || x == numNodesPerAxis.x - 1 || y == 0 || y == numNodesPerAxis.y - 1 || z == 0 ||
                        z == numNodesPerAxis.z - 1)
                    {
                        noiseHeight = 1.0f;
                    }
                    else
                    {
                        float amplitude = 1.0f;
                        float frequency = 1.0f;

                        for (int i = 0; i < numNoiseOctaves; ++i)
                        {
                            float sampleX = (x + octaveOffsets[i].x) / noiseScale * frequency;
                            float sampleY = (y + octaveOffsets[i].y) / noiseScale * frequency;
                            float sampleZ = (z + octaveOffsets[i].z) / noiseScale * frequency;

                            // Get perlin values from -1 to 1
                            float current = noise.cnoise(new float3(sampleX, sampleY, sampleZ));// - 0.5f) * 2.0f;
                            noiseHeight += current * amplitude;

                            amplitude *= persistence; // Persistence should be between 0 and 1 - amplitude decreases with each octave.
                            frequency *= lacunarity;  // Lacunarity should be greater than 1 - frequency increases with each octave.
                        }
                    }
                    
                    terrainHeightMap[index] = noiseHeight;
                }
            }
        }
        
        octaveOffsets.Dispose();
    }

    private int ExpandIndex(int x, int y, int z)
    {
        return x + (numNodesPerAxis.x) * z + (numNodesPerAxis.x * numNodesPerAxis.z) * y;
    }
}

[BurstCompile]
public struct ChunkMeshGenerationJob : IJob
{
    // Marching cube configuration
    [ReadOnly] public NativeArray<int> cornerTable;
    [ReadOnly] public NativeArray<int> edgeTable;
    [ReadOnly] public NativeArray<int> triangleTable;
    [ReadOnly] public NativeArray<float> terrainHeightMap;
    [ReadOnly] public float terrainSurfaceLevel;
    [ReadOnly] public int3 axisDimensionsInCubes;
    [ReadOnly] public int3 numNodesPerAxis;
    [ReadOnly] public int numCubes;

    [WriteOnly] public NativeArray<float3> vertices;
    [WriteOnly] public NativeArray<int> numElements;

    public void Execute()
    {
        NativeArray<float> cubeCornerValues = new NativeArray<float>(8, Allocator.Temp);

        int vertexIndex = 0;
        int cubeIndex = 0;
        
        for (int y = 0; y < axisDimensionsInCubes.y; ++y)
        {
            for (int x = 0; x < axisDimensionsInCubes.x; ++x)
            {
                for (int z = 0; z < axisDimensionsInCubes.z; ++z)
                {
                    if (cubeIndex > numCubes)
                    {
                        break;
                    }
                    
                    // Construct cube with noise values.
                    int3 normalizedCubePosition = new int3(x, y, z);
                    cubeCornerValues[0] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x, normalizedCubePosition.y, normalizedCubePosition.z)];
                    cubeCornerValues[1] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x + 1, normalizedCubePosition.y, normalizedCubePosition.z)];
                    cubeCornerValues[2] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x + 1, normalizedCubePosition.y + 1, normalizedCubePosition.z)];
                    cubeCornerValues[3] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x, normalizedCubePosition.y + 1, normalizedCubePosition.z)];
                    cubeCornerValues[4] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x, normalizedCubePosition.y, normalizedCubePosition.z + 1)];
                    cubeCornerValues[5] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x + 1, normalizedCubePosition.y, normalizedCubePosition.z + 1)];
                    cubeCornerValues[6] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x + 1, normalizedCubePosition.y + 1, normalizedCubePosition.z + 1)];
                    cubeCornerValues[7] = terrainHeightMap[IndexFromCoordinate(normalizedCubePosition.x, normalizedCubePosition.y + 1, normalizedCubePosition.z + 1)];

                    // March cube.
                    int configuration = GetCubeConfiguration(cubeCornerValues);

                    if (configuration == 0 || configuration == 255)
                    {
                        ++cubeIndex;
                        continue;
                    }

                    int edgeIndex = 0;
                    
                    // A configuration has maximum 5 triangles in it.
                    for (int i = 0; i < 5; ++i) {
                        // A configuration element (triangle) consists of 3 points.
                        for (int j = 0; j < 3; ++j) {
                            int triangleIndex = triangleTable[configuration * 16 + edgeIndex];

                            // Reached the end of this configuration.
                            if (triangleIndex == -1)
                            {
                                break;
                            }

                            int edgeVertex1Index = triangleIndex * 2 + 0;
                            int edgeVertex2Index = triangleIndex * 2 + 1;

                            int corner1Index = edgeTable[edgeVertex1Index] * 3;
                            int corner2Index = edgeTable[edgeVertex2Index] * 3;
                            
                            int3 corner1 = new int3(cornerTable[corner1Index + 0], cornerTable[corner1Index + 1], cornerTable[corner1Index + 2]);
                            int3 corner2 = new int3(cornerTable[corner2Index + 0], cornerTable[corner2Index + 1], cornerTable[corner2Index + 2]);
                            
                            float3 edgeVertex1 = normalizedCubePosition + corner1;
                            float3 edgeVertex2 = normalizedCubePosition + corner2;

                            // Calculate vertex position.
                            vertices[vertexIndex++] = (edgeVertex1 + edgeVertex2) / 2.0f;
                            ++edgeIndex;
                        }
                    }

                    ++cubeIndex;
                }
            }
        }
        
        numElements[0] = vertexIndex; // numElements only has 1 element.
        cubeCornerValues.Dispose();
    }

    float3 Interpolate(float3 vertex1, float vertex1Value, float3 vertex2, float vertex2Value)
    {
        float t = (terrainSurfaceLevel - vertex1Value) / (vertex2Value - vertex1Value);
        float3 vert = vertex1 + t * (vertex2 - vertex1);
        return vert;
    }
    
    int IndexFromCoordinate(int x, int y, int z)
    {
        return x + z * numNodesPerAxis.x + y * numNodesPerAxis.x * numNodesPerAxis.z;
    }

    int GetCubeConfiguration(NativeArray<float> cubeCornerValues)
    {
        int configuration = 0;

        for (int i = 0; i < 8; ++i) {
            if (cubeCornerValues[i] > terrainSurfaceLevel) {
                configuration |= 1 << i;
            }
        }
        
        return configuration;
    }
}