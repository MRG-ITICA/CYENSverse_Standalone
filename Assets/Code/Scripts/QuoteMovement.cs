using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteMovement : MonoBehaviour
{
    [SerializeField]
    float angularSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, angularSpeed * Time.deltaTime, Space.Self);
    }
}
