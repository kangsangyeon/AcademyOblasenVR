using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTransform : MonoBehaviour
{
    public Transform m_Target;

    private float m_StartTime;
    private float m_Duration;
    private float m_Strength;

    private int m_Direction = 1;
    private Coroutine m_ShakeCoroutine;

    public void DoShake(float _duration, float _strength)
    {
        m_StartTime = Time.time;
        m_Duration = _duration;
        m_Strength = _strength;
        m_ShakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    public void StopShake()
    {
        if (m_ShakeCoroutine == null)
            return;

        StopCoroutine(m_ShakeCoroutine);
        m_ShakeCoroutine = null;
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            float _elapsedTime = Time.time - m_StartTime;
            if (_elapsedTime >= m_Duration)
                break;

            float _delta = _elapsedTime / m_Duration;

            Vector3 _desiredPosition = Vector3.right * m_Direction * Mathf.Lerp(m_Strength, 0, _delta);
            m_Target.localPosition = Vector3.Lerp(m_Target.localPosition, _desiredPosition, 0.1f);
            m_Direction = m_Direction * -1;

            yield return null;
        }

        m_Target.localPosition = Vector3.zero;
        m_ShakeCoroutine = null;
    }

    [ContextMenu("Test DoShake")]
    private void TestDoShake()
    {
        DoShake(1, 3.0f);
    }
}