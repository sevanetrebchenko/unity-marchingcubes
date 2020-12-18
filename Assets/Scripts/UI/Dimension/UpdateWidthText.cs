using UnityEngine;
using UnityEngine.UI;

public class UpdateWidthText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    
    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        _text.text = "WIDTH: " + terrainChunkGenerator.width.ToString();
    }
}