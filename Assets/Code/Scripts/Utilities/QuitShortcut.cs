// *********************************************
// *********************************************
// <info>
//   File: QuitShortcut.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverseUrp/Assembly-CSharp
//   Creation Date: 2023/10/04 6:54 PM
//   Last Modification Date: 2023/10/04 6:55 PM
// </info>
// <copyright file="QuitShortcut.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

public class QuitShortcut : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }
    }
}