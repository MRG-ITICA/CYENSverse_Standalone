// *********************************************
// *********************************************
// <info>
//   File: AssetsLoadingIndicator.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/09/28 11:09 AM
//   Last Modification Date: 2023/09/28 2:36 PM
// </info>
// <copyright file="AssetsLoadingIndicator.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using TriInspector;

using UnityEngine;
using UnityEngine.UI;

public class AssetsLoadingIndicator : MonoBehaviour
{
    #region Variables

    /// <summary>
    ///     The assets loader that send progress update callbacks
    /// </summary>
    [Title("Assets management")]
    public AssetsLoader loader;

    /// <summary>
    ///     Image showing the progress. This should be set to filled textured.
    /// </summary>
    [Title("Assets management")]
    public Image progressBar;

    #endregion Variables

    #region Unity Messages

    void Awake()
    {
        if (progressBar != null) return;
        Debug.LogWarning($"Progress bar not set for {name}");
        enabled = false;
    }

    /// <summary>
    ///     This function is called when the object becomes enabled and active
    /// </summary>
    void OnEnable()
    {
        if (loader == null)
        {
            loader = AssetsLoader.Instance;
            if (loader == null)
            {
                Debug.LogWarning($"Asset loader not found in {name}");
                enabled = false;
                return;
            }
        }

        loader.OnLoadingProgress.RegisterListener(OnProgressUpdated);
    }

    /// <summary>
    ///     This function is called when the behaviour becomes disabled or inactive
    /// </summary>
    void OnDisable()
    {
        if (loader == null) return;
        loader.OnLoadingProgress.UnregisterListener(OnProgressUpdated);
    }

    #endregion Unity Messages

    #region User Interface

    /// <summary>
    ///     Callback when progress has been updated
    /// </summary>
    /// <param name="value"></param>
    void OnProgressUpdated(float value)
    {
        if (progressBar == null) return;
        progressBar.fillAmount = value;
    }

    #endregion User Interface
}