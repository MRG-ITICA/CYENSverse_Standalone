// *********************************************
// *********************************************
// <info>
//   File: SetVideoColliderToUiSize.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 9:42 AM
//   Last Modification Date: 2023/10/08 10:00 AM
// </info>
// <copyright file="SetVideoColliderToUiSize.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SetVideoColliderToUiSize : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private RectTransform videoRoot;

    [SerializeField]
    private BoxCollider videoCollider;

    #endregion Variables

    private void Reset()
    {
        SetReferences();

        SetVideoColliderSize();
    }

    private void Awake()
    {
        SetReferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetVideoColliderSize();

        Destroy(this);
    }

    /// <summary>
    ///     Set the references for the video elements
    /// </summary>
    private void SetReferences()
    {
        if (videoRoot == null)
            videoRoot = transform.parent.GetComponent<RectTransform>();

        if (videoCollider == null)
            videoCollider = GetComponent<BoxCollider>();
    }

    /// <summary>
    ///     Set the collider size
    /// </summary>
    void SetVideoColliderSize()
    {
        if (videoRoot == null || videoCollider == null) return;

        var colliderSize = videoCollider.size;
        var rect = videoRoot.rect;
        colliderSize.x = rect.width;
        colliderSize.y = rect.height;
        videoCollider.size = colliderSize;
    }
}