using UnityEngine;
using UnityEngine.UI;

public class UpdatePersistenceText : MonoBehaviour
{
    public TerrainChunkGenerator terrainChunkGenerator;
    private Text _text;
    private float _previousPersistence;

    private void Start()
    {
        _text = GetComponent<Text>();
        _previousPersistence = terrainChunkGenerator.persistence; // Set initial value.
        _text.text = "Current value: " + _previousPersistence.ToString("F3"); // Round to 3 decimal places.
    }

    private void Update()
    {
        float persistence = terrainChunkGenerator.persistence;
        
        // Only update text if persistence changed since the last frame.
        if (!Mathf.Approximately(persistence, _previousPersistence))
        {
            _text.text = "Current value: " + persistence.ToString("F3"); // Round to 3 decimal places.
            _previousPersistence = persistence;
        }
    }
}