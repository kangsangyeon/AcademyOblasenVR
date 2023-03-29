using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;

public class IntroPlayerManager : MonoBehaviour
{
    [SerializeField] GameObject TutoMob;

    [SerializeField] private Transform mHandPoint;

    [SerializeField] private SkillInfoFinder m_SkillInfoFinder;
    [SerializeField] private PatternRecognitionPhase m_PatternRecognitionPhase;

    [SerializeField] private Transform m_PatternRecognitionTimer;

    [SerializeField] private Image m_Image_PatternRecognitionPhaseLeftoverTimeSlider;

    [SerializeField] private SpellPhase m_SpellPhase;
    [SerializeField] GameObject m_SpellPanel;

    [SerializeField] private TextMeshProUGUI m_Text_CurrentLevel;
    [SerializeField] private TextMeshProUGUI m_Text_HighestLevel;
    [SerializeField] private Image m_Image_SpellPhaseLeftoverTimeSlider;

    [SerializeField] private InputActionReference m_ShowActionRef;
    [SerializeField] public GameObject InputCanvas;
    [SerializeField] public GameObject TutoBot;
    [SerializeField] private Hand mHand;

    [SerializeField] private AudioClip elementSelectSound;
    [SerializeField] private AudioClip elementCancelSound;

    [SerializeField] AudioClip tutoStartSound;
    [SerializeField] AudioClip tutoWowSound;

    [SerializeField] private float ScriptSpeed;

    private Transform mCamera;

    #region Enum

    enum Elements
    {
        eIDLE,
        eFire,
        eWater,
    }

    enum Skills
    {
        eIDLE,

        eFire_1 = 11,
        eFire_2,

        eWater_1 = 21,
        eWater_2,
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

    // ��Ȯ��
    private float mPercentage;

    // ����
    private int mVolume;

    // �� �ð�
    private float mTimes;

    private Vector3 enemyPos;
    private Vector3 magicPos;

    private MobController mobCon;
    private int mobScriptIndex;

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
    /// �ʱ�ȭ
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
        mobCon = TutoMob.GetComponent<MobController>();

        m_PatternRecognitionPhase.HandPoint = mHandPoint;

        SetElementGameObject(false);
        SetSkillGameObject(false, selectedElement);
        SetSkillEffect(false, selectedSkill);
        InputCanvas.SetActive(false);
        m_PatternRecognitionTimer.gameObject.SetActive(false);
    }

    private void ResetAll()
    {
        selectedElement = Elements.eIDLE;
        selectedSkill = Skills.eIDLE;

        mPercentage = 1.0f;
        mVolume = 5;
    }

    private void SetHandBool()
    {
        if (selectedElement == Elements.eIDLE)
        {
            mHand.SetBool(false);
        }

        else
        {
            mHand.SetBool(true);
        }
    }

    private void ActiveMobScripts()
    {
        mobCon.SetScript(mobScriptIndex);
        mobScriptIndex++;
    }

    IEnumerator StartScriptsCorountine(int _start, int _end)
    {
        for (int i = _start; i <= _end; i++)
        {
            mobCon.SetScript(i);
            yield return new WaitForSeconds(ScriptSpeed);
        }

        mobCon.OffScript(_end);
    }

    /// <summary>
    /// Element GameObject ��ü�� SetActive
    /// </summary>
    /// <param name="set"><bool ��>
    private void SetElementGameObject(bool set)
    {
        //if (set == true)
        //{
        //    Vector3 _camera = mCamera.position;
        //    _camera.y = enemyPos.y;
        //    Vector3 direction = (enemyPos - _camera).normalized;
        //    ElementSet.transform.position = mCamera.position + direction * 1;
        //    Quaternion lookRot = Quaternion.LookRotation(direction);
        //    ElementSet.transform.rotation = lookRot;
        //}

        elementFire.SetActive(set);
        elementWater.SetActive(set);
        elementLight.SetActive(set);
        elementDark.SetActive(set);
    }

    /// <summary>
    /// Skill Object���� SetActive ����
    /// </summary>
    /// <param name="set"><true or false>
    /// <param name="element"><enum Element>
    private void SetSkillGameObject(bool set, Elements element)
    {
        //if (set == true)
        //{
        //    Vector3 _camera = mCamera.position;
        //    _camera.y = enemyPos.y;
        //    Vector3 direction = (enemyPos - _camera).normalized;
        //    SkillUISet.transform.position = mCamera.position + direction * 1;
        //    Quaternion lookRot = Quaternion.LookRotation(direction);
        //    SkillUISet.transform.rotation = lookRot;
        //}

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
        magicPos = gameObject.transform.position;
        if (set == true)
        {
            //SkillUISet.transform.position = mCamera.position + mCamera.forward * 1;
            //Vector3 direction = (mCamera.position - SkillUISet.transform.position).normalized;
            //Quaternion lookRot = Quaternion.LookRotation(direction);
            //SkillUISet.transform.rotation = lookRot;
        }

        switch (skill)
        {
            case Skills.eWater_2:
                {
                    WitchEffectManager.instance.ChainIceboltEffect(magicPos, enemyPos);
                    skillTime = WitchEffectManager.instance.GetHealingWaveTime();
                }
                break;

            case Skills.eIDLE:
                {
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

    public void FireSelect()
    {
        isElementSelected = true;
        selectedElement = Elements.eFire;
    }

    public void WaterSelect()
    {
        if (isElementSelected == true)
        {
            selectedElement = Elements.eWater;
        }

        else
        {
            NotThisElement();
        }
    }

    public void NotThisElement()
    {
        Debug.Log("�� ���Ұ� �ƴϾ�");
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
        //SetHandBool();
    }


    IEnumerator MyTurn()
    {
        TutoMob.SetActive(true);
        mobScriptIndex = 0;

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(FireSelectRoutine());
        yield return StartCoroutine(ReSelectElemnt());
        yield return StartCoroutine(WaterSelectRoutine());
        yield return StartCoroutine(SkillSelectPhase());

        var _elementType = (EElementType)((int)selectedSkill / 10);
        var _skillType = (ESkillType)((int)selectedSkill % 10);
        var _skill = m_SkillInfoFinder.Find(_elementType, _skillType);
        m_PatternRecognitionPhase.SetPattern(_skill.Pattern);


        yield return StartCoroutine(StartScriptsCorountine(9, 12));
        // ���� �̸����� �ð�
        yield return StartCoroutine(m_PatternRecognitionPhase.ShowNodesInPathStep());
        yield return StartCoroutine(m_PatternRecognitionPhase.ShowPathPreviewStep());
        yield return new WaitForSeconds(1);

        StartCoroutine(StartScriptsCorountine(13, 14));
        // ���� �׷����� �ð�
        StartCoroutine(UpdatePatternRecognitionPhaseUICoroutine());
        yield return StartCoroutine(m_PatternRecognitionPhase.TryDrawPathStep());

        yield return StartCoroutine(StartScriptsCorountine(15, 18));
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(UpdateSkillPhaseUICoroutine());
        yield return StartCoroutine(m_SpellPhase.StartPhase());


        ShootMagic(selectedSkill);
        yield return new WaitForSeconds(skillTime);

        selectedElement = Elements.eIDLE;
        selectedSkill = Skills.eIDLE;

        yield return StartCoroutine(StartScriptsCorountine(19, 21));
        TutoMob.SetActive(false);
    }

    private IEnumerator UpdatePatternRecognitionPhaseUICoroutine()
    {
        m_Image_PatternRecognitionPhaseLeftoverTimeSlider.fillAmount = 1.0f;
        m_PatternRecognitionTimer.gameObject.SetActive(true);

        // spell phase�� ���� �� �ڵ����� �ڷ�ƾ�� �����Ű����,
        // bool������ ������ �����ʷ� ����մϴ�.
        bool _bEndPhase = false;
        UnityAction _callback = () => _bEndPhase = true;
        m_PatternRecognitionPhase.OnEndPhase.AddListener(_callback);

        while (true)
        {
            if (_bEndPhase)
                break;

            m_Image_PatternRecognitionPhaseLeftoverTimeSlider.fillAmount =
                m_PatternRecognitionPhase.GetPhaseLeftoverTime() / m_PatternRecognitionPhase.GetTryDrawDuration();

            yield return null;
        }

        m_PatternRecognitionPhase.OnEndPhase.RemoveListener(_callback);

        m_PatternRecognitionTimer.gameObject.SetActive(false);
    }

    IEnumerator FireSelectRoutine()
    {
        Debug.Log("Elements SetActive (true)");
        ActiveMobScripts(); // ����

        yield return new WaitForSeconds(ScriptSpeed);

        ActiveMobScripts(); // �Ӽ�
        yield return new WaitForSeconds(ScriptSpeed);

        ActiveMobScripts(); // ��
        yield return new WaitForSeconds(ScriptSpeed);

        SetElementGameObject(true);

        while (selectedElement != Elements.eFire)
        {
            yield return null;
        }
        SoundManager.instance.SFXPlay("ElementSelect", elementSelectSound);
        SetElementGameObject(false);

        ActiveMobScripts(); // ����
        yield return new WaitForSeconds(ScriptSpeed);

        Debug.Log("Elements SetActive (false)");
    }

    IEnumerator SkillSelectPhase()
    {
        SetSkillGameObject(true, selectedElement);
        ActiveMobScripts(); // ��ų ������
        yield return new WaitForSeconds(ScriptSpeed);

        ActiveMobScripts(); // ü�� ���̽�
        yield return new WaitForSeconds(ScriptSpeed);


        while (selectedSkill != Skills.eWater_2)
        {
            yield return null;
        }

        SetSkillGameObject(false, selectedElement);
    }

    IEnumerator ReSelectElemnt()
    {
        SetSkillGameObject(true, selectedElement);
        yield return new WaitForSeconds(0.5f);

        ActiveMobScripts(); // �����̴�
        yield return new WaitForSeconds(ScriptSpeed);


        ActiveMobScripts(); // ���� ��Ʈ�ѷ�
        yield return new WaitForSeconds(ScriptSpeed);


        while (selectedElement != Elements.eIDLE)
        {
            if (m_ShowActionRef.action.IsPressed())
            {
                Debug.Log("ResetAll");
                ResetAll();
                SetSkillGameObject(false, (int)Skills.eIDLE);
                SoundManager.instance.SFXPlay("ElementCancel", elementCancelSound);
            }

            yield return null;
        }
    }

    IEnumerator WaterSelectRoutine()
    {
        SetElementGameObject(true);

        ActiveMobScripts(); // ��
        yield return new WaitForSeconds(ScriptSpeed);


        while (selectedElement != Elements.eWater)
        {
            yield return null;
        }

        SoundManager.instance.SFXPlay("ElementSelect", elementSelectSound);

        SetElementGameObject(false);
    }

    public void InitCoroutine()
    {
        SoundManager.instance.SFXPlay("TutoStart", tutoStartSound);
        Debug.Log("Start Coroutine");
        CapsuleCollider tmp = TutoBot.GetComponent<CapsuleCollider>();
        StartCoroutine(MyTurn());
        tmp.enabled = false;
    }


    private IEnumerator UpdateSkillPhaseUICoroutine()
    {
        m_Image_SpellPhaseLeftoverTimeSlider.fillAmount = 1.0f;
        m_SpellPanel.gameObject.SetActive(true);

        // spell phase�� ���� �� �ڵ����� �ڷ�ƾ�� �����Ű����,
        // bool������ ������ �����ʷ� ����մϴ�.
        bool _bEndPhase = false;
        UnityAction _callback = () => _bEndPhase = true;
        m_SpellPhase.OnEndPhase.AddListener(_callback);

        while (true)
        {
            if (_bEndPhase)
                break;

            m_Text_CurrentLevel.text = m_SpellPhase.GetCurrentLevel().ToString();
            m_Text_HighestLevel.text = m_SpellPhase.GetHighestLevel().ToString();
            m_Image_SpellPhaseLeftoverTimeSlider.fillAmount = m_SpellPhase.GetPhaseLeftoverTime() / m_SpellPhase.GetDuration();

            yield return null;
        }

        m_SpellPhase.OnEndPhase.RemoveListener(_callback);

        m_SpellPanel.gameObject.SetActive(false);
    }
}