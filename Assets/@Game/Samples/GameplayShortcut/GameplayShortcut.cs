using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayShortcut : MonoBehaviour
{
    [SerializeField] private Transform m_ShortcutUIHolder;
    [SerializeField] private Transform m_DebugUIHolder;
    [SerializeField] private GameplayScene m_Gameplay;
    [SerializeField] private InputActionReference[] m_ShortcutActionRefs;
    [SerializeField] private float m_RequiredHoldDuration = 2.0f;

    private bool m_bAllTriggeredPrevFrame;
    private float m_AllTriggeredStartTime;
    private bool m_bActivated;

    public void OnClickToggleDebugUI()
    {
        m_DebugUIHolder.gameObject.SetActive(m_DebugUIHolder.gameObject.activeSelf);
    }

    public void OnClickSetMyHealthToZero()
    {
        m_Gameplay.GetMyCharacter().AddHP(m_Gameplay.GetMyCharacter().GetHP() * -1);
        m_Gameplay.GetMyCharacter().MSG_C2S_PLAYER_STATE();
    }

    public void OnClickGotoCreditScene()
    {
        SceneManager.LoadScene("Credit");
    }
    
    private void Start()
    {
        m_ShortcutUIHolder.gameObject.SetActive(false);
    }

    private void Update()
    {
        bool _bAnyNotTriggerred = m_ShortcutActionRefs.Any(_actionRef => _actionRef.action.inProgress == false);
        bool _bAllTriggered = !_bAnyNotTriggerred;

        if (_bAllTriggered)
        {
            if (m_bAllTriggeredPrevFrame == false)
            {
                m_AllTriggeredStartTime = Time.time;
            }

            float _elapsedTime = Time.time - m_AllTriggeredStartTime;
            if (_elapsedTime >= m_RequiredHoldDuration
                && m_bActivated == false)
            {
                m_ShortcutUIHolder.gameObject.SetActive(!m_ShortcutUIHolder.gameObject.activeSelf);
                m_bActivated = true;
            }
        }
        else if (m_bAllTriggeredPrevFrame == true)
        {
            m_AllTriggeredStartTime = 0.0f;
            m_bActivated = false;
        }

        m_bAllTriggeredPrevFrame = _bAllTriggered;
    }
}