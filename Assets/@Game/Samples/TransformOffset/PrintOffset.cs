using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintOffset : MonoBehaviour
{
    [SerializeField] private Transform m_Target;

    private IEnumerator Start()
    {
        while (true)
        {
            if (m_Target == null)
            {
                yield return null;
                continue;
            }

            Vector3 _offset = m_Target.position - transform.position;
            Debug.Log($"offset: {_offset}");

            yield return new WaitForSeconds(2.0f);
        }
    }
}