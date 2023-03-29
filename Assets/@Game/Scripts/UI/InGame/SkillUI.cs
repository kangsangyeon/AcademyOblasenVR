using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    #region Private Fields
    private Button mButton;
    private int mSkill;
    #endregion

    #region Private Methods
    private void OnButtonClicked()
    {
        Debug.Log("Element Selected");
        var _components = GameObject.FindObjectsOfType<PlayerManagerCoroutine>();
        foreach (var _component in _components)
        {
            var _photonView = _component.gameObject.GetComponent<Photon.Pun.PhotonView>();
            if (_photonView.IsMine)
            {
                _component.SetSkill(mSkill);
                break;
            }
        }
    }

    private void FindName()
    {
        switch (gameObject.transform.parent.name)
        {
            case "Fire Skill_1":
                {
                    mSkill = 11;
                }
                break;

            case "Fire Skill_2":
                {
                    mSkill = 12;
                }
                break;

            case "Water Skill_1":
                {
                    mSkill = 21;
                }
                break;

            case "Water Skill_2":
                {
                    mSkill = 22;
                }
                break;

            case "Light Skill_1":
                {
                    mSkill = 31;
                }
                break;

            case "Light Skill_2":
                {
                    mSkill = 32;
                }
                break;

            case "Dark Skill_1":
                {
                    mSkill = 41;
                }
                break;

            case "Dark Skill_2":
                {
                    mSkill = 42;
                }
                break;

            default:
                mSkill = 99;
                break;
        }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mButton = GetComponent<Button>();
        FindName();
        mButton.onClick.AddListener(OnButtonClicked);
    }
}

/* 
asdfs (int skill, int dmg, int mp, bool isEnable)
{
    
}
*/