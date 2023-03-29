using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class GameplayTurn : MonoBehaviour
{
    [SerializeField] private SelectSkillPhase m_SelectSkillPhase;
    [SerializeField] private PatternRecognitionPhase m_PatternRecognitionPhase;
    [SerializeField] private SpellPhase m_SpellPhase;

    private int m_SkillBaseDamage;
    private float m_PatternDamageMultiplier;
    private int m_SpellExtraDamage;
    
    public SelectSkillPhase GetSelectSkillPhase() => m_SelectSkillPhase;
    public PatternRecognitionPhase GetPatternRecognitionPhase() => m_PatternRecognitionPhase;
    public SpellPhase GetSpellPhase() => m_SpellPhase;

    public int GetSkillBaseDamage() => m_SkillBaseDamage;
    public float GetPatternDamageMultiplier() => m_PatternDamageMultiplier;
    public int GetSpellExtraDamage() => m_SpellExtraDamage;
    

    public IEnumerator StartTurn()
    {
        m_SkillBaseDamage = 0;
        m_PatternDamageMultiplier = 0.0f;
        m_SpellExtraDamage = 0;
        
        Debug.Log("start select skill phase");
        yield return StartCoroutine(m_SelectSkillPhase.StartPhase());
        Debug.Log($"selected element: {m_SelectSkillPhase.GetSelectedElement().ToString()}, selected skill: {m_SelectSkillPhase.GetSelectedSkill().ToString()}");

        Debug.Log("start pattern recognition phase");
        var _skill = m_SelectSkillPhase.GetSelectedSkillInfo();
        yield return StartCoroutine(m_PatternRecognitionPhase.StartPhase(_skill.Pattern));
        Debug.Log($"max progress: {m_PatternRecognitionPhase.GetMaxProgress()}");

        Debug.Log("start spell phase");
        yield return StartCoroutine(m_SpellPhase.StartPhase());
        Debug.Log($"highest level: {m_SpellPhase.GetHighestLevel()}");

        m_SkillBaseDamage = _skill.Damage;
        m_PatternDamageMultiplier = m_PatternRecognitionPhase.GetDamageMultiplier();
        m_SpellExtraDamage = m_SpellPhase.GetExtraDamage();

       // m_SelectSkillPhase.OnElementChanged.Invoke(EElementType.None);

        // m_Damage = (int)(_skill.Damage * m_PatternRecognitionPhase.GetDamageMultiplier() + m_SpellPhase.GetExtraDamage());
        // Debug.Log($"damage: {_skill.Damage}({_skill.SkillName}'s base damage)" +
        //           $" * {m_PatternRecognitionPhase.GetDamageMultiplier()}(progress {m_PatternRecognitionPhase.GetMaxProgress()})" +
        //           $" + {m_SpellPhase.GetExtraDamage()}(highest level {m_SpellPhase.GetHighestLevel()}) = {m_Damage}");
    }
}