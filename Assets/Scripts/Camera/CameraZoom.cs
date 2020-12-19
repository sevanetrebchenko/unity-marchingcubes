using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    public Transform target;

    private float _maximumAxis;
    
    // Lerping for zooming in.
    private int _zoomInInterpolationFramesCount; 
    private int _zoomInElapsedFrames;
    private bool _zoomInlerping;
    
    // Lerping for zooming out.
    private int _zoomOutInterpolationFramesCount; 
    private int _zoomOutElapsedFrames;
    private bool _zoomOutlerping;

    private void Start()
    {
        _maximumAxis = Mathf.Max(Mathf.Max(terrainChunkGenerator.width, terrainChunkGenerator.height), terrainChunkGenerator.depth);
        
        _zoomInInterpolationFramesCount = 150; // Number of frames to completely interpolate between the 2 positions.
        _zoomInElapsedFrames = 0;
        _zoomInlerping = false;
        
        _zoomOutInterpolationFramesCount = 150; // Number of frames to completely interpolate between the 2 positions.
        _zoomOutElapsedFrames = 0;
        _zoomOutlerping = false;
    }

    private void Update()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        if (!_zoomInlerping && !_zoomOutlerping)
        {
            position += forward * Input.mouseScrollDelta.y;
        }
        
        _maximumAxis = Mathf.Max(Mathf.Max(terrainChunkGenerator.width, terrainChunkGenerator.height), terrainChunkGenerator.depth);
        float maximumDistance = _maximumAxis + 25.0f;
        float minimumDistance = _maximumAxis + 5.0f;
        float distance = Vector3.Distance(position, target.position);
        
        // Limit how far you can zoom in.
        // Distance from camera to target is smaller than radius, move camera back to a position where it is not intersecting.
        if (distance < minimumDistance)
        {
            Vector3 desiredPosition = position - forward * ((minimumDistance - distance) * 1.5f);
            Vector3 offsetPosition = position + (forward * 0.1f);
            
            float interpolationRatio = (float)_zoomOutElapsedFrames / _zoomOutInterpolationFramesCount;
            position = Vector3.Lerp(offsetPosition, desiredPosition, interpolationRatio);
            _zoomOutElapsedFrames = (_zoomOutElapsedFrames + 1) % (_zoomOutInterpolationFramesCount + 1);  // Reset elapsedFrames to zero after it reached.

            _zoomOutlerping = _zoomOutElapsedFrames != 0;
        }
        // Limit how far you can zoom out.
        else if (distance > maximumDistance)
        {
            Vector3 desiredPosition = position + forward * ((distance - maximumDistance) * 2.0f);
            Vector3 offsetPosition = position - (forward * 0.5f);
            
            float interpolationRatio = (float)_zoomInElapsedFrames / _zoomInInterpolationFramesCount;
            position = Vector3.Lerp(offsetPosition, desiredPosition, interpolationRatio);
            _zoomInElapsedFrames = (_zoomInElapsedFrames + 1) % (_zoomInInterpolationFramesCount + 1);  // Reset elapsedFrames to zero after it reached.

            _zoomInlerping = _zoomInElapsedFrames != 0;
        }

        transform.position = position;
    }
    
    
}
