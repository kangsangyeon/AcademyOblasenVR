using System.Linq;
using UnityEngine;

public class SkillInfoFinder : MonoBehaviour
{
    [SerializeField] private Asset_Skill[] m_SkillArray;

    public Asset_Skill Find(EElementType _elementType, ESkillType _skillType) => m_SkillArray.First(s => s.ElementType == _elementType && s.SkillType == _skillType);
}