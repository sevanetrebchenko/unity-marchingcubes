using UnityEngine;
using UnityEngine.UI;

public class ToggleCubeMarchingAnimation : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Button _button;
    private bool _animateCubeMarching;
        
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        _animateCubeMarching = _button.enabled;
    }

    private void OnClick()
    {
        _animateCubeMarching = !_animateCubeMarching;
        terrainChunkGenerator.animateCubeMarching = _animateCubeMarching;

        if (_animateCubeMarching)
        {
            terrainChunkGenerator.ResetMarchingCubes();
        } 
    }
}
