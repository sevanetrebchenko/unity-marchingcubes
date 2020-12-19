using UnityEngine;
using UnityEngine.UI;

public class UpdateTerrainSeedText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private uint _previousSeed;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousSeed = terrainChunkGenerator.terrainSeed; // Set initial value.
        _text.text = "Current value: " + _previousSeed.ToString();
    }

    private void Update()
    {
        uint terrainSeed = terrainChunkGenerator.terrainSeed;
        
        // Only update text if terrain seed changed since the last frame.
        if (terrainSeed != _previousSeed)
        {
            _text.text = "Current value: " + terrainSeed.ToString();
            _previousSeed = terrainSeed;
        }
    }
}