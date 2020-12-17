using UnityEngine;
using UnityEngine.UI;

public class MarchingCubesController : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Button _startButton;
    private Button _resetButton;
    private Button _resumeButton;
    private Button _stepForwardButton;
    private Button _stepBackwardButton;
    private Button _pauseButton;
    
    private void Start()
    {
        _startButton = GetChildComponentByName("StartCubeMarchingButton");
        _resetButton = GetChildComponentByName("ResetCubeMarchingButton");
        _resumeButton = GetChildComponentByName("ResumeCubeMarchingButton");
        _stepForwardButton = GetChildComponentByName("StepCubeForwardButton");
        _stepBackwardButton = GetChildComponentByName("StepCubeBackwardButton");
        _pauseButton = GetChildComponentByName("PauseCubeMarchingButton");
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
            if (terrainChunkGenerator.pauseGeneration) 
            {
                _resumeButton.gameObject.SetActive(true);
                _stepForwardButton.gameObject.SetActive(true);
                _stepBackwardButton.gameObject.SetActive(true);
                
                _startButton.gameObject.SetActive(false);
                _resetButton.gameObject.SetActive(false);
            }
            else 
            {
                _pauseButton.gameObject.SetActive(true);
                
                _resumeButton.gameObject.SetActive(false);
                _stepForwardButton.gameObject.SetActive(false);
                _stepBackwardButton.gameObject.SetActive(false);
                _startButton.gameObject.SetActive(false);
            }

            _resetButton.gameObject.SetActive(true);
        }
    }
}
