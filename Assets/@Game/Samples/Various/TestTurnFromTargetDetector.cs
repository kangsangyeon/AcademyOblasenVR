using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurnFromTargetDetector : MonoBehaviour
{
    [SerializeField] private TurnAwayFromTargetDetector m_Detector;

    private void OnEnable()
    {
        m_Detector.OnBeginOutOfThresholdAngle.AddListener(OnBeginOutOfThresholdAngle);
        m_Detector.OnOutOfThreshold.AddListener(OnOutOfThreshold);
        m_Detector.OnBackInRange.AddListener(OnBackInRange);
    }

    private void OnDisable()
    {
        m_Detector.OnBeginOutOfThresholdAngle.RemoveListener(OnBeginOutOfThresholdAngle);
        m_Detector.OnOutOfThreshold.RemoveListener(OnOutOfThreshold);
        m_Detector.OnBackInRange.RemoveListener(OnBackInRange);
    }

    private void OnBeginOutOfThresholdAngle()
    {
        Debug.Log("OnBeginOutOfThresholdAngle");
    }

    private void OnOutOfThreshold()
    {
        Debug.Log("OnOutOfThreshold");
    }

    private void OnBackInRange()
    {
        Debug.Log("OnBackInRange");
    }
}