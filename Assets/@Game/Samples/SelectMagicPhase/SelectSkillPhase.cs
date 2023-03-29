using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SelectSkillPhase : MonoBehaviour
{
    [SerializeField] private Asset_Skill[] m_SkillArr;
    [SerializeField] private Transform m_SelectElementHolder;
    [SerializeField] private Transform[] m_Elements;
    [SerializeField] private Transform m_SelectSkillHolder;
    [SerializeField] private UI_SkillPanel m_UI_FirstSkillPanel;
    [SerializeField] private UI_SkillPanel m_UI_SecondSkillPanel;

    private EElementType m_SelectedElement;
    private ESkillType m_SelectedSkill;
    private Asset_Skill m_SelectedSkillInfo;

    private int m_CurrentHP;
    private int m_CurrentMP;

    public UnityEvent OnStartPhase;
    public UnityEvent OnEndPhase;
    public UnityEvent<EElementType> OnSelectElementType;
    public UnityEvent<ESkillType> OnSelectSkillType;
    public UnityEvent OnCancelElementSelection;

    public UnityEvent<EElementType> OnElementChanged;
    public Transform GetElement(int _index) => m_Elements[_index];
    public EElementType GetSelectedElement() => m_SelectedElement;
    public ESkillType GetSelectedSkill() => m_SelectedSkill;
    public Asset_Skill GetSelectedSkillInfo() => m_SelectedSkillInfo;
    public void SetCurrentHPMP(int _hp, int _mp)
    {
        m_CurrentHP = _hp;
        m_CurrentMP = _mp;
    }

    private void OnEnable()
    {
        m_SelectElementHolder.gameObject.SetActive(false);
        SetActiveSkillHolder(false);
    }

    public IEnumerator StartPhase()
    {
        m_SelectedElement = EElementType.None;
        m_SelectedSkill = ESkillType.None;

        OnElementChanged.Invoke(m_SelectedElement);

        m_SelectElementHolder.gameObject.SetActive(true);

        SetActiveSkillHolder(false);

        OnStartPhase.Invoke();

        while (true)
        {
            if (m_SelectedSkill != ESkillType.None)
            {
                // 원소를 고르고 스킬까지 선택한 뒤
                // 다음 phase로 전환합니다.
                break;
            }

            yield return null;
        }

        OnEndPhase.Invoke();
    }

    public void OnClickElementProxy(int _type)
    {
        OnClickElement((EElementType)_type);
        OnElementChanged.Invoke((EElementType)_type);
    }

    public void OnClickSkillProxy(int _type)
    {
        OnClickSkill((ESkillType)_type);
    }

    /// <summary>
    /// 원소를 선택했을 때 호출되는 콜백입니다.
    /// </summary>
    public void OnClickElement(EElementType _type)
    {
        m_SelectedElement = _type;
        m_SelectElementHolder.gameObject.SetActive(false);
        OnSelectElementType.Invoke(_type);
        SetActiveSkillHolder(true);
    }

    /// <summary>
    /// 스킬을 선택했을 때 호출되는 콜백입니다.
    /// </summary>
    public void OnClickSkill(ESkillType _type)
    {
        m_SelectedSkill = _type;
        SetActiveSkillHolder(false);

        m_SelectedSkillInfo = m_SkillArr.First(s => s.ElementType == m_SelectedElement && s.SkillType == m_SelectedSkill);

        OnSelectSkillType.Invoke(_type);
    }

    /// <summary>
    /// 뒤로가기 버튼을 선택했을 때 호출되는 콜백입니다.
    /// </summary>
    public void OnClickBack()
    {
        SetActiveSkillHolder(false);
        m_SelectElementHolder.gameObject.SetActive(true);
        m_SelectedElement = EElementType.None;

        OnElementChanged.Invoke(m_SelectedElement);
        OnCancelElementSelection.Invoke();
    }

    /// <summary>
    /// 스킬 선택창의 활성화 여부를 토글하는 함수입니다.
    /// 활성화시, 내가 선택한 원소의 타입에 대응하는 스킬 UI의 이미지로 교체한 뒤 활성화합니다.
    /// </summary>
    private void SetActiveSkillHolder(bool _active)
    {
        if (_active == false)
        {
            m_SelectSkillHolder.gameObject.SetActive(false);
            return;
        }

        var _firstSkill = m_SkillArr.First(s => s.ElementType == m_SelectedElement && s.SkillType == ESkillType.First);
        var _secondSkill = m_SkillArr.First(s => s.ElementType == m_SelectedElement && s.SkillType == ESkillType.Second);
        
        m_UI_FirstSkillPanel.SetSkill(_firstSkill);
        m_UI_FirstSkillPanel.SetInteractable(m_CurrentMP >= _firstSkill.UsedMP);

        m_UI_SecondSkillPanel.SetSkill(_secondSkill);
        m_UI_SecondSkillPanel.SetInteractable(m_CurrentMP >= _secondSkill.UsedMP);

        m_SelectSkillHolder.gameObject.SetActive(true);
    }
}