using UnityEngine;
using UnityEngine.UI;

public class UpdateDepthText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    
    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        _text.text = "DEPTH: " + terrainChunkGenerator.depth.ToString();
    }
}