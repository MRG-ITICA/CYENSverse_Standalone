// *********************************************
// *********************************************
// <info>
//   File: Annotation.cs
//   Author: Fotos  Frangoudes
//   Project: ViveEyeTracking/Assembly-CSharp
//   Creation Date: 2023/08/29 10:31 PM
//   Last Modification Date: 2023/08/29 11:00 PM
// </info>
// <copyright file="Annotation.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using UnityEngine;

[ExecuteAlways]
public class Annotation : MonoBehaviour
{
    public Transform target;

    [Range(0, 1)]
    public float x;

    [Range(0, 1)]
    public float y;

    public float radius = 50f;

    public float size = 0.1f;

    public float width = 2048;
    public float height = 2048;

    void Update()
    {
        var point = CartesianToSphericalCoordinates(new Vector2(x, y));
        //var point = GetPoint(radius, (x * 360 - 90) * Mathf.Deg2Rad, ((1 - y) * 180 - 90) * Mathf.Deg2Rad);
        if (target == null) return;
        target.localScale = Vector3.one * size;
        target.position = point;
    }

    Vector3 GetPoint(float rho, float theta, float phi)
    {
        float x = rho * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = rho * Mathf.Sin(phi);
        float z = rho * Mathf.Cos(theta) * Mathf.Cos(phi);
        return new Vector3(x, y, z);
    }


    // Turn cartesian to spherical coordinates
    private Vector3 CartesianToSphericalCoordinates(Vector2 position)
    {
        float x = position.x * width;
        float y = (1-position.y) * height;

        float xDegree;
        float yDegree;

        // Calculate (x,y) coordinates according to image's dimensions in unity
        if (x < width / 2)
        {
            xDegree = -(width / 2 - x) * 360 / width;
        }
        else if (x > width / 2)
        {
            xDegree = (x - width / 2) * 360 / width;
        }
        else
        {
            xDegree = 0;
        }

        if (y < height / 2)
        {
            yDegree = -(height / 2 - y) * 180 / height;
        }
        else if (y > height / 2)
        {
            yDegree = (y - height / 2) * 180 / height;
        }
        else
        {
            yDegree = 0;
        }

        // Turn degrees into radians
        float phi = xDegree * Mathf.Deg2Rad;
        float theta = yDegree * Mathf.Deg2Rad;
        Debug.Log($"x: {xDegree}, y: {yDegree}");

        // Calculate spherical coordinates
        float xPos = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float yPos = radius * Mathf.Cos(phi) * Mathf.Sin(theta);
        float zPos = radius * Mathf.Cos(phi);

        return new Vector3(xPos, yPos, zPos);
    }
}