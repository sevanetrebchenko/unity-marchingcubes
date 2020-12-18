using UnityEngine;
using UnityEngine.UI;

public class UpdateDrawTerrainNodes : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Toggle _toggle;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = terrainChunkGenerator.drawTerrainNodes;
        _toggle.onValueChanged.AddListener(OnClick);
    }

    private void OnClick(bool drawTerrainNodes)
    {
        terrainChunkGenerator.drawTerrainNodes = drawTerrainNodes;
    }
}
