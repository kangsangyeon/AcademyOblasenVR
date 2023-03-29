using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WitchEffects : MonoBehaviour
{
    [SerializeField] public GameObject TutoBot;

    private Transform mCamera;

    #region Enum

    enum Elements
    {
        eIDLE,
        eFire,
        eWater,
        eLight,
        eDark,
    }

    enum Skills
    {
        eIDLE,

        eFire_1 = 11,
        eFire_2,

        eWater_1 = 21,
        eWater_2,

        eLight_1 = 31,
        eLight_2,

        eDark_1 = 41,
        eDark_2,
    }

    #endregion

    #region GameObjects

    #region Element Object

    private GameObject ElementSet;

    private GameObject elementFire;
    private GameObject elementWater;
    private GameObject elementLight;
    private GameObject elementDark;
    #endregion

    #region Skill UI Object

    private GameObject SkillUISet;

    #region Fire
    private GameObject UI_FireSkill_1;
    private GameObject UI_FireSkill_2;
    #endregion

    #region Water
    private GameObject UI_WaterSkill_1;
    private GameObject UI_WaterSkill_2;

    #endregion

    #region Light
    private GameObject UI_LightSkill_1;
    private GameObject UI_LightSkill_2;

    #endregion

    #region Dark
    private GameObject UI_DarkSkill_1;
    private GameObject UI_DarkSkill_2;

    #endregion

    #endregion

    #endregion

    #region Private Fields

    private int mHP;
    private int mMP;

    private int enemyHP;
    private int enemyMP;

    private float skillTime;

    private Elements selectedElement;
    private Skills selectedSkill;

    // 정확도
    private float mPercentage;

    // 볼륨
    private int mVolume;

    // 턴 시간
    private float mTimes;

    private Vector3 enemyPos;
    private Vector3 effectPos;

    #endregion

    #region Public Fields

    public bool isElementSelected;

    #endregion

    #region Private Methods

    private void FindGameObjects()
    {
        ElementSet = GameObject.Find("Element UI");
        SkillUISet = GameObject.Find("Skill UI");

        elementFire = GameObject.Find("Fire Element");
        elementWater = GameObject.Find("Water Element");
        elementLight = GameObject.Find("Light Element");
        elementDark = GameObject.Find("Dark Element");

        UI_FireSkill_1 = GameObject.Find("Fire Skill_1");
        UI_FireSkill_2 = GameObject.Find("Fire Skill_2");

        UI_WaterSkill_1 = GameObject.Find("Water Skill_1");
        UI_WaterSkill_2 = GameObject.Find("Water Skill_2");

        UI_LightSkill_1 = GameObject.Find("Light Skill_1");
        UI_LightSkill_2 = GameObject.Find("Light Skill_2");

        UI_DarkSkill_1 = GameObject.Find("Dark Skill_1");
        UI_DarkSkill_2 = GameObject.Find("Dark Skill_2");
    }



    /// <summary>
    /// 초기화
    /// </summary>
    private void Initailize()
    {
        mCamera = transform.GetChild(0).transform.GetChild(0);

        mHP = 100;
        mMP = 100;

        selectedElement = Elements.eIDLE;
        selectedSkill = Skills.eIDLE;

        mPercentage = 0f;
        mTimes = 0;
        mVolume = 0;

        skillTime = 0;

        isElementSelected = false;

        enemyPos = TutoBot.transform.position;

        FindGameObjects();

        SetElementGameObject(false);
        SetSkillGameObject(false, selectedElement);
        SetSkillEffect(false, selectedSkill);

        StartCoroutine(MyTurn());
    }

    private void ResetAll()
    {
        selectedElement = Elements.eIDLE;
        selectedSkill = Skills.eIDLE;
    }

    /// <summary>
    /// Element GameObject 전체의 SetActive
    /// </summary>
    /// <param name="set"><bool 값>
    private void SetElementGameObject(bool set)
    {
        if (set == true)
        {
            Vector3 _camera = mCamera.position;
            _camera.y = enemyPos.y;
            Vector3 direction = (enemyPos - _camera).normalized;
            ElementSet.transform.position = mCamera.position + direction * 1;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            ElementSet.transform.rotation = lookRot;
        }

        elementFire.SetActive(set);
        elementWater.SetActive(set);
        elementLight.SetActive(set);
        elementDark.SetActive(set);
    }

    /// <summary>
    /// Skill Object들의 SetActive 조절
    /// </summary>
    /// <param name="set"><true or false>
    /// <param name="element"><enum Element>
    private void SetSkillGameObject(bool set, Elements element)
    {
        if (set == true)
        {
            Vector3 _camera = mCamera.position;
            _camera.y = enemyPos.y;
            Vector3 direction = (enemyPos - _camera).normalized;
            SkillUISet.transform.position = mCamera.position + direction * 1;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            SkillUISet.transform.rotation = lookRot;
        }

        switch (element)
        {
            case Elements.eFire:
                {
                    UI_FireSkill_1.SetActive(set);
                    UI_FireSkill_2.SetActive(set);
                }
                break;

            case Elements.eWater:
                {
                    UI_WaterSkill_1.SetActive(set);
                    UI_WaterSkill_2.SetActive(set);
                }
                break;

            case Elements.eLight:
                {
                    UI_LightSkill_1.SetActive(set);
                    UI_LightSkill_2.SetActive(set);
                }
                break;

            case Elements.eDark:
                {
                    UI_DarkSkill_1.SetActive(set);
                    UI_DarkSkill_2.SetActive(set);
                }
                break;

            case Elements.eIDLE:
                {
                    UI_FireSkill_1.SetActive(set);
                    UI_FireSkill_2.SetActive(set);

                    UI_WaterSkill_1.SetActive(set);
                    UI_WaterSkill_2.SetActive(set);

                    UI_LightSkill_1.SetActive(set);
                    UI_LightSkill_2.SetActive(set);

                    UI_DarkSkill_1.SetActive(set);
                    UI_DarkSkill_2.SetActive(set);
                }
                break;

            default:
                break;
        }

    }

    private void SetSkillEffect(bool set, Skills skill)
    {
        if (set == true)
        {
            effectPos = mCamera.position + mCamera.forward * 1;

            enemyPos.y = mCamera.position.y;

            //SkillUISet.transform.position = mCamera.position + mCamera.forward * 1;
            //Vector3 direction = (mCamera.position - SkillUISet.transform.position).normalized;
            //Quaternion lookRot = Quaternion.LookRotation(direction);
            //SkillUISet.transform.rotation = lookRot;
        }
        switch (skill)
        {
            case Skills.eFire_1:
                {
                    WitchEffectManager.instance.MeteorEffect(effectPos,enemyPos);
                }
                break;

            case Skills.eFire_2:
                {
                    WitchEffectManager.instance.MagmaImpactEffect(effectPos, enemyPos);

                }
                break;

            case Skills.eWater_1:
                {
                    WitchEffectManager.instance.HealingWaveEffect(effectPos);
                }
                break;

            case Skills.eWater_2:
                {
                    WitchEffectManager.instance.HealingWaveEffect(effectPos);
                }
                break;

            case Skills.eDark_1:
                {
                    WitchEffectManager.instance.CircleOfCurseEffect(enemyPos);
                }
                break;

            case Skills.eDark_2:
                {
                    WitchEffectManager.instance.DarkMistEffect(enemyPos);
                }
                break;

            case Skills.eLight_1:
                {
                    WitchEffectManager.instance.JudgementSwordEffect(effectPos, enemyPos);
                }
                break;

            case Skills.eLight_2:
                {
                    WitchEffectManager.instance.LighteningStrikeEffect(enemyPos);
                }
                break;

            case Skills.eIDLE:
                {
                }
                break;

            default:
                {
                    Debug.Log("SetSkillEffect default case");
                }
                break;
        }
    }

    private void ShootMagic(Skills skill)
    {
        SetSkillEffect(true, skill);
    }
    #endregion


    #region Public Methods

 


    public void ElementSelect(int element)
    {
        selectedElement = (Elements)element;
    }

    public void SkillSelected(int skill)
    {
        selectedSkill = (Skills)skill;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Initailize();
    }

    // Update is called once per frame
    void Update()
    {
        mCamera = transform.GetChild(0).transform.GetChild(0);
    }


    IEnumerator MyTurn()
    {
        while (true)
        {
            yield return StartCoroutine(ElementSelectPhase());

            yield return StartCoroutine(SkillSelectPhase());

            ShootMagic(selectedSkill);
            ResetAll();
            yield return new WaitForSeconds(skillTime);
        }
    }

    IEnumerator ElementSelectPhase()
    {
        SetElementGameObject(true);
        yield return null;

        while (selectedElement == Elements.eIDLE)
        {
            yield return null;
        }

        SetElementGameObject(false);
    }

    IEnumerator SkillSelectPhase()
    {
        SetSkillGameObject(true, selectedElement);

        while (selectedSkill == Skills.eIDLE)
        {
            yield return null;
        }

        SetSkillGameObject(false, selectedElement);
    }
    public void InitCoroutine()
    {
        Debug.Log("Start Coroutine");
        CapsuleCollider tmp = TutoBot.GetComponent<CapsuleCollider>();
        StartCoroutine(MyTurn());
        tmp.enabled = false;
    }

}
