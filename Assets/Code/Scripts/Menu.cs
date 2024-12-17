// *********************************************
// *********************************************
// <info>
//   File: Menu.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/10 8:33 AM
// </info>
// <copyright file="Menu.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using TriInspector;

using UnityEngine;
using UnityEngine.Video;

public class Menu : MonoBehaviour
{
    #region Variables

    [Title("Interaction")]
    [SerializeField]
    private bool hideOnStart = false;

    [SerializeField]
    private bool autoLoadCyensVerse = false;

    public float imageScaleFactor = 1.2f;

    private Vector3 initialScale;

    [SerializeField]
    private SpinnerController loadingEffect;

    [Title("Visuals")]
    [SerializeField]
    private Transform cyensVerseButtonRoot;

    private FadeController fader;

    [SerializeField]
    private PopUpController popUpController;

    #endregion Variables

    #region Unity Messages

    void Awake()
    {
        fader = XrReferences.FadeToBlack;
    }

    // Start is called before the first frame update
    void Start()
    {
        popUpController = FindObjectOfType<PopUpController>();
        if (autoLoadCyensVerse)
        {
            LoadCyensVerse();
            return;
        }
        if (!hideOnStart) return;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        initialScale = cyensVerseButtonRoot.localScale;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    #endregion Unity Messages

    #region Interaction

    public void HoverEntered()
    {
        ScaleUp();
        StartCoroutine(Hovering());
    }

    IEnumerator Hovering()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(Loading());
    }

    private void ScaleUp()
    {
        cyensVerseButtonRoot.localScale *= imageScaleFactor;
    }

    private void ScaleDown()
    {
        cyensVerseButtonRoot.localScale = initialScale;
    }

    IEnumerator Loading()
    {
        popUpController.handAnimation.SetActive(false);
        StartCoroutine(popUpController.FadeOutPopUp());
        loadingEffect.Show();
        loadingEffect.Load();
        yield return new WaitUntil(() => !loadingEffect.IsLoading());
        loadingEffect.Hide();
        FindObjectOfType<ContentController>().SetFloor360Mode(false);
        LoadCyensVerse();
    }

    public void HoverExited()
    {
        StopCoroutine(Loading());
        StopCoroutine(Hovering());
        ScaleDown();
        loadingEffect.Hide();
    }

    public void LoadCyensVerse()
    {
        fader.LoadMainScene(true, 0.5f, 3f);
    }

    #endregion Interaction
}