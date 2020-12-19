using UnityEngine;
using UnityEngine.UI;

public class UpdateLacunarityText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private float _previousLacunarity;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousLacunarity = terrainChunkGenerator.lacunarity; // Set initial value.
        _text.text = "Current value: " + terrainChunkGenerator.lacunarity.ToString("F3"); // Round to 3 decimal places.
    }

    private void Update()
    {
        float lacunarity = terrainChunkGenerator.lacunarity;

        if (!Mathf.Approximately(lacunarity, _previousLacunarity))
        {
            _text.text = "Current value: " + lacunarity.ToString("F3"); // Round to 3 decimal places.
            _previousLacunarity = lacunarity;
        }
        
    }
}