using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{

    Planet planet;
    Editor shapeEditor;

    public override void OnInspectorGUI() {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            base.OnInspectorGUI();
            if (check.changed) {
                planet.OnInspectorUpdate();
            }
        }

        if (GUILayout.Button("Generate Planet")) {
            planet.OnInspectorUpdate();
        }

        DrawSettingsEditor(planet.shape, planet.OnInspectorUpdate, ref planet.shapeFoldout, ref shapeEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope()) {
                if (foldout) {
                    // Debug.Log("step 1");
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed) {
                        if (onSettingsUpdated != null) {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable() {
        planet = (Planet)target;
    }
}
