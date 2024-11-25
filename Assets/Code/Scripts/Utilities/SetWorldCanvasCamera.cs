// *********************************************
// *********************************************
// <info>
//   File: SetWorldCanvasCamera.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 9:42 AM
//   Last Modification Date: 2023/10/08 8:38 AM
// </info>
// <copyright file="SetWorldCanvasCamera.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using TriInspector;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

[RequireComponent(typeof(Canvas))]
public class SetWorldCanvasCamera : MonoBehaviour
{
#if UNITY_EDITOR

    #region Inspector

    /// <summary>
    ///     Reset Button
    /// </summary>
    [Button("Reset")]
    void ResetButton()
    {
        Reset();
    }

    #endregion Inspector

#endif //UNITY_EDITOR

    #region Unity Messages

    /// <summary>
    ///     Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component
    ///     the first time.
    /// </summary>
    void Reset()
    {
        SetCamera();
    }

    /// <summary>
    ///     Awake is called either when an active GameObject that contains the script is initialized when a Scene loads, or
    ///     when a previously inactive GameObject is set to active, or after a GameObject created with Object.Instantiate is
    ///     initialized.
    /// </summary>
    void Awake()
    {
        SetCamera();
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif //UNITY_EDITOR
        Destroy(this);
    }

    #endregion Unity Messages

    #region Set World Canvas Camera

    /// <summary>
    ///     Set the camera of the canvas
    /// </summary>
    void SetCamera()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas.worldCamera != null) return;

        Camera camera = null;
#if UNITY_EDITOR
        camera = !EditorApplication.isPlayingOrWillChangePlaymode ? Camera.main : XrReferences.XrCamera;
#else
        camera = XrReferences.XrCamera;
#endif
        canvas.worldCamera = camera;
    }

    #endregion Set World Canvas Camera
}