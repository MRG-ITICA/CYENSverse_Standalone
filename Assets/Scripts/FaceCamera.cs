// *********************************************
// *********************************************
// <info>
//   File: FaceCamera.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/06 4:03 PM
// </info>
// <copyright file="FaceCamera.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XrReferences.Instance.RotateTowardsCamera(transform, 0, null, 0);
    }
}