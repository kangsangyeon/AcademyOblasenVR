using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestSelectSkillPhase : MonoBehaviour
{
    [SerializeField] private SelectSkillPhase m_Phase;
    [SerializeField] private TextMeshProUGUI m_Text_ElementType;
    [SerializeField] private TextMeshProUGUI m_Text_SkillType;
    [SerializeField] private TextMeshProUGUI m_Text_SkillName;

    private IEnumerator Start()
    {
        m_Phase.OnSelectElementType.AddListener(t =>
        {
            m_Text_ElementType.text = m_Phase.GetSelectedElement().ToString();
            m_Text_SkillType.text = m_Phase.GetSelectedSkill().ToString();
        });
        
        m_Phase.OnSelectSkillType.AddListener(t =>
        {
            m_Text_ElementType.text = m_Phase.GetSelectedElement().ToString();
            m_Text_SkillType.text = m_Phase.GetSelectedSkill().ToString();
            m_Text_SkillName.text = m_Phase.GetSelectedSkillInfo().SkillName;
        });

        yield return StartCoroutine(m_Phase.StartPhase());
    }
}