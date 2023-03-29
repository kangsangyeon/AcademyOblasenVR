using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnableQuickSceneChanger : MonoBehaviour
{
    [SerializeField] private InputActionReference m_ShowActionRef;
    [SerializeField] private GameObject m_QuickSceneChanger;

    private void OnEnable()
    {
        if (m_ShowActionRef == null || m_QuickSceneChanger == null)
            return;

        m_ShowActionRef.action.performed += ActivateQuickSceneChanger;
        m_ShowActionRef.action.canceled += DeactivateQuickSceneChanger;
    }

    private void OnDisable()
    {
        if (m_ShowActionRef == null || m_QuickSceneChanger == null)
            return;

        m_ShowActionRef.action.performed -= ActivateQuickSceneChanger;
        m_ShowActionRef.action.canceled -= DeactivateQuickSceneChanger;
    }

    private void ActivateQuickSceneChanger(InputAction.CallbackContext _context)
    {
        m_QuickSceneChanger.SetActive(true);
    }

    private void DeactivateQuickSceneChanger(InputAction.CallbackContext _context)
    {
        m_QuickSceneChanger.SetActive(false);
    }
}