using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    public Transform m_Target;
    public Vector3 m_Axis = Vector3.up;
    public float m_RotAnglePerSec = 30.0f;

    private void Update()
    {
        if (m_Target == null)
            return;

        m_Target.Rotate(m_Axis, m_RotAnglePerSec * Time.deltaTime);
    }
}
