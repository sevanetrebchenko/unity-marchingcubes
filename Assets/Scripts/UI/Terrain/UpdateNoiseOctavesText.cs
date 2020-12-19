using UnityEngine;
using UnityEngine.UI;

public class UpdateNoiseOctavesText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private int _previousNumNoiseOctaves;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousNumNoiseOctaves = terrainChunkGenerator.numNoiseOctaves; // Set initial value.
        _text.text = "Current value: " + terrainChunkGenerator.numNoiseOctaves.ToString(); // Set initial value.
    }

    private void Update()
    {
        int numNoiseOctaves = terrainChunkGenerator.numNoiseOctaves;

        // Only update text if the number of noise octaves changed since the last frame.
        if (numNoiseOctaves != _previousNumNoiseOctaves)
        {
            _text.text = "Current value: " + numNoiseOctaves.ToString();
            _previousNumNoiseOctaves = numNoiseOctaves;
        }
        
    }
}