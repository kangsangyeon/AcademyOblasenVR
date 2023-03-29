using UnityEngine;
using UnityEngine.Events;

public class TurnAwayFromTargetDetector : MonoBehaviour
{
    public UnityEvent OnOutOfThreshold;
    public UnityEvent OnBackInRange;
    public UnityEvent OnBeginOutOfThresholdAngle;

    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_ThresholdAngle = 60.0f;
    [SerializeField] private float m_ThresholdTime = 2.0f;

    private float m_StartTime = -1.0f;
    private bool m_bInvoked = false;
    private bool m_bOutOfThresholdPrevFrame = false;

    private void Update()
    {
        var _targetForwardXZ = m_Target.forward;
        _targetForwardXZ.y = 0.0f;
        _targetForwardXZ.Normalize();

        var _myForwardXZ = transform.forward;
        _myForwardXZ.y = 0.0f;
        _myForwardXZ.Normalize();

        var _cosTheta = Vector3.Dot(_targetForwardXZ, _myForwardXZ);
        var _angle = Mathf.Acos(_cosTheta) * Mathf.Rad2Deg;

        bool bOutOfThreshold = false;

        if (_angle >= m_ThresholdAngle)
        {
            bOutOfThreshold = true;

            if (m_StartTime < 0)
            {
                m_StartTime = Time.time;
                OnBeginOutOfThresholdAngle.Invoke();
            }

            var _elapsedTime = Time.time - m_StartTime;
            if (_elapsedTime >= m_ThresholdTime
                && m_bInvoked == false)
            {
                m_bInvoked = true;
                OnOutOfThreshold.Invoke();
            }
        }
        
        if(bOutOfThreshold == false && m_bOutOfThresholdPrevFrame == true)
        {
            m_StartTime = -1.0f;
            m_bInvoked = false;
            OnBackInRange.Invoke();
        }

        m_bOutOfThresholdPrevFrame = bOutOfThreshold;
    }
}