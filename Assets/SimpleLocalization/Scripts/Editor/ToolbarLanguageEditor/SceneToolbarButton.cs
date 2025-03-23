using System.Linq;
using Assets.SimpleLocalization.Scripts;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleLocalization.Scripts.Editor
{
    [Overlay(typeof(SceneView), "Language Overlay")]
    public class LanguageOverlay : Overlay
    {
        private VisualElement _root;
        private DropdownField _languageDropdown;

        public override VisualElement CreatePanelContent()
        {
            // Cria um container para os elementos
            _root = new VisualElement();

            // Adiciona botões para cada idioma
            foreach (var language in LocalizationManager.Dictionary.Keys)
            {
                var button = new Button(() => ChangeLanguage(language))
                {
                    text = language,
                    tooltip = $"Mudar para {language}"
                };
                _root.Add(button);
            }

            // Adiciona um dropdown para selecionar o idioma
            _languageDropdown = new DropdownField("Selecione o Idioma", LocalizationManager.Dictionary.Keys.ToList(), 0);
            _languageDropdown.RegisterValueChangedCallback(evt => ChangeLanguage(evt.newValue));
            _root.Add(_languageDropdown);

            return _root;
        }

        private void ChangeLanguage(string language)
        {
            // Muda o idioma
            LocalizationManager.Language = language;
            Debug.Log($"Idioma alterado para: {language}");
        }
    }
}