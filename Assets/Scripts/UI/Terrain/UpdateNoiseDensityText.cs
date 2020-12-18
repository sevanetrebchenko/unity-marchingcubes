using UnityEngine;
using UnityEngine.UI;

public class UpdateNoiseDensityText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private float _previousNoiseDensity;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousNoiseDensity = terrainChunkGenerator.noiseDensity; // Set initial value.
        _text.text = "Noise Density: " + _previousNoiseDensity.ToString("F3"); // Round to 3 decimal places.
    }

    private void Update()
    {
        float noiseDensity = terrainChunkGenerator.noiseDensity;

        // Only update text if the noise density changed since the last frame.
        if (!Mathf.Approximately(noiseDensity, _previousNoiseDensity))
        {
            _text.text = "Noise Density: " + noiseDensity.ToString("F3"); // Round to 3 decimal places.
            _previousNoiseDensity = noiseDensity;
        }
    }
}
