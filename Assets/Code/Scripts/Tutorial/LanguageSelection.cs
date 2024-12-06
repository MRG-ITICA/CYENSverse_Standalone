using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Localization.Settings;

public class LanguageSelection : MonoBehaviour
{

    [SerializeField]
    private GameObject languageComponent;

    public void SetLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"Language set to {localeCode}");
        }
        else
        {
            Debug.LogError($"Locale with code '{localeCode}' not found!");
        }
    }

    public void FadeIn()
    {
        languageComponent.SetActive(true);
        TMP_Text[] texts = languageComponent.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in texts)
        {
            FadeInWord(t, 2);
        }
    }

    public void FadeInWord(TMP_Text text, float duration)
    {
        StartCoroutine(FadeTextAlpha(text, 1, duration));
    }

    private IEnumerator FadeTextAlpha(TMP_Text text, float to, float duration)
    {
        // Color variables
        var color = text.color;

        // Time variables
        var timeStep = (to - color.a) / duration * Time.fixedDeltaTime;

        var elapsedTime = 0f;
        var waitTime = new WaitForFixedUpdate();
        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;

            color.a += timeStep;

            text.color = color;

            yield return waitTime;
        }

        color.a = to;
        text.color = color;
    }
}
