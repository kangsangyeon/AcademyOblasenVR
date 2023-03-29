using TMPro;
using UnityEngine;

[System.Serializable]
public enum EElementType
{
    None,
    Fire,
    Water,
    Light,
    Dark
}

[System.Serializable]
public enum ESkillType
{
    None,
    First,
    Second
}

[CreateAssetMenu(menuName = "Witch/Skill")]
public class Asset_Skill : ScriptableObject
{
    public EElementType ElementType;
    public ESkillType SkillType;
    public string SkillName;
    public string[] SkillSpell = new string[2];
    public Sprite SkillSprite;
    public Sprite DescriptionSprite;
    public AudioClip SfxMaleVoice;
    public AudioClip SfxGirlVoice;
    public TMP_ColorGradient SpellTextColorGradient;
    public Pattern Pattern;
    
    public int UsedMP = 0;
    public int Damage = 0;
    public int HealMP = 0;
    public int HealHP = 0;

    public bool isAble;
    
    // TODO: 스킬의 부가적인 효과를 만드려고 하지만, 별도의 어빌리티 시스템을 구축하고 싶지 않기 때문에 만든 변수입니다.
    // 거의 대부분의 스킬에 이 값은 0일 것이며, 특정 스킬만 이 속성을 사용합니다.
    public int ExtraDamage;
    public int ExtraHeal;

    public string GetRandomSkillSpell()
    {
        int _index = Random.Range(0, SkillSpell.Length);
        return SkillSpell[_index];
    }
}
