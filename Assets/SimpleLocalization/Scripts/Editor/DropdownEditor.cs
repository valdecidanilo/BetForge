using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Assets.SimpleLocalization.Scripts.Editor
{
    /// <summary>
    /// Adds "Sync" button to LocalizationSync script.
    /// </summary>
    [CustomEditor(typeof(TMP_Dropdown))]
    public class DropdownEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (TMP_Dropdown) target;

            if (component.GetComponent<LocalizedText>()) return;

            if (GUILayout.Button("Localize"))
            {
                component.gameObject.AddComponent<LocalizedText>();
            }
        }
    }
}