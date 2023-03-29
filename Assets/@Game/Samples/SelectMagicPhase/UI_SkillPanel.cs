using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillPanel : MonoBehaviour
{
    [SerializeField] private Asset_Skill m_Asset_Skill;

    [SerializeField] private Image m_Image_Skill;
    [SerializeField] private Image m_Image_Description;
    [SerializeField] private TMP_Text m_Damage_Text;
    [SerializeField] private TMP_Text m_Mana_Text;
    [SerializeField] private Button m_Button;

    public void SetSkill(Asset_Skill _skill)
    {
        m_Asset_Skill = _skill;
        OnValidate();
    }

    public void SetInteractable(bool _bInteractable)
    {
        m_Button.interactable = _bInteractable;

        if (_bInteractable == false)
        {
            Color _tmp = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            m_Image_Skill.color = _tmp;
            m_Image_Description.color = _tmp;
        }

        else
        {
            m_Image_Skill.color = Color.white;
            m_Image_Description.color = Color.white;
        }
    }

    private void Start()
    {
        OnValidate();
        m_Image_Description.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (m_Asset_Skill == null)
            return;

        m_Image_Skill.sprite = m_Asset_Skill.SkillSprite;
        m_Image_Description.sprite = m_Asset_Skill.DescriptionSprite;

        m_Damage_Text.text = Convert.ToString(m_Asset_Skill.Damage);
        m_Mana_Text.text = Convert.ToString(m_Asset_Skill.UsedMP);

        if (m_Asset_Skill.ElementType == EElementType.Water)
        {
            if (m_Asset_Skill.SkillType == ESkillType.First)
                m_Damage_Text.text = Convert.ToString(m_Asset_Skill.HealHP);

            else
                m_Mana_Text.text = Convert.ToString(m_Asset_Skill.UsedMP);
        }
    }
}
