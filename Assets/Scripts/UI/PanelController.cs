using UnityEngine;

public class PanelController : MonoBehaviour
{
    private GameObject _dimensionPanel;
    private bool _dimensionPanelPrevState;

    private GameObject _terrainButton;
    private GameObject _terrainPanel;
    private bool _terrainPanelPrevState;

    private GameObject _marchingCubesButton;
    private GameObject _marchingCubesPanel;

    private float _dimensionTerrainPanelShift;
    private float _terrainMarchingCubesPanelShift;

    private void Start()
    {
        _dimensionPanel = GameObject.Find("DimensionControllerPanel");
        _dimensionPanelPrevState = _dimensionPanel.activeSelf;
        
        _terrainButton = GameObject.Find("TerrainMenuButton");
        _terrainPanel = GameObject.Find("TerrainControllerPanel");
        _terrainPanelPrevState = _terrainPanel.activeSelf;
        
        _marchingCubesButton = GameObject.Find("MarchingCubesMenuButton");
        _marchingCubesPanel = GameObject.Find("MarchingCubesControllerPanel");

        _dimensionTerrainPanelShift = _dimensionPanel.GetComponent<RectTransform>().sizeDelta.y;
        _terrainMarchingCubesPanelShift = _terrainPanel.GetComponent<RectTransform>().sizeDelta.y;
    }

    private void Update()
    {
        // User toggled the dimension panel.
        if (_dimensionPanel.activeSelf != _dimensionPanelPrevState)
        {
            // Panel was activated, shift elements below down.
            if (_dimensionPanel.activeSelf)
            {
                // Shift terrain elements.
                ShiftGameObject(_terrainButton, -_dimensionTerrainPanelShift);
                ShiftGameObject(_terrainPanel, -_dimensionTerrainPanelShift);
                
                // Shift marching cubes elements.
                ShiftGameObject(_marchingCubesButton, -_dimensionTerrainPanelShift);
                ShiftGameObject(_marchingCubesPanel, -_dimensionTerrainPanelShift);
            }
            // Panel was deactivated, shift elements positions up.
            else
            {
                // Shift terrain elements.
                ShiftGameObject(_terrainButton, _dimensionTerrainPanelShift);
                ShiftGameObject(_terrainPanel, _dimensionTerrainPanelShift);
                
                // Shift marching cubes elements.
                ShiftGameObject(_marchingCubesButton, _dimensionTerrainPanelShift);
                ShiftGameObject(_marchingCubesPanel, _dimensionTerrainPanelShift);
            }

            _dimensionPanelPrevState = _dimensionPanel.activeSelf;
        }
        
        // User toggled the terrain options panel.
        if (_terrainPanel.activeSelf != _terrainPanelPrevState)
        {
            // Panel was activated, shift elements below down.
            if (_terrainPanel.activeSelf)
            {
                ShiftGameObject(_marchingCubesButton, -_terrainMarchingCubesPanelShift);
                ShiftGameObject(_marchingCubesPanel, -_terrainMarchingCubesPanelShift);
                
            }
            // Panel was deactivated, shift elements positions up.
            else
            {
                ShiftGameObject(_marchingCubesButton, _terrainMarchingCubesPanelShift);
                ShiftGameObject(_marchingCubesPanel, _terrainMarchingCubesPanelShift);
            }

            _terrainPanelPrevState = _terrainPanel.activeSelf;
        }
    }

    private void ShiftGameObject(GameObject gObject, float shift)
    {
        Vector3 position = gObject.transform.localPosition;
        position.y += shift;
        gObject.transform.localPosition = position;
    }
}
