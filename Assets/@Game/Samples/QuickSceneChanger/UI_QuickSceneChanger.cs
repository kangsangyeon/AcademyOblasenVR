using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_QuickSceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_Asset_ButtonPrefab;
    [SerializeField] private Transform m_ButtonGrid;
    [SerializeField] private string[] m_SceneNames;

    private void Start()
    {
        for(int i = 0; i < m_ButtonGrid.childCount; ++i)
            Destroy(m_ButtonGrid.GetChild(i));
        
        foreach (var _scene in m_SceneNames)
        {
            var _go = GameObject.Instantiate(m_Asset_ButtonPrefab, m_ButtonGrid);
            _go.GetComponent<UI_Button_ChangeScene>().SceneName = _scene;
        }
    }
}
