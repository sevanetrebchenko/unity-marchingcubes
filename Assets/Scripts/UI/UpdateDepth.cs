using UnityEngine;
using UnityEngine.UI;

public class UpdateDepth : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Slider _slider;
    private float _value;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.value = terrainChunkGenerator.depth;
        _value = _slider.value;
    }

    private void Update()
    {
        float value = _slider.value;
        if (value != _value)
        {
            terrainChunkGenerator.depth = (int)_slider.value;
            _value = value;
        }
    }
}