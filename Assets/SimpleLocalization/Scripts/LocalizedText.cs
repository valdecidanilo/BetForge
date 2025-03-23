﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Assets.SimpleLocalization.Scripts
{
	/// <summary>
	/// Localize text component.
	/// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        public string LocalizationKey;

        public void Start()
        {
            Localize();
            LocalizationManager.OnLocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.OnLocalizationChanged -= Localize;
        }

        private void Localize()
        {
            GetComponent<TMP_Text>().text = LocalizationManager.Localize(LocalizationKey);
        }
    }
}