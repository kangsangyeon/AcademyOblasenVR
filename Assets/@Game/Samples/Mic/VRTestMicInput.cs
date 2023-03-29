using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRTestMicInput : MonoBehaviour
{
    [SerializeField] private InputActionReference m_RecActionRef;
    [SerializeField] private MicInputUser m_MicUser;
    [SerializeField] private TextMeshProUGUI m_Text_HighestLevel;

    private void Start()
    {
        m_MicUser.OnEndAction.AddListener(OnMicActionEnd);
        m_RecActionRef.action.performed += EnableMicUser;
        m_RecActionRef.action.canceled += DisableMicUser;
    }

    private void OnDisable()
    {
        m_MicUser.OnEndAction.RemoveListener(OnMicActionEnd);
        m_RecActionRef.action.performed -= EnableMicUser;
        m_RecActionRef.action.canceled -= DisableMicUser;
    }

    private void OnMicActionEnd(float _level)
    {
        m_Text_HighestLevel.text = _level.ToString();
        Debug.Log(_level);
    }

    private void EnableMicUser(InputAction.CallbackContext _context)
    {
        m_MicUser.enabled = true;
    }

    private void DisableMicUser(InputAction.CallbackContext _context)
    {
        m_MicUser.enabled = false;
    }

}
