using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;


public class PlayerManagerCoroutine : MonoBehaviour
{
    [SerializeField] private InputActionReference m_SelectButton;
    [SerializeField] private Hand hand;

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

    private GameObject elementUISet;

    private GameObject elementFire;
    private GameObject elementWater;
    private GameObject elementLight;
    private GameObject elementDark;
    #endregion

    #region Skill Object

    private GameObject skillUISet;

    #region Fire
    private GameObject UI_FireSkill_1;
    private GameObject UI_FireSkill_2;
    private SkillInfo Info_FireSkill_1;
    private SkillInfo Info_FireSkill_2;
    #endregion

    #region Water
    private GameObject UI_WaterSkill_1;
    private GameObject UI_WaterSkill_2;
    private SkillInfo Info_WaterSkill_1;
    private SkillInfo Info_WaterSkill_2;

    #endregion

    #region Light
    private GameObject UI_LightSkill_1;
    private GameObject UI_LightSkill_2;
    private SkillInfo Info_LightSkill_1;
    private SkillInfo Info_LightSkill_2;

    #endregion

    #region Dark
    private GameObject UI_DarkSkill_1;
    private GameObject UI_DarkSkill_2;
    private SkillInfo Info_DarkSkill_1;
    private SkillInfo Info_DarkSkill_2;

    #endregion

    #region ECT
    #endregion

    #endregion

    #region Skill Effect Object

    private GameObject circleEffect;

    #endregion

    #region ECT Object

    private GameObject TurnButton;

    #endregion

    #endregion

    #region Private Fields

    [SerializeField] private Transform mCamera;
    [SerializeField] private PhotonView photonView;

    private int mHP;
    private int mMP;

    private int mDmg;

    private bool isMyTurn;

    private float mSkillTime;

    private Elements selectedElement;

    private Skills selectedSkill;

    // ��Ȯ��
    private float mPercentage;

    // ����
    private int mVolume;

    // �� �ð�
    private float mTimes;

    private int mViewID;

    private Vector3 enemyPos;
    private Vector3 myPos;
    private Vector3 effectPos;

    #endregion

    #region Private Methods

    private void FindGameObjects()
    {
        #region Element Object
        elementUISet = GameObject.Find("Element UI");
        elementFire = GameObject.Find("Fire Element");
        elementWater = GameObject.Find("Water Element");
        elementLight = GameObject.Find("Light Element");
        elementDark = GameObject.Find("Dark Element");
        #endregion

        #region Skill Objects
        skillUISet = GameObject.Find("Skill UI");

        UI_FireSkill_1 = GameObject.Find("Fire Skill_1");
        UI_FireSkill_2 = GameObject.Find("Fire Skill_2");
        Info_FireSkill_1 = GameObject.Find("Fire01").GetComponent<SkillInfo>();
        Info_FireSkill_2 = GameObject.Find("Fire02").GetComponent<SkillInfo>();

        UI_WaterSkill_1 = GameObject.Find("Water Skill_1");
        UI_WaterSkill_2 = GameObject.Find("Water Skill_2");
        Info_WaterSkill_1 = GameObject.Find("Water01").GetComponent<SkillInfo>();
        Info_WaterSkill_2 = GameObject.Find("Water02").GetComponent<SkillInfo>();

        UI_LightSkill_1 = GameObject.Find("Light Skill_1");
        UI_LightSkill_2 = GameObject.Find("Light Skill_2");
        Info_LightSkill_1 = GameObject.Find("Light01").GetComponent<SkillInfo>();
        Info_LightSkill_2 = GameObject.Find("Light02").GetComponent<SkillInfo>();

        UI_DarkSkill_1 = GameObject.Find("Dark Skill_1");
        UI_DarkSkill_2 = GameObject.Find("Dark Skill_2");
        Info_DarkSkill_1 = GameObject.Find("Dark01").GetComponent<SkillInfo>();
        Info_DarkSkill_2 = GameObject.Find("Dark02").GetComponent<SkillInfo>();
        #endregion

        #region ECT
        TurnButton = GameObject.Find("Turn Button");
        #endregion

        #region Effect Object
        circleEffect = GameObject.Find("Circle Effect");
        #endregion

    }

    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    private void Initailize()
    {
        if (photonView.IsMine == true)
        {
            isMyTurn = false;

            selectedElement = Elements.eIDLE;
            selectedSkill = Skills.eIDLE;

            mPercentage = 0f;
            mVolume = 0;
            mTimes = 0;

            mSkillTime = 0f;

            enemyPos = Vector3.zero;
            effectPos = Vector3.zero;

            var _photonView = gameObject.GetComponent<Photon.Pun.PhotonView>();
            mViewID = _photonView.ViewID;

            FindGameObjects();

            SetElementGameObject(false);
            SetSkillGameObject(false, selectedElement);

            TurnButton.SetActive(false);
        }
    }

    private void ResetAll()
    {
        selectedElement = Elements.eIDLE;
        selectedSkill = Skills.eIDLE;

        mPercentage = 0.0f;
        mTimes = 0;
        mVolume = 0;

        isMyTurn = false;
    }
    private void SetHandBool()
    {
        if (selectedElement == Elements.eIDLE)
        {
            hand.SetBool(false);
        }

        else
        {
            hand.SetBool(true);
        }
    }
    /// <summary>
    /// Element GameObject ��ü�� SetActive
    /// </summary>
    /// <param name="set"><bool ��>
    private void SetElementGameObject(bool set)
    {
        if (set == true)
        {
            Vector3 _camera = mCamera.position;
            _camera.y = enemyPos.y;
            Vector3 direction = (enemyPos - _camera).normalized;
            elementUISet.transform.position = mCamera.position + direction * 1;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            elementUISet.transform.rotation = lookRot;
        }

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
        if (set == true)
        {
            Vector3 _camera = mCamera.position;
            _camera.y = enemyPos.y;
            Vector3 direction = (enemyPos - _camera).normalized;
            skillUISet.transform.position = mCamera.position + direction * 1;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            skillUISet.transform.rotation = lookRot;
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

    private void SetSkillEffectAndDMG(bool set, Skills skill)
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
                    mSkillTime= WitchEffectManager.instance.GetMeteorTime();
                    mDmg = Info_FireSkill_1.Damage;
                }
                break;

            case Skills.eFire_2:
                {
                    WitchEffectManager.instance.MagmaImpactEffect(effectPos, enemyPos);
                    mSkillTime= WitchEffectManager.instance.GetMagmaImpactTime(effectPos, enemyPos);
                    mDmg = Info_FireSkill_2.Damage;

                }
                break;

            case Skills.eWater_1:
                {
                    WitchEffectManager.instance.HealingWaveEffect(effectPos);
                    mSkillTime= WitchEffectManager.instance.GetHealingWaveTime();
                    mDmg = Info_WaterSkill_1.Damage;
                }
                break;

            case Skills.eWater_2:
                {
                    WitchEffectManager.instance.HealingWaveEffect(effectPos);
                    mSkillTime= WitchEffectManager.instance.GetHealingWaveTime();
                    mDmg = Info_WaterSkill_2.Damage;
                }
                break;

            case Skills.eDark_1:
                {
                    WitchEffectManager.instance.CircleOfCurseEffect(enemyPos);
                    mSkillTime= WitchEffectManager.instance.GetCircleOfCurseTime();
                    mDmg = Info_DarkSkill_1.Damage;
                }
                break;

            case Skills.eDark_2:
                {
                    WitchEffectManager.instance.DarkMistEffect(enemyPos);
                    mSkillTime= WitchEffectManager.instance.GetDarkMistTime();
                    mDmg = Info_DarkSkill_2.Damage;
                }
                break;

            case Skills.eLight_1:
                {
                    WitchEffectManager.instance.JudgementSwordEffect(effectPos, enemyPos);
                    mSkillTime= WitchEffectManager.instance.GetJudgementSwordTime();
                    mDmg = Info_LightSkill_1.Damage;
                }
                break;

            case Skills.eLight_2:
                {
                    WitchEffectManager.instance.LighteningStrikeEffect(enemyPos);
                    mSkillTime = WitchEffectManager.instance.GetLightningStrikeTime();
                    mDmg = Info_LightSkill_2.Damage;
                }
                break;

            case Skills.eIDLE:
                {
                }
                break;

            default:
                {
                    Debug.Log("SetSkillEffect default case");
                    circleEffect.SetActive(set);
                }
                break;
        }
    }

    private void MagicEffectStart(Skills skill)
    {
        SetSkillEffectAndDMG(true, skill);
    }

    private void SetTurnButton(bool set)
    {
        if (set == true)
        {
            Vector3 _camera = mCamera.position;
            _camera.y = enemyPos.y;
            Vector3 direction = (enemyPos - _camera).normalized;
            TurnButton.transform.position = mCamera.position + direction * 1;
            Quaternion lookRot = Quaternion.LookRotation(direction);
            TurnButton.transform.rotation = lookRot;
        }

        TurnButton.SetActive(set);
    }

    #endregion

    #region Public Fields

    public PlayerLogic _logic;

    #endregion

    #region Public Methods
    public void InitCoroutine()
    {
        Debug.Log("Start Coroutine");
        StartCoroutine(MyTurn());
    }

    /// <summary>
    /// selectedElement setter
    /// Fire = 1 , Water = 2, Light = 3, Dark = 4
    /// </summary>
    /// <param name="value"></param>
    public void SetElement(int value)
    {
        if (photonView.IsMine)
            selectedElement = (Elements)value;
    }

    /// <summary>
    /// selectedSkill setter
    /// Fire = 1n, Water = 2n ...
    /// </summary>
    /// <param name="value"></param>
    public void SetSkill(int value)
    {
        selectedSkill = (Skills)value;
    }

    public int GetHP()
    {
        if (_logic != null)
        {
            mHP = _logic.GetHP();
            return mHP;
        }
        else
        {
            return 0;
        }
    }

    public int GetMP()
    {
        if (_logic != null)
        {
            mMP = _logic.GetMP();
            return mMP;

        }
        else
        {
            return 0;
        }
    }

    #endregion

    #region MonoBehaviour Callbacks

    void Awake()
    {
        Initailize();
        Debug.Log("Initialize Complete!");
    }

    void Update()
    {
        SetHandBool();
    }

    #endregion

    #region Coroutine Methods
    IEnumerator MyTurn()
    {
        yield return StartCoroutine(ElementSelectPhase());

        yield return StartCoroutine(SkillSelectPhase());

        // ī�����ϰ� ��������� �׷�����
        SendDataToPhoton(mViewID, (int)selectedSkill, mPercentage, mVolume);

        MagicEffectStart(selectedSkill);


        // ���� ��� ������ �������ٰ� ������.
        _logic.MSG_C2S_ATTACK_INFO((int)selectedSkill, mDmg);
        Debug.Log("������ ��ų = " + selectedSkill + " ������ = " + mDmg);
        //_logic.MSG_C2S_ATTACK_INFO(11, totalDMG);

        yield return new WaitForSeconds(mSkillTime);

        // �� ����� Attack_Result���� ü��, ���� ��ġ �� �����ϰ� �� �Ŀ� ����.
        //Debug.Log("Turn End! Wait 5sec");
        //yield return new WaitForSeconds(5);

        ResetAll();
    }

    IEnumerator ElementSelectPhase()
    {
        SetElementGameObject(true);
        Debug.Log("Elements SetActive (true)");
        yield return null;

        while (selectedElement == Elements.eIDLE)
        {
            yield return null;
        }

        Debug.Log("Element Selected");

        SetElementGameObject(false);
        Debug.Log("Elements SetActive (false)");
    }

    IEnumerator SkillSelectPhase()
    {
        SetSkillGameObject(true, selectedElement);
        Debug.Log("Skills SetActive (true)");

        while (selectedSkill == Skills.eIDLE)
        {
            yield return null;
        }

        SetSkillGameObject(false, selectedElement);
        Debug.Log("Skills SetActive (false)");
    }

    IEnumerator ReSelectElement()
    {
        SetSkillGameObject(true, selectedElement);
        yield return new WaitForSeconds(0.5f);

        while (selectedElement != Elements.eIDLE)
        {
            if (m_SelectButton.action.IsPressed())
            {
                Debug.Log("ResetAll");
                selectedElement = Elements.eIDLE;
                SetSkillGameObject(false, (int)Skills.eIDLE);
            }
            yield return null;
        }

        yield return null;

    }

    // 2�� ���� ȿ���� ���� ���� ���� �ʿ��� �����̸� ���� �ڷ�ƾ
    IEnumerator DoubleElectric(float time)
    {
        // ù��° ����
        _logic.DownHP(Info_LightSkill_2.Damage);
        ChangeElectric_Count(-1);       // ��ø ����

        yield return new WaitForSeconds(time);

        // �ι�° ����
        _logic.DownHP(Info_LightSkill_2.Damage);
        ChangeElectric_Count(-1);

        // �� ����
        isMyTurn = true;
    }

    private void SendDataToPhoton(int viewID, int skill, float percent, int volum)
    {
        Debug.Log(viewID + " " + skill + " " + percent + " " + volum);
    }

    #endregion

    #region Network Logic

    public void S2C_Game_Ready()
    {
        // �÷��̾�� ���� �غ� �Ǿ����� �����.

        Debug.Log(mViewID + "Client �̱��� : S2C_GAME_READY");

        var _components = GameObject.FindObjectsOfType<PlayerManagerCoroutine>();
        foreach (var _component in _components)
        {
            var _photonView = _component.gameObject.GetComponent<Photon.Pun.PhotonView>();

            if (_photonView.IsMine == true)
            {
                myPos = _component.transform.position;
            }
            else if (_photonView.IsMine == false)
            {
                enemyPos = _component.transform.position;
            }
        }

        // ���� ���� �غ� �Ϸ�Ǿ��ٰ� �������� �˸���.
        // ��ư�� ������ �Ʒ� �Լ��� ȣ��
        _logic.MSG_C2S_GAME_READY_OK();
    }

    public void S2C_Game_Start()
    {
        // ������ ���۵Ǿ��ٴ� �˸� ���� ���´�.
        // todo :: ���� ���� UI ���� ǥ������.

        Debug.Log(mViewID + "Client �̱��� : S2C_GAME_START");

    }

    public void S2C_Announce_StartPlayer(bool myTurn)
    {
        // ���� ���� �÷��̾����� �ƴ��� �˷��ش�.
        // todo :: ��,�İ� �˸� ǥ�� Ȥ�� UI ���̱�

        if (myTurn == true)
        {
            // ���� �˸� �޽��� ǥ��
            Debug.Log(mViewID + "Client �̱��� : S2C_Announce_StartPlayer_myTurn");

        }
        else if (myTurn == false)
        {
            // �İ� �˸� �޽��� ǥ��
            Debug.Log(mViewID + "Client �̱��� : S2C_Announce_StartPlayer_anemyTurn");

        }

        // ���� �İ� �˸� �޽����� �����ߴٰ� �������� �˸�
        _logic.MSG_C2S_ANNOUNCE_STARTPLAYER_OK(isMyTurn);
    }

    public void S2C_Start_Turn(bool myTurn)
    {
        // �ڷ�ƾ���� ����
        // �������� �̹� ���� �÷��̾ �������� �˷��ش�.
        if (myTurn == true)
        {
            // ������ ó������. ����Ʈ�� �� �־��ּ���
            // ���� ��ø�Ǿ�������
            if (_logic.GetElectric_Count() == 3)
            {
                StartCoroutine(DoubleElectric(3.0f));
            }
            else if (_logic.GetElectric_Count() == 1)
            {
                _logic.DownHP(Info_LightSkill_2.Damage);
                ChangeElectric_Count(-1);
                isMyTurn = true;
            }


            Debug.Log("Turn Start");
            SetTurnButton(true);
        }

        // ����� ���� ���϶�
        // todo :: ����� ���� ���϶� ���𰡸� �� �� �ֵ��� ����. �� ��ų Ȯ�� ��
        else if (myTurn == false)
        {
            isMyTurn = false;
        }

        // ���� �� ó�� (��ų�� ���ϰ�, ������ �׸���... ����� ��������.
        // MyTurnButton �� ���̵��� �ϸ� �ɲ�����!
    }


    public void S2C_EndGame(bool loser)
    {
        // ���� ������
        if (loser == true)
        {
            // �й� �޽����� �����.
            Debug.Log(mViewID + "Client �̱��� : S2C_EndGame_anemyWin");
        }
        else
        {
            // �¸� �޽����� �����
            Debug.Log(mViewID + "Client �̱��� : S2C_EndGame_Win!");
        }


        // ������ ���� ���� ó���� ���� �������� �˸���. (��� �� �ʿ��Ѱ� �ƴ�)
        _logic.MSG_C2S_END_GAME_OK();


        // todo :: ���� ���� ������ �Ѿ��
    }

    // ������ �ϰų� ������ �´� ���� ü�� ���� ��ȭ�� ���⼭ ó���Ѵ�.
    public void S2C_Attack_Result(bool myTurn, int skillInfo, int totalDMG)
    {
        if (photonView.IsMine)
        {
            // ���� �����Ѱ��϶�
            if (myTurn == true)
            {
                switch (skillInfo)
                {
                    case 11:        // �ҽ�ų 1��
                        // ���� ������ �� �����ϴ� ����Ʈ�� ó���ϰ� ������ ����Ʈ����.
                        if (myTurn == true)
                        {
                            DownMP(Info_FireSkill_1.UsedMP);
                        }

                        // ��밡 ������ ��. �´� ����ƮƮ�� ó���ϰ� ������ ����Ʈ����.
                        else if (myTurn == false)
                        {

                            DownHP(totalDMG);
                        }
                        break;

                    case 12:        // �ҽ�ų 2��
                        if (myTurn == true)
                        {
                            DownMP(Info_FireSkill_2.UsedMP);
                        }

                        else if (myTurn == false)
                        {
                            DownHP(totalDMG);
                        }
                        break;

                    case 21:        // ����ų 1��
                        if (myTurn == true)
                        {
                            UpHP(totalDMG);
                        }

                        else if (myTurn == false)
                        {

                        }
                        break;

                    case 22:        // ����ų 2��
                        if (myTurn == true)
                        {
                            UpMP(totalDMG);
                        }

                        else if (myTurn == false)
                        {

                        }
                        break;

                    case 31:        // ������ų 1��
                        if (myTurn == true)
                        {
                            DownMP(Info_LightSkill_2.UsedMP);
                        }

                        else if (myTurn == false)
                        {

                        }
                        break;

                    case 32:        // ������ų 2��
                        if (myTurn == true)
                        {
                            DownMP(Info_LightSkill_2.UsedMP);
                        }

                        else if (myTurn == false)
                        {
                            ChangeElectric_Count(2);
                        }
                        break;

                    case 41:        // ���潺ų 1��      ����� ���� �̱���
                        if (myTurn == true)
                        {
                            DownMP(Info_DarkSkill_2.UsedMP);
                        }

                        else if (myTurn == false)
                        {
                            DownHP(totalDMG);
                        }
                        break;

                    case 42:        // ���潺ų 2��      ����� ���� �̱���
                        if (myTurn == true)
                        {
                            DownMP(Info_DarkSkill_2.UsedMP);
                        }

                        else if (myTurn == false)
                        {
                            DownHP(totalDMG);
                        }
                        break;
                }
            }

            // ��,�� ó���� �������� ���� �����Ѵ�.
            _logic.MSG_C2S_ENDTURN(GetHP(), GetMP());
        }
    }

    private void DownHP(int amount)
    {
        _logic.DownHP(amount);
    }

    private void UpHP(int amount)
    {
        _logic.UpHP(amount);
    }

    private void DownMP(int amount)
    {
        _logic.DownMP(amount);
    }

    private void UpMP(int amount)
    {
        _logic.UpMP(amount);
    }

    private void ChangeElectric_Count(int amount)
    {
        _logic.ChangeElectric_Count(amount);
    }

    #endregion
}
