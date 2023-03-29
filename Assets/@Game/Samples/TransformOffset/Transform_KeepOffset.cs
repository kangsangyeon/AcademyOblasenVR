using System;
using System.Collections;
using UnityEngine;

public class Transform_KeepOffset : MonoBehaviour
{
    private enum State
    {
        Idle,
        Wait,
        Move
    }

    public Transform m_Target;
    public bool m_bSetOffsetAtStart;
    public Vector3 m_OffsetFromTarget;
    public float m_Delay = 3.0f;
    public float m_SlerpSpeed = 5.0f;

    private Vector3 m_DesiredPosition;
    private bool m_bActivate;
    private State m_State;
    private float m_ActivateTime;

    private void Start()
    {
        if (m_bSetOffsetAtStart)
        {
            m_OffsetFromTarget = transform.position - m_Target.position;
        }

        m_DesiredPosition = transform.position + m_OffsetFromTarget;
    }

    private void Update()
    {
        Vector3 _offsetThisFrame = transform.position - m_Target.position;
        float _distanceBetweenOffset = Vector3.Distance(_offsetThisFrame, m_OffsetFromTarget);

        if (_distanceBetweenOffset <= 0.001f
            && m_State == State.Move)
        {
            // 이동을 모두 마쳤을 때 한 번 실행됩니다.

            m_State = State.Idle;
            m_ActivateTime = 0.0f;
        }

        if (_distanceBetweenOffset > 0.001f
            && m_State == State.Idle)
        {
            // 오브젝트가 이동한 것을 감지합니다.
            // 이동했을 때 한 번 실행됩니다.

            m_State = State.Wait;
            m_ActivateTime = Time.time;
        }

        float _elapsedTime = Time.time - m_ActivateTime;
        if (_elapsedTime >= m_Delay
            && m_State == State.Wait)
        {
            m_State = State.Move;
            m_ActivateTime = 0.0f;

            m_DesiredPosition = m_Target.position + m_OffsetFromTarget;
        }

        if (m_State == State.Move)
        {
            transform.position = Vector3.Lerp(transform.position, m_DesiredPosition, Time.deltaTime * m_SlerpSpeed);
        }
    }
}