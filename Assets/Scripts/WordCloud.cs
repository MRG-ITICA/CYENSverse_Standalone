// *********************************************
// *********************************************
// <info>
//   File: WordCloud.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/09 8:04 AM
// </info>
// <copyright file="WordCloud.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Collections;

using TMPro;

using TriInspector;

using UnityEngine;
using UnityEngine.Serialization;

public class WordCloud : MonoBehaviour
{
    #region Variables

#if UNITY_EDITOR
    [SerializeField]
    private bool hideOnAwake = true;
#endif // UNITY_EDITOR

    [SerializeField]
    private Categories categories;

    [Title("Future")]
    public int futureDuration;

    [SerializeField]
    private GameObject FutureRootTransform;

    [Title("Words")]
    [SerializeField]
    private WordsFile wordsFile;

    [SerializeField]
    private UsersFile usersFile;

#if UNITY_EDITOR
    [SerializeField]
#endif
    private WordData[] words;

    [Title("Word cloud")]
    [SerializeField]
    private Transform worldCloudRoot;

    [SerializeField]
    private float wordCloudFloorOffset = 10;

    [SerializeField]
    private GameObject wordPrefab;

    public float size = 10f;

    [SerializeField]
    private float minimumWordCloudDisplayTime = 10f;

    [SerializeField]
    private float timeDelayBetweenWordsAppearance = 0.2f;

    [SerializeField]
    private ContentController contentController;

    #region Message

    [Title("Message")]
    [SerializeField]
    private CanvasGroup messageCanvas;

    [SerializeField]
    private TMP_Text[] messageLabels;

    [SerializeField]
    [Tooltip("Time before the question will appear in seconds")]
    private float messageDisplayTimeDelay = 5f;

    [SerializeField]
    [Tooltip("The duration the question will be shown in seconds")]
    private float messagesDuration = 5f;

    #endregion Message

    #region Question

    [Title("Question")]
    [SerializeField, TextArea]
    private string questionText = "How do you imagine the future\nof the Buffer Zone?";

    #endregion Question

    #region All user selections

    [Title("All user selections")]
    [SerializeField]
    private CanvasGroup interactionCanvasGroup;

    [SerializeField, TextArea]
    private string allUserSelectionText = "A shared future\ncrafted by everyone,\nfor everyone...";

    [FormerlySerializedAs("allUserSelectionDisplayTime")]
    [SerializeField]
    private float allUserSelectionsDisplayTime = 10f;

    public Action OnFutureSequenceEnded;

    #endregion All user selections

    #endregion Variables

    #region Unity Messages

    void Reset()
    {
        if (wordsFile == null) wordsFile = GetComponent<WordsFile>();
        if (categories == null) categories = FindAnyObjectByType<Categories>();
    }

    void Awake()
    {
        if (categories == null)
            categories = Categories.Instance;

        RemoveWorldCloud();

        LoadWords();
#if UNITY_EDITOR
        if (!hideOnAwake) return;
#endif //UNITY_EDITOR
        Hide();
    }

    void OnEnable()
    {
        contentController.SetFloor360Mode(true);

        RemoveWorldCloud();

        StartCoroutine(ShowFutureSequence());
    }

    #endregion Unity Messages

    #region Words I/O

    private void LoadWords()
    {
        words = wordsFile.LoadWords();
    }

    private void SaveWords()
    {
        wordsFile.SaveWords(words);
        usersFile.SaveUserWords();
    }

    #endregion Words I/O

    #region Word Cloud State

    public void Show()
    {
        FutureRootTransform.SetActive(true);
    }

    public void Hide()
    {
        FutureRootTransform.SetActive(false);
    }

    /// <summary>
    ///     Remove world cloud by destroying the visuals and hiding any displayed message
    /// </summary>
    private void RemoveWorldCloud()
    {
        DestroyWordVisuals();
        HideMessage();
    }

    #endregion Word Cloud State

    // Update is called once per frame
    //void Update()
    //{
    //    Transform camera = Camera.main.transform;

    //    // Tell each of the objects to look at the camera
    //    foreach (Transform child in transform)
    //    {
    //        child.LookAt(camera.position);
    //        child.Rotate(0.0f, 180.0f, 0.0f);
    //    }
    //}

    #region Message

    /// <summary>
    ///     Fade the question in and out
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowMessage(string message, float displayTime = 8)
    {
        if (displayTime < 1)
        {
            Debug.LogWarning($"Display duration for message '{message}' is less than 1!");
            displayTime = 2;
        }

        XrReferences.Instance.PositionTowardsCamera(messageCanvas.transform);

        messageCanvas.alpha = 0;

        foreach (TMP_Text m in messageLabels)
        {
            m.text = message;
        }
        
        var waitTime = new WaitForFixedUpdate();
        var alphaDeltaProgress = Time.fixedDeltaTime * 2;

        // Fade title in
        var elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            messageCanvas.alpha += alphaDeltaProgress;
            yield return waitTime;
            elapsedTime += Time.fixedDeltaTime;
        }

        messageCanvas.alpha = 1;

        // Wait for a bit
        yield return new WaitForSeconds(displayTime - 1);

        // Fade message out
        elapsedTime = 0f;
        while (elapsedTime < .5f)
        {
            messageCanvas.alpha -= alphaDeltaProgress;
            yield return waitTime;
            elapsedTime += Time.fixedDeltaTime;
        }

        // Hide message
        HideMessage();
    }

    /// <summary>
    ///     Hide the message
    /// </summary>
    private void HideMessage()
    {
        messageCanvas.alpha = 0;
        foreach (TMP_Text m in messageLabels)
        {
            m.text = String.Empty;
        }
    }

    #region Transform Manipulation

    /// <summary>
    ///     Set the credits position so they are in front of the camera
    /// </summary>
    /// <param name="messageTransform"></param>
    /// <param name="xrCameraTransform"></param>
    private void SetMessagePosition(Transform messageTransform, Transform xrCameraTransform)
    {
        var cameraForward = xrCameraTransform.forward;
        var mesasagePosition = cameraForward * 6;

        mesasagePosition.y = messageTransform.position.y;

        messageTransform.position = mesasagePosition;
    }

    /// <summary>
    ///     Set the credits rotation so they face the camera
    /// </summary>
    /// <param name="messageTransform"></param>
    /// <param name="xrCameraTransform"></param>
    private void SetMessageRotation(Transform messageTransform, Transform xrCameraTransform)
    {
        var messageRotation = messageTransform.localEulerAngles;
        messageRotation.y = xrCameraTransform.eulerAngles.y;
        messageTransform.localEulerAngles = messageRotation;
    }

    #endregion Transform Manipulation

    #endregion Message

    // Wait for the duration of the future category before fading out the initial
    // word cloud and fading in the final word cloud
    private IEnumerator ShowFutureSequence()
    {
        // Wait for a bit before start showing anything
        yield return new WaitForSeconds(messageDisplayTimeDelay);

        // Ask the question about the future
        yield return StartCoroutine(ShowMessage(questionText, messagesDuration));

        interactionCanvasGroup.interactable = true;

        // and then start showing the word cloud
        StartCoroutine(CreateWords());
    }

    // Create and position the word objects, fade them in one by one
    private IEnumerator CreateWords()
    {
        var worldCenter = new Vector3(0, 1.5f, 0);
        float increment = Mathf.PI * (3 - Mathf.Sqrt(5));
        float offsetY = 0.6f; // Offset to avoid poles

        var waitTime = new WaitForSeconds(timeDelayBetweenWordsAppearance);

        var wordCount = words.Length;
        for (int i = 0; i < wordCount; i++)
        {
            float theta = i * increment;
            float y = offsetY * (1f - (i / ((float)wordCount - 1f))); // Gradually decrease y from top to bottom
            float radius = Mathf.Sqrt(1 - y * y);
            Vector3 pos = new Vector3(Mathf.Cos(theta) * radius * size, y * size - wordCloudFloorOffset,
                Mathf.Sin(theta) * radius * size);

            // Create the word object
            var child = Instantiate(wordPrefab, pos, Quaternion.identity, worldCloudRoot);

            var childTransform = child.transform;
            childTransform.LookAt(worldCenter);
            childTransform.Rotate(0.0f, 180.0f, 0.0f);

            var wordController = child.GetComponent<WordController>();
            wordController.SetData(words[i]);
            wordController.FadeInWord();

            // Time spacing between making each word appear in initial word cloud
            yield return waitTime;
        }

        StartCoroutine(ContinueFutureSequence());
    }

    private IEnumerator ContinueFutureSequence()
    {
        // Set display time based on time multiplier in categories
        var displayTime = futureDuration * categories.TimeMultiplier;
        // Subtract the time for the question / messages
        displayTime -= messageDisplayTimeDelay + messagesDuration;
        // and ensure a minimum display duration
        if (displayTime < 0)
            displayTime = minimumWordCloudDisplayTime;

        yield return new WaitForSeconds(displayTime);

        interactionCanvasGroup.interactable = false;

        FadeOutWordCloud();

        // Save the user selections
        SaveWords();

        // wait a bit for words to fade out
        yield return new WaitForSeconds(2f);

        // Show message about all users' selections
        yield return StartCoroutine(ShowMessage(allUserSelectionText, messagesDuration));

        ShowAllUsersSelectionsWordCloud();

        yield return new WaitForSeconds(allUserSelectionsDisplayTime);

        DestroyWordVisuals();

        // Notify completion of word cloud sequence
        OnFutureSequenceEnded?.Invoke();

        Hide();
    }

    private void FadeOutWordCloud()
    {
        // Until it's time to show the results
        for (int i = 0, n = words.Length; i < n; i++)
        {
            if (words[i].wordController == null) continue;
            words[i].wordController.Hide();
        }
    }

    // Create final word cloud with sizes and opacity depending on how many times 
    // each word was selected by all users
    public void ShowAllUsersSelectionsWordCloud()
    {
        int sumOfWordCounts = 0;
        var numberOfWords = words.Length;
        for (int i = 0; i < numberOfWords; i++)
        {
            sumOfWordCounts += words[i].count;
        }

        for (int i = 0; i < numberOfWords; i++)
        {
            var controller = words[i].wordController;
            if (controller == null) continue;
            controller.Deactivate();
            controller.ShowWordBasedOnUserSelections(sumOfWordCounts, numberOfWords);
        }
    }

    private void DestroyWordVisuals()
    {
        foreach (Transform t in worldCloudRoot)
        {
            Destroy(t.gameObject);
        }
    }

    public WordData[] GetWords()
    {
        return words;
    }
}