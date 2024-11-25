// *********************************************
// *********************************************
// <info>
//   File: DestroyGameObjectOnBuild.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSVERSE/Assembly-CSharp
//   Creation Date: 2023/10/02 1:02 PM
//   Last Modification Date: 2023/10/02 1:03 PM
// </info>
// <copyright file="DestroyGameObjectOnBuild.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

[DefaultExecutionOrder(int.MinValue)]
public class DestroyGameObjectOnBuild : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }
}