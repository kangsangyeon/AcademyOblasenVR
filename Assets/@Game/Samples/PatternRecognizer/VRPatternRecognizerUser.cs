using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPatternRecognizerUser : MonoBehaviour
{
    [SerializeField] private InputActionReference m_ActivateActionRef;
    [SerializeField] private PatternRecognizer m_Recognizer;

    private void OnEnable()
    {
        m_ActivateActionRef.action.performed += EnableRecognizer;
        m_ActivateActionRef.action.canceled += DisableRecognizer;
    }

    private void OnDisable()
    {
        m_ActivateActionRef.action.performed -= EnableRecognizer;
        m_ActivateActionRef.action.canceled -= DisableRecognizer;
    }

    private void EnableRecognizer(InputAction.CallbackContext _context) => m_Recognizer.enabled = true;
    private void DisableRecognizer(InputAction.CallbackContext _context) => m_Recognizer.enabled = false;
}
