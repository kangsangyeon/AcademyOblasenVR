using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform m_Target;

    private void Update()
    {
        if (m_Target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, m_Target.position, 0.2f);
    }
}
