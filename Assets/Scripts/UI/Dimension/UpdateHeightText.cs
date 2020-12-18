using UnityEngine;
using UnityEngine.UI;

public class UpdateHeightText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    
    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        _text.text = "HEIGHT: " + terrainChunkGenerator.height.ToString();
    }
}