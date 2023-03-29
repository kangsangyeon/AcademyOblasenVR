using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestVRSpellPhase : MonoBehaviour
{
    [SerializeField] private SpellPhase m_Phase;
    [SerializeField] private TextMeshProUGUI m_Text_Phase;
    [SerializeField] private TextMeshProUGUI m_Text_Multiplier;
    [SerializeField] private TextMeshProUGUI m_Text_HighestLevel;

    private IEnumerator Start()
    {
        m_Phase.OnEndWaitingStep.AddListener(()=>m_Text_Phase.text = "listen spell..");
        m_Phase.OnEndSpellStep.AddListener(() =>
        {
            m_Text_Phase.text = "end";
            m_Text_Multiplier.text = m_Phase.GetExtraDamage().ToString();
            m_Text_HighestLevel.text = m_Phase.GetHighestLevel().ToString();
        });

        yield return new WaitForSeconds(3);
        yield return StartCoroutine(m_Phase.StartPhase());
    }
}