using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRecognizerUser : MonoBehaviour
{
    [SerializeField] private PatternRecognizer m_Recognizer;
    [SerializeField] private Transform m_RecognizerHand;
    [SerializeField] private LayerMask m_LayerMask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            m_Recognizer.enabled = true;
        else if (Input.GetMouseButtonUp(0))
            m_Recognizer.enabled = false;

        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;
        Physics.Raycast(_ray, out _hit, 100.0f, m_LayerMask);

        if (_hit.collider)
        {
            m_RecognizerHand.position = _hit.point;
        }
    }
}
