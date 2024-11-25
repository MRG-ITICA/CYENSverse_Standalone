// *********************************************
// *********************************************
// <info>
//   File: PersistObject.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/09/28 7:49 AM
//   Last Modification Date: 2023/09/28 7:53 AM
// </info>
// <copyright file="PersistObject.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

[DefaultExecutionOrder(int.MinValue)]
public class PersistObject : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private GameObject target;

    #endregion Variables

    #region Unity Messages

    /// <summary>
    ///     Reset to default values
    /// </summary>
    void Reset()
    {
        target = gameObject;
    }

    /// <summary>
    ///     Start is called before the first frame update
    /// </summary>
    void Awake()
    {
        // Market target gameobject not to be destroyed
        DontDestroyOnLoad(target);

        // Destroy this scripts since it has done its job
        Destroy(this);
    }

    #endregion Unity Messages
}