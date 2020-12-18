using UnityEngine;
using UnityEngine.UI;

public class UpdateNoiseOctaves : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Slider _slider;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.value = terrainChunkGenerator.numNoiseOctaves; // Set initial value.
        _slider.onValueChanged.AddListener(OnClick);
    }

    private void OnClick(float sliderValue)
    {
        terrainChunkGenerator.numNoiseOctaves = (int)sliderValue;
    }
}