using UnityEngine;
using UnityEngine.UI;

public class UpdateWidthText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private int _previousWidth;
    
    private void Start()
    {
        _text = GetComponent<Text>();
        _previousWidth = terrainChunkGenerator.height; // Set initial value.
        _text.text = "Width: " + _previousWidth.ToString();
    }

    private void Update()
    {
        int width = terrainChunkGenerator.width;

        // Only update if the width changed since the last frame.
        if (width != _previousWidth)
        {
            _text.text = "Width: " + width.ToString();
            _previousWidth = width;
        }
    }
}