using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Material _material;

    private void Start()
    {
        _material = new Material(Shader.Find("Diffuse"));
    }

    private void OnPostRender()
    {
        if (terrainChunkGenerator.drawBoundingBox)
        {
            DrawBoundingBox();
        }
    }

    private void DrawBoundingBox()
    {
        GL.Begin(GL.LINES);
        
        _material.SetPass(0);
        GL.Color(Color.gray);

        // Bottom rectangle
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[0]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[1]);

        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[1]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[2]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[2]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[3]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[3]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[0]);
        
        // Top rectangle
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[4]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[5]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[5]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[6]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[6]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[7]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[7]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[4]);
        
        // Connector sides
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[0]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[4]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[1]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[5]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[2]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[6]);
        
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[3]);
        GL.Vertex(terrainChunkGenerator.boundingBoxCorners[7]);

        GL.End();
    }
}
