using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTransform : MonoBehaviour
{
    public Transform m_Target;

    private void Update()
    {
        if (m_Target == null)
            return;

        Vector3 _targetForward = m_Target.forward;
        _targetForward.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, _targetForward, 0.2f);
    }
}
