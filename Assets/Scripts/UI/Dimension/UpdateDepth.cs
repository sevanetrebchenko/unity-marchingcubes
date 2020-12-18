using UnityEngine;
using UnityEngine.UI;

public class UpdateDepth : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Slider _slider;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.value = terrainChunkGenerator.depth; // Set initial value.
        _slider.onValueChanged.AddListener(OnClick);
    }

    private void OnClick(float sliderValue)
    {
        terrainChunkGenerator.depth = (int)sliderValue;
    }
}