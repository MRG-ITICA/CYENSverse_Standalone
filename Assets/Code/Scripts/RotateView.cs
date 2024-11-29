// *********************************************
// *********************************************
// <info>
//   File: RotateView.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/08 9:53 AM
// </info>
// <copyright file="RotateView.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections;

using UnityEngine;

public class RotateView : MonoBehaviour
{
    public GameObject circularWall;

    private Vector3 lastRightPalmPosition;
    private Vector3 currentRightPalmPosition;
    private Vector3 lastLeftPalmPosition;
    private Vector3 currentLeftPalmPosition;

    private float xDistance;
    private float yDistance;
    private float speed;

    private Transform rightPalm;
    private Transform leftPalm;

    bool rotatedLastFrame;
    bool rotating;

    private Rigidbody m_Rigidbody;

    public float force = 1;

    bool leftGrab = false;
    bool rightGrab = false;

    bool slowDown = false;

    private void Awake()
    {
        m_Rigidbody = circularWall.GetComponent<Rigidbody>();
        m_Rigidbody.ResetCenterOfMass();
        m_Rigidbody.centerOfMass = Vector3.zero;
        m_Rigidbody.inertiaTensorRotation = Quaternion.identity;
        m_Rigidbody.angularVelocity = Vector3.zero;
        rotatedLastFrame = false;
        rotating = false;

        StartCoroutine(FindRightPalm());
        StartCoroutine(FindLeftPalm());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Other methods of rotating
        // Functionality 1: Rotate according to hand movement speed - right hand -> right movement -> right direction, left hand -> left movement -> left direction
        /*if (rightPalm)
        {
            if (!rotatedLastFrame)
            {
                currentRightPalmPosition = rightPalm.transform.position;

                xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;
                yDistance = Math.Abs(currentRightPalmPosition.y - lastRightPalmPosition.y);

                speed = xDistance / Time.deltaTime;

                if (speed > 0.5 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }

        if (leftPalm)
        {
            if (!rotatedLastFrame)
            {
                currentLeftPalmPosition = leftPalm.transform.position;

                xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;
                yDistance = Math.Abs(currentLeftPalmPosition.y - lastLeftPalmPosition.y);

                speed = xDistance / Time.deltaTime;

                if (speed < -0.5 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }*/

        // Functionality 2: Rotate according to hand movement distance - right hand -> right movement -> right direction, left hand -> left movement -> left direction
        /*if (rightPalm)
        {
            if (!rotatedLastFrame)
            {
                currentRightPalmPosition = rightPalm.transform.position;

                xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;
                yDistance = Math.Abs(currentRightPalmPosition.y - lastRightPalmPosition.y);

                if (xDistance > 0.005 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, xDistance / 0.005f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }

        if (leftPalm)
        {
            if (!rotatedLastFrame)
            {
                currentLeftPalmPosition = leftPalm.transform.position;

                xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;
                yDistance = Math.Abs(currentLeftPalmPosition.y - lastLeftPalmPosition.y);

                if (xDistance < -0.005 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, xDistance / 0.005f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }*/

        // Functionality 3: Rotate according to hand movement speed (right hand -> left movement -> left direction, left hand -> right movement -> right direction)
        /*if (rightPalm)
        {
            if (!rotatedLastFrame)
            {
                currentRightPalmPosition = rightPalm.transform.position;

                xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;
                yDistance = Math.Abs(currentRightPalmPosition.y - lastRightPalmPosition.y);

                speed = xDistance / Time.deltaTime;

                if (speed < -0.4 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }

        if (leftPalm)
        {
            if (!rotatedLastFrame)
            {
                currentLeftPalmPosition = leftPalm.transform.position;

                xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;
                yDistance = Math.Abs(currentLeftPalmPosition.y - lastLeftPalmPosition.y);

                speed = xDistance / Time.deltaTime;

                if (speed > 0.4 && yDistance < 0.004)
                {
                    circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
                    rotatedLastFrame = true;
                }
            }
            else
            {
                rotatedLastFrame = false;
            }
        }*/

        // Functionality 4, 5: Grab sphere/torus and rotate according to hand movement speed
        /*if (rightPalm && rotating)
        {
          currentRightPalmPosition = rightPalm.transform.position;
          xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;

          speed = xDistance / Time.deltaTime;

            if (speed > 0.5 || speed < -0.5)
            {
                circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
            }
        }

        if (leftPalm && rotating)
        {
            currentLeftPalmPosition = leftPalm.transform.position;
            xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;

            speed = xDistance / Time.deltaTime;

            if (speed > 0.5 || speed < -0.5)
            {
                circularWall.transform.Rotate(new Vector3(0, 0, speed / 0.2f));
            }
        }*/

        // Functionality 6, 7: Grab sphere/torus and rotate according to hand movement distance (remove RotateGrab method)
        if (rightPalm && rightGrab)
        {
            currentRightPalmPosition = rightPalm.transform.position;
            xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;

            if (xDistance > 0.005 || xDistance < -0.005)
            {
                circularWall.transform.Rotate(new Vector3(0, xDistance / 0.004f, 0));
            }
        }

        if (leftPalm && leftGrab)
        {
            currentLeftPalmPosition = leftPalm.transform.position;
            xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;

            if (xDistance > 0.005 || xDistance < -0.005)
            {
                circularWall.transform.Rotate(new Vector3(0, xDistance / 0.004f, 0));
            }
        }

        // Functionality 8: Grab wall and rotate with physics (normally replace "right/leftgrab" with "rotating")
        /*if (rightPalm && rightGrab)
        {
            currentRightPalmPosition = rightPalm.transform.position;
            xDistance = currentRightPalmPosition.x - lastRightPalmPosition.x;

            speed = xDistance / Time.deltaTime;

            if (speed > 0.7 && yDistance < 0.004 && rightGrab)
            {
                m_Rigidbody.angularDrag = 0.3f;
                m_Rigidbody.inertiaTensor = new Vector3(0, 0, 1);
                m_Rigidbody.AddRelativeTorque(0, 0, speed/7, ForceMode.VelocityChange);
            }

            if (speed < -0.8 && yDistance < 0.004 && rightGrab)
            {
                m_Rigidbody.angularDrag = 0.3f;
                m_Rigidbody.inertiaTensor = new Vector3(0, 0, 1);
                m_Rigidbody.AddRelativeTorque(0, 0, speed/7, ForceMode.VelocityChange);
            }

        }

        if (leftPalm && leftGrab)
        {
            currentLeftPalmPosition = leftPalm.transform.position;
            xDistance = currentLeftPalmPosition.x - lastLeftPalmPosition.x;

            speed = xDistance / Time.deltaTime;

            if (xDistance < -0.005 && yDistance < 0.004 && leftGrab)
            {
                m_Rigidbody.angularDrag = 0.3f;
                m_Rigidbody.inertiaTensor = new Vector3(0, 0, 1);
                m_Rigidbody.AddRelativeTorque(0, 0, speed/7, ForceMode.VelocityChange);
            }

            if (xDistance > 0.005 && yDistance < 0.004 && leftGrab)
            {
                m_Rigidbody.angularDrag = 0.3f;
                m_Rigidbody.inertiaTensor = new Vector3(0, 0, 1);
                m_Rigidbody.AddRelativeTorque(0, 0, speed/7, ForceMode.VelocityChange);
            }
        }*/

        if (rightPalm)
        {
            lastRightPalmPosition = rightPalm.transform.position;
        }

        if (leftPalm)
        {
            lastLeftPalmPosition = leftPalm.transform.position;
        }
    }

    private IEnumerator StopRotation()
    {
        yield return new WaitForSeconds(2f);
        m_Rigidbody.angularVelocity = Vector3.zero;
        rotating = false;
        rightGrab = false;
        leftGrab = false;
    }

    private IEnumerator FindRightPalm()
    {
        var rightHandVisual = XrReferences.RightHandVisual;
        if (rightHandVisual != null)
        {
            while (rightPalm == null)
            {
                yield return null;
                rightPalm = rightHandVisual.Find("R_wrist/R_RingMetacarpal");
            }
        }
        else
        {
            GameObject palmGameObject = null;
            while (palmGameObject == null)
            {
                yield return null;
                palmGameObject = GameObject.Find("R_RingMetacarpal");
            }

            rightPalm = palmGameObject.transform;
        }

        lastRightPalmPosition = rightPalm.position;
    }

    private IEnumerator FindLeftPalm()
    {
        var leftHandVisual = XrReferences.LeftHandVisual;
        if (leftHandVisual != null)
        {
            while (leftPalm == null)
            {
                yield return null;
                leftPalm = leftHandVisual.Find("L_wrist/L_RingMetacarpal");
            }
        }
        else
        {
            GameObject palmGameObject = null;
            while (palmGameObject == null)
            {
                yield return null;
                palmGameObject = GameObject.Find("L_RingMetacarpal");
            }

            leftPalm = palmGameObject.transform;
        }

        lastLeftPalmPosition = leftPalm.position;
    }

    /*public void RotateGrab()
    {
        if (rotating)
        {
            rotating = false;
            m_Rigidbody.angularVelocity = Vector3.zero;
            rightGrab = false;
            leftGrab = false;
        } else
        {
            rotating = true;
            m_Rigidbody.angularDrag = 0;
        }
    }*/

    public void RightGrabEnabled()
    {
        rightGrab = true;
    }

    public void LeftGrabEnabled()
    {
        leftGrab = true;
    }

    public void RightGrabDisabled()
    {
        rightGrab = false;
    }

    public void LeftGrabDisabled()
    {
        leftGrab = false;
    }
}