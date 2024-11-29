// *********************************************
// *********************************************
// <info>
//   File: Credits.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/06 4:33 PM
// </info>
// <copyright file="Credits.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;

using TriInspector;

using UnityEngine;

public class Credits : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private bool hideOnStart = true;

    [Title("Speakers")]
    [SerializeField]
    private CanvasGroup speakersCanvasGroup;

    [SerializeField]
    private float speakersFadeTime = .5f;

    [SerializeField]
    private float speakersDisplayTime = 5;

    [Title("Team credits")]
    [SerializeField]
    private CanvasGroup creditsCanvasGroup;

    [InfoBox(
        "Note: Length of credits animation is computed based on clip length.\nAs such the speed modifier should be changed from the default value of 1 in the animator.",
        TriMessageType.Warning)]
    [SerializeField]
    private Animator creditsAnimator;

    public Action OnCreditsSequenceFinished;

    #endregion Variables

    #region Unity Messages

    void Start()
    {
        if (!hideOnStart) return;

        Hide();
    }

    void OnEnable()
    {

        // Start the credits display
        StartCoroutine(PlayClosingSequence());
    }

    #endregion Unity Messages

    #region State Management

    /// <summary>
    ///     Show credits sequence
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    ///     Hide the credits sequence
    /// </summary>
    public void Hide()
    {
        speakersCanvasGroup.alpha = 0;
        creditsCanvasGroup.alpha = 0;

        gameObject.SetActive(false);
    }

    #endregion State Management

    #region Sequence Management

    /// <summary>
    ///     Play the credits sequence
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayClosingSequence()
    {
        speakersCanvasGroup.alpha = 0;
        creditsCanvasGroup.alpha = 0;

        yield return ShowSpeakers();
        yield return ShowCredits();

        OnCreditsSequenceFinished?.Invoke();
        Hide();
    }

    /// <summary>
    ///     Show the speakers
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowSpeakers()
    {
        var fixedWaitTime = new WaitForFixedUpdate();
        yield return new WaitForSeconds(2f);

        var creditsTransform = GetComponent<Transform>();

        // Set credits position
        XrReferences.Instance.PositionTowardsCamera(creditsTransform);

        // Fade credits in
        var timer = 0f;
        var timeStep = Time.fixedDeltaTime / speakersFadeTime;
        speakersCanvasGroup.alpha = 0;
        while (timer < speakersFadeTime)
        {
            speakersCanvasGroup.alpha += timeStep;
            yield return fixedWaitTime;
            timer += Time.fixedDeltaTime;
        }

        speakersCanvasGroup.alpha = 1;

        // Wait for some time
        yield return new WaitForSeconds(speakersDisplayTime - 1);

        // Fade credits out
        timer = speakersFadeTime;
        while (timer > 0.0f)
        {
            speakersCanvasGroup.alpha -= timeStep;
            yield return fixedWaitTime;
            timer -= Time.fixedDeltaTime;
        }

        speakersCanvasGroup.alpha = 0;
    }

    /// <summary>
    ///     Show the credits animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowCredits()
    {
        var creditsDisplayTime = 0f;
        var creditsAnimationClips = creditsAnimator.runtimeAnimatorController.animationClips;
        for (int i = 0, n = creditsAnimationClips.Length; i < n; i++)
        {
            var clip = creditsAnimationClips[i];
            if (clip == null) continue;
            creditsDisplayTime += clip.length;
        }

        creditsAnimator.SetTrigger("ShowCredits");
        yield return new WaitForSeconds(creditsDisplayTime);
        yield break;
    }

    #endregion Sequence Management
}