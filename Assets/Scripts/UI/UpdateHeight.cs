using UnityEngine;
using UnityEngine.UI;

public class UpdateHeight : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Slider _slider;
    private float _value;
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _value = _slider.value;
    }

    private void Update()
    {
        float value = _slider.value;
        if (value != _value)
        {
            terrainChunkGenerator.height = (int)_slider.value;
            _value = value;
        }
    }
}