// *********************************************
// *********************************************
// <info>
//   File: FollowUser.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/06 6:28 PM
// </info>
// <copyright file="FollowUser.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;
using UnityEngine.Animations;

[DefaultExecutionOrder(int.MaxValue)]
[RequireComponent(typeof(PositionConstraint))]
public class FollowUser : MonoBehaviour
{
    void Awake()
    {
        var positionConstraint = GetComponent<PositionConstraint>();
        positionConstraint.AddSource(new ConstraintSource()
        {
            sourceTransform = XrReferences.XrCameraTransform,
            weight = 1
        });

        Destroy(this);
    }
}