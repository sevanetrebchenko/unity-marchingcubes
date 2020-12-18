using UnityEngine;
using UnityEngine.UI;

public class MarchingCubesController : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Button _startButton;
    private Button _resumeButton;
    private Button _stepForwardButton;
    private Button _stepBackwardButton;
    private Button _pauseButton;
    
    private Button _resetButton;
    private RectTransform _resetButtonTransform;
    
    private void Start()
    {
        _startButton = GetChildComponentByName("StartCubeMarchingButton");
        _resumeButton = GetChildComponentByName("ResumeCubeMarchingButton");
        _stepForwardButton = GetChildComponentByName("StepCubeForwardButton");
        _stepBackwardButton = GetChildComponentByName("StepCubeBackwardButton");
        _pauseButton = GetChildComponentByName("PauseCubeMarchingButton");
        
        _resetButton = GetChildComponentByName("ResetCubeMarchingButton");
        _resetButtonTransform = _resetButton.GetComponent<RectTransform>();
    }
    
    private Button GetChildComponentByName(string name) 
    {
        foreach (Button component in GetComponentsInChildren<Button>(true)) 
        {
            if (component.gameObject.name == name) 
            {
                return component;
            }
        }
        
        return null;
    }

    private void Update()
    {
        if (!terrainChunkGenerator.generatingTerrain)
        {
            _startButton.gameObject.SetActive(true);
            
            _resetButton.gameObject.SetActive(false);
            _resumeButton.gameObject.SetActive(false);
            _stepForwardButton.gameObject.SetActive(false);
            _stepBackwardButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
        }
        else 
        {
            Vector3 resetButtonRectPosition = _resetButtonTransform.localPosition;
            
            if (terrainChunkGenerator.pauseGeneration) 
            {
                _resumeButton.gameObject.SetActive(true);
                _stepForwardButton.gameObject.SetActive(true);
                _stepBackwardButton.gameObject.SetActive(true);
                
                _pauseButton.gameObject.SetActive(false);
                _startButton.gameObject.SetActive(false);
                _resetButton.gameObject.SetActive(false);

                resetButtonRectPosition.y = -67.6f;
                _resetButtonTransform.localPosition = resetButtonRectPosition;
            }
            else 
            {
                _pauseButton.gameObject.SetActive(true);
                
                _resumeButton.gameObject.SetActive(false);
                _stepForwardButton.gameObject.SetActive(false);
                _stepBackwardButton.gameObject.SetActive(false);
                _startButton.gameObject.SetActive(false);
                
                resetButtonRectPosition.y = 12.8f;
                _resetButtonTransform.localPosition = resetButtonRectPosition;
            }

            _resetButton.gameObject.SetActive(true);
        }
    }
}
