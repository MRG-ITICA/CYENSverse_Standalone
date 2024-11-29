// *********************************************
// *********************************************
// <info>
//   File: RotateUI.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/06 4:32 PM
// </info>
// <copyright file="RotateUI.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [SerializeField]
    float angularSpeed = 5f;

    [SerializeField]
    float anglesY = 0;

    private void Start()
    {
        XrReferences.Instance.RotateTowardsCamera(transform, 50, anglesY, null);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, angularSpeed * Time.deltaTime, Space.Self);
    }
}