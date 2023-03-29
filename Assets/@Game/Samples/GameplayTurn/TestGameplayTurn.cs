using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestGameplayTurn : MonoBehaviour
{
    [SerializeField] private GameplayTurn m_Turn;
    
    [SerializeField] private PatternRecognitionPhase m_PatternRecognitionPhase;
    [SerializeField] private Transform m_HandPoint;
    [SerializeField] private ActionBasedController m_Controller;
    
    private void Start()
    {
        m_PatternRecognitionPhase.HandPoint = m_HandPoint;
        m_PatternRecognitionPhase.Controller = m_Controller;
        
        StartCoroutine(m_Turn.StartTurn());
    }
}
