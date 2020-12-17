using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainChunkGenerator))]
public class SingleTerrainChunkEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainChunkGenerator chunkGenerator = (TerrainChunkGenerator)target;
        base.OnInspectorGUI();
        
        if (chunkGenerator.animateCubeMarching) {
            GUILayout.BeginHorizontal();

            // If the terrain generator is not current generating terrain, show the start button.
            if (!chunkGenerator.generatingTerrain) {
                if (GUILayout.Button("Start")) {
                    chunkGenerator.StartMarchingCubes();
                }
            }
            else {
                if (chunkGenerator.pauseGeneration) {
                    if (GUILayout.Button("Resume")) {
                        chunkGenerator.ToggleMarchingCubesPaused(false);
                    }

                    if (GUILayout.Button("Step Forward")) {
                        chunkGenerator.StepMarchingCubes(true);
                    }

                    if (GUILayout.Button("Step Backward")) {
                        chunkGenerator.StepMarchingCubes(false);
                    }
                }
                else {
                    if (GUILayout.Button("Pause")) {
                        chunkGenerator.ToggleMarchingCubesPaused(true);
                    }
                }

                if (GUILayout.Button("Reset")) {
                    chunkGenerator.ResetMarchingCubes();
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}