using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SpellPhase : MonoBehaviour
{
    [System.Serializable]
    struct LevelStep
    {
        public float requiredLevel;
        public int extraDamage;
    }

    [SerializeField] private InputActionReference m_Asset_ActivateActionRef;
    [SerializeField] private MicInputUser m_MicInputUser;
    [SerializeField] private float m_WaitingDuration = 1.0f;
    [SerializeField] private float m_Duration = 4.0f;
    [SerializeField] private float m_LeastDuration = 0.5f;
    [SerializeField] private LevelStep[] m_LevelStepArr;

    public UnityEvent OnStartPhase;
    public UnityEvent OnEndPhase;
    public UnityEvent OnEndWaitingStep;
    public UnityEvent OnEndSpellStep;

    private float m_HighestLevel;
    private float m_CurrentLevel;
    private float m_PhaseLeftoverTime;
    private int m_ExtraDamage;

    public float GetDuration() => m_Duration;
    public float GetHighestLevel() => m_HighestLevel;
    public float GetCurrentLevel() => m_CurrentLevel;
    public float GetPhaseLeftoverTime() => m_PhaseLeftoverTime;
    public int GetExtraDamage() => m_ExtraDamage;

    public IEnumerator StartPhase()
    {
        m_HighestLevel = 0.0f;
        m_ExtraDamage = 0;

        OnStartPhase.Invoke();

        yield return new WaitForSeconds(m_WaitingDuration);
        OnEndWaitingStep.Invoke();

        yield return StartCoroutine(SpellStep());
        OnEndSpellStep.Invoke();

        OnEndPhase.Invoke();
    }

    private IEnumerator SpellStep()
    {
        m_MicInputUser.enabled = true;

        float _startTime = Time.time;
        while (true)
        {
            m_CurrentLevel = m_MicInputUser.GetCurrentLevel();
            m_HighestLevel = m_MicInputUser.GetHighestLevel();

            float _elapsedTime = Time.time - _startTime;
            m_PhaseLeftoverTime = m_Duration - _elapsedTime;

            if (_elapsedTime <= m_LeastDuration)
            {
                // 최소 시간보다 적은 시간이 지난 경우, 무조건 다음 phase로 넘어가지 않습니다.
                yield return null;
                continue;
            }

            if (_elapsedTime >= m_Duration)
            {
                // phase의 시간이 모두 경과된 후, phase를 빠져나갑니다.
                m_PhaseLeftoverTime = 0.0f;
                break;
            }

            if (m_Asset_ActivateActionRef.action.WasReleasedThisFrame())
                break;

            yield return null;
        }

        m_MicInputUser.enabled = false;

        // 마이크 입력 값을 차등하여 추가 데미지를 결정합니다.
        m_ExtraDamage = m_LevelStepArr.First(ls => ls.requiredLevel <= m_HighestLevel).extraDamage;
    }

    private void OnValidate()
    {
        Array.Sort(m_LevelStepArr, (first, second) => first.requiredLevel < second.requiredLevel ? 1 : -1);
    }
}