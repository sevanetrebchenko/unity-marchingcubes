using UnityEngine;
using UnityEngine.UI;

public class UpdateDrawMarchingCube : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Toggle _toggle;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = terrainChunkGenerator.drawMarchingCube;
        _toggle.onValueChanged.AddListener(OnClick);
    }

    private void OnClick(bool drawTerrainNodes)
    {
        terrainChunkGenerator.drawMarchingCube = drawTerrainNodes;
    }
}