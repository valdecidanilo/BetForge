using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FeatureSceneLoader : EditorWindow
{
    private List<string> featurePaths = new List<string>();
    private List<string> featureNames = new List<string>();
    private int selectedIndex = 0;
    
    private const string FEATURES_PATH = "Assets/Features";
    private const string CORE_SCENE_PATH = "Assets/Core/Scenes/Scene.unity";

    [MenuItem("Tools/Feature Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<FeatureSceneLoader>("Feature Loader");
    }

    private void OnEnable()
    {
        LoadFeatureScenes();
    }

    private void LoadFeatureScenes()
    {
        featurePaths.Clear();
        featureNames.Clear();

        if (Directory.Exists(FEATURES_PATH))
        {
            string[] directories = Directory.GetDirectories(FEATURES_PATH);
            foreach (string dir in directories)
            {
                string featureName = Path.GetFileName(dir);
                string scenePath = Path.Combine(dir, "Scenes");

                if (Directory.Exists(scenePath))
                {
                    string[] sceneFiles = Directory.GetFiles(scenePath, "*.unity");
                    if (sceneFiles.Length > 0)
                    {
                        featurePaths.Add(sceneFiles[0].Replace("\\", "/")); // Ajusta o caminho para o Unity
                        featureNames.Add(featureName);
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Selecione uma Feature para carregar:", EditorStyles.boldLabel);

        if (featureNames.Count == 0)
        {
            EditorGUILayout.HelpBox("Nenhuma feature encontrada!", MessageType.Warning);
            return;
        }

        selectedIndex = EditorGUILayout.Popup("Feature:", selectedIndex, featureNames.ToArray());

        if (GUILayout.Button("Carregar Cena Principal"))
        {
            LoadCoreScene();
        }

        if (GUILayout.Button("Carregar Feature Aditivamente"))
        {
            LoadFeatureScene();
        }
    }

    private void LoadCoreScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(CORE_SCENE_PATH);
        }
    }

    private void LoadFeatureScene()
    {
        if (featurePaths.Count > selectedIndex)
        {
            string selectedScene = featurePaths[selectedIndex];
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(CORE_SCENE_PATH);
                EditorSceneManager.OpenScene(selectedScene, OpenSceneMode.Additive);
            }
        }
    }
}
