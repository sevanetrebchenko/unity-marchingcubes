using UnityEngine;
using UnityEngine.UI;

public class UpdateHeightText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private int _previousHeight;
    
    private void Start()
    {
        _text = GetComponent<Text>();
        _previousHeight = terrainChunkGenerator.height; // Set initial value.
        _text.text = "Height: " + _previousHeight.ToString();
    }

    private void Update()
    {
        int height = terrainChunkGenerator.height;

        // Only update if the height changed since the last frame.
        if (height != _previousHeight)
        {
            _text.text = "Height: " + height.ToString();
            _previousHeight = height;
        }
    }
}