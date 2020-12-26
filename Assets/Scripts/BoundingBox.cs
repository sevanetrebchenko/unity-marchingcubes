using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    public Material material;

    private void OnPostRender()
    {
        // Draw bounding box for entire terrain chunk.
        if (terrainChunkGenerator.gameObject.activeSelf)
        {
            if (terrainChunkGenerator.drawBoundingBox)
            {
                DrawCube(terrainChunkGenerator.boundingBoxCorners);
            }

            // Draw cube at the current position.
            if (terrainChunkGenerator.animateCubeMarching && terrainChunkGenerator.drawMarchingCube)
            {
                DrawCube(terrainChunkGenerator.marchingCubeCorners);
            }
        }
    }

    private void DrawCube(Vector3[] cubeCorners)
    {
        GL.Begin(GL.LINES);
        
        GL.Color(material.color);
        material.SetPass(0);

        // Bottom rectangle
        GL.Vertex(cubeCorners[0]);
        GL.Vertex(cubeCorners[1]);

        GL.Vertex(cubeCorners[1]);
        GL.Vertex(cubeCorners[2]);
        
        GL.Vertex(cubeCorners[2]);
        GL.Vertex(cubeCorners[3]);
        
        GL.Vertex(cubeCorners[3]);
        GL.Vertex(cubeCorners[0]);
        
        // Top rectangle
        GL.Vertex(cubeCorners[4]);
        GL.Vertex(cubeCorners[5]);
        
        GL.Vertex(cubeCorners[5]);
        GL.Vertex(cubeCorners[6]);
        
        GL.Vertex(cubeCorners[6]);
        GL.Vertex(cubeCorners[7]);
        
        GL.Vertex(cubeCorners[7]);
        GL.Vertex(cubeCorners[4]);
        
        // Connector sides
        GL.Vertex(cubeCorners[0]);
        GL.Vertex(cubeCorners[4]);
        
        GL.Vertex(cubeCorners[1]);
        GL.Vertex(cubeCorners[5]);
        
        GL.Vertex(cubeCorners[2]);
        GL.Vertex(cubeCorners[6]);
        
        GL.Vertex(cubeCorners[3]);
        GL.Vertex(cubeCorners[7]);

        GL.End();
    }
}
