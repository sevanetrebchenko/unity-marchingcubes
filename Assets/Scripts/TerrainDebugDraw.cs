// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using Unity.Collections;
// using UnityEngine;
//
// [System.Serializable]
// public class TerrainDebugDraw
// {
//
//     
//     public TerrainDebugDraw(TerrainChunkGenerator terrainChunkGenerator)
//     {
//         _terrainChunkGenerator = terrainChunkGenerator;
//         _nodes = new GameObject[terrainChunkGenerator.width * terrainChunkGenerator.height * terrainChunkGenerator.depth];
//         _boundingBoxCorners = new Vector3[8];
//         _marchingCubeCorners = new Vector3[8];
//     }
//
//     public void UpdateNodeVisibility()
//     {
//         for (int x = 0; x < _terrainChunkGenerator.width; ++x)
//         {
//             for (int z = 0; z < _terrainChunkGenerator.depth; ++z)
//             {
//                 for (int y = 0; y < _terrainChunkGenerator.height; ++y)
//                 {
//                     int index = x + z * _terrainChunkGenerator.width + y * _terrainChunkGenerator.width * _terrainChunkGenerator.depth;
//                     float noiseValue = _terrainChunkGenerator._chunkHeightMap[index];
//                     GameObject node = _nodes[index];
//                     
//                     switch (nodePosition)
//                     {
//                         case NodePosition.InsideTerrain:
//                             if (noiseValue > _terrainChunkGenerator.terrainConfiguration.surfaceLevel)
//                             {
//                                 node.SetActive(false);
//                             }
//                             else
//                             {
//                                 node.SetActive(true);
//                             }
//                             break;
//                         case NodePosition.All:
//                             node.SetActive(true);
//                             break;
//                         case NodePosition.None:
//                             node.SetActive(false);
//                             break;
//                     }
//                 }
//             }
//         }
//     }
//     
//     public void Update()
//     {
//         if (_terrainChunkGenerator.DimensionsChanged())
//         {
//             _nodes = new GameObject[_terrainChunkGenerator.width * _terrainChunkGenerator.height * _terrainChunkGenerator.depth];
//         
//             for (int x = 0; x < _terrainChunkGenerator.width; ++x)
//             {
//                 for (int z = 0; z < _terrainChunkGenerator.depth; ++z)
//                 {
//                     for (int y = 0; y < _terrainChunkGenerator.height; ++y)
//                     {
//                         int index = x + z * _terrainChunkGenerator.width + y * _terrainChunkGenerator.width * _terrainChunkGenerator.depth;
//                         float noiseValue = _terrainChunkGenerator._chunkHeightMap[index];
//                         GameObject node = GameObject.Instantiate(_terrainChunkGenerator.nodePrefab, new Vector3(x, y, z), Quaternion.identity, _terrainChunkGenerator.nodeParent.transform);
//                     
//                         if (noiseValue > _terrainChunkGenerator.terrainConfiguration.surfaceLevel)
//                         {
//                             node.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.9f, 0.9f);
//                         }
//                         else
//                         {
//                             node.GetComponent<MeshRenderer>().material.color = Color.black;
//                         }
//                     
//                         _nodes[index] = node;
//                     }
//                 }
//             }
//         }
//             
//         UpdateNodeVisibility();
//     }
//
//     public void DrawBoundingBox()
//     {
//         GL.Begin(GL.LINES);
//
//         GL.Color(Color.black);
//         
//         // Bottom rectangle
//         GL.Vertex(_boundingBoxCorners[0]);
//         GL.Vertex(_boundingBoxCorners[1]);
//         
//         // GL.Vertex(boundingBoxCorners[1]);
//         // GL.Vertex(boundingBoxCorners[2]);
//         //
//         // GL.Vertex(boundingBoxCorners[2]);
//         // GL.Vertex(boundingBoxCorners[3]);
//         //
//         // GL.Vertex(boundingBoxCorners[3]);
//         // GL.Vertex(boundingBoxCorners[0]);
//         //
//         // // Top rectangle
//         // GL.Vertex(boundingBoxCorners[4]);
//         // GL.Vertex(boundingBoxCorners[5]);
//         //
//         // GL.Vertex(boundingBoxCorners[5]);
//         // GL.Vertex(boundingBoxCorners[6]);
//         //
//         // GL.Vertex(boundingBoxCorners[6]);
//         // GL.Vertex(boundingBoxCorners[7]);
//         //
//         // GL.Vertex(boundingBoxCorners[7]);
//         // GL.Vertex(boundingBoxCorners[4]);
//         //
//         // // Connector sides
//         // GL.Vertex(boundingBoxCorners[0]);
//         // GL.Vertex(boundingBoxCorners[4]);
//         //
//         // GL.Vertex(boundingBoxCorners[2]);
//         // GL.Vertex(boundingBoxCorners[5]);
//         //
//         // GL.Vertex(boundingBoxCorners[2]);
//         // GL.Vertex(boundingBoxCorners[6]);
//         //
//         // GL.Vertex(boundingBoxCorners[3]);
//         // GL.Vertex(boundingBoxCorners[7]);
//         
//         GL.End();
//     }
//
//     private void ConstructBoundingBoxCorners()
//     {
//         Vector3 chunkPosition = _terrainChunkGenerator.transform.position;
//         _boundingBoxCorners[0] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z - 0.5f);
//         _boundingBoxCorners[1] = new Vector3(chunkPosition.x + _terrainChunkGenerator.width + 0.5f, chunkPosition.y - 0.5f, chunkPosition.z - 0.5f);
//         _boundingBoxCorners[2] = new Vector3(chunkPosition.x + _terrainChunkGenerator.width + 0.5f, chunkPosition.y - 0.5f, chunkPosition.z + _terrainChunkGenerator.depth + 0.5f);
//         _boundingBoxCorners[3] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y - 0.5f, chunkPosition.z + _terrainChunkGenerator.depth + 0.5f);
//         
//         _boundingBoxCorners[4] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + _terrainChunkGenerator.height + 0.5f, chunkPosition.z - 0.5f);
//         _boundingBoxCorners[5] = new Vector3(chunkPosition.x + _terrainChunkGenerator.width + 0.5f, chunkPosition.y + _terrainChunkGenerator.height + 0.5f, chunkPosition.z - 0.5f);
//         _boundingBoxCorners[6] = new Vector3(chunkPosition.x + _terrainChunkGenerator.width + 0.5f, chunkPosition.y + _terrainChunkGenerator.height + 0.5f, chunkPosition.z + _terrainChunkGenerator.depth + 0.5f);
//         _boundingBoxCorners[7] = new Vector3(chunkPosition.x - 0.5f, chunkPosition.y + _terrainChunkGenerator.height + 0.5f, chunkPosition.z + _terrainChunkGenerator.depth + 0.5f);
//     }
// }
