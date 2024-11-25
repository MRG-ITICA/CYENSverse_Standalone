// *********************************************
// *********************************************
// <info>
//   File: CheckLevel.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverseUrp/Assembly-CSharp
//   Creation Date: 2023/10/02 7:26 PM
//   Last Modification Date: 2023/10/04 5:37 PM
// </info>
// <copyright file="CheckLevel.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using TriInspector;

using UnityEngine;
#if !DONT_USE_DIRECT_VIDEO_REFERENCES
#endif

[DefaultExecutionOrder(int.MinValue + 10)]
public class CheckLevel : MonoBehaviour
{
#if UNITY_EDITOR

    [GUIColor("yellow")]
    [Button(ButtonSizes.Gigantic, "Do not attach any other components or children objects!")]
    public void WarningButton()
    {
    }

    // Start is called before the first frame update
    void Awake()
    {
#if DONT_USE_DIRECT_VIDEO_REFERENCES
        if (AssetsLoader.isInstanceNull)
        {
            SceneManager.LoadScene(0);
        }
#endif //DONT_USE_DIRECT_VIDEO_REFERENCES

        Destroy(gameObject);
    }
#endif //UNITY_EDITOR
}