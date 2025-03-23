using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleLocalization.Scripts.Editor
{
    /// <summary>
    /// Adds "Sync" button to LocalizationSync script.
    /// </summary>
    [CustomEditor(typeof(TMP_Text))]
    public class TextEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (TMP_Text) target;

            if (component.GetComponent<LocalizedText>()) return;

            if (GUILayout.Button("Localize"))
            {
                component.gameObject.AddComponent<LocalizedText>();
            }
        }
    }
}