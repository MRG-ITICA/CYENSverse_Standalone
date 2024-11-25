// *********************************************
// *********************************************
// <info>
//   File: InputManager.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/09 9:16 AM
//   Last Modification Date: 2023/10/10 9:23 AM
// </info>
// <copyright file="InputManager.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private KeyCode openRandomVideoKeyCode = KeyCode.R;

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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(openRandomVideoKeyCode))
        {
            // TODO: select a random available video and open it
        }
    }
}