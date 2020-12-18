using UnityEngine;
using UnityEngine.UI;

public class UpdateDepthText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private int _previousDepth;
    
    private void Start()
    {
        _text = GetComponent<Text>();
        _previousDepth = terrainChunkGenerator.depth; // Set initial value.
        _text.text = "Depth: " + _previousDepth.ToString();
    }

    private void Update()
    {
        int depth = terrainChunkGenerator.depth;

        // Only update if the depth changed since the last frame.
        if (depth != _previousDepth)
        {
            _text.text = "Depth: " + depth.ToString();
            _previousDepth = depth;
        }
    }
}