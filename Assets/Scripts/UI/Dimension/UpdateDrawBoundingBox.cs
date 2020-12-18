using UnityEngine;
using UnityEngine.UI;

public class UpdateDrawBoundingBox : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Toggle _toggle;
    
    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = terrainChunkGenerator.drawBoundingBox; // Set initial value.
        _toggle.onValueChanged.AddListener(OnClick);
    }

    private void OnClick(bool drawBoundingBox)
    {
        terrainChunkGenerator.drawBoundingBox = drawBoundingBox;
    }
}
