using UnityEngine;
using UnityEngine.UI;

public class UpdateSurfaceLevelText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private float _previousSurfaceLevel;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousSurfaceLevel = terrainChunkGenerator.surfaceLevel; // Set initial value.
        _text.text = "Current value: " + _previousSurfaceLevel.ToString("F3"); // Round to 3 decimal places.
    }

    private void Update()
    {
        float surfaceLevel = terrainChunkGenerator.surfaceLevel;
        
        // Only update text if surface level changed since the last frame.
        if (!Mathf.Approximately(surfaceLevel, _previousSurfaceLevel))
        {
            _text.text = "Current value: " + surfaceLevel.ToString("F3"); // Round to 3 decimal places.
            _previousSurfaceLevel = surfaceLevel;
        }
    }
}