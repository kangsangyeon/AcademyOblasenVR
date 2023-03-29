using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    #region GameObjects

    #region Element Object
    public GameObject elementFire;
    public GameObject elementWater;
    public GameObject elementLight;
    public GameObject elementDark;
    #endregion

    #region Skill Object

    #region Fire
    public GameObject FireSkill_1;
    public GameObject FireSkill_2;
    public GameObject FireSkill_3;
    public GameObject FireSkill_4;
    #endregion

    #region Water
    public GameObject WaterSkill_1;
    public GameObject WaterSkill_2;
    public GameObject WaterSkill_3;
    public GameObject WaterSkill_4;
    #endregion

    #region Light
    public GameObject LightSkill_1;
    public GameObject LightSkill_2;
    public GameObject LightSkill_3;
    public GameObject LightSkill_4;
    #endregion

    #region Dark
    public GameObject DarkSkill_1;
    public GameObject DarkSkill_2;
    public GameObject DarkSkill_3;
    public GameObject DarkSkill_4;
    #endregion

    #region ECT

    public GameObject ReturnObject;

    #endregion

    #endregion

    #region UI Object

    public GameObject TurnButton;

    #endregion

    #endregion

    #region Private Fields

    private int mHP;
    private int mMP;

    private bool isMyTurn;

    private int selectedElement;
    private int selectedSkill;

    // 정확도
    private float mPercentage;

    // 볼륨
    private float mVolume;

    // 턴 시간
    private float mTimes;

    // 페이즈는 1 ~ 5까지 int로 체크
    private int mPhase;

    #endregion

    #region Public Fields



    #endregion

    #region Private Methods

    /// <summary>
    /// 초기화
    /// </summary>
    private void Initailize()
    {
        mHP = 100;
        mMP = 100;

        isMyTurn = false;

        selectedElement = 0;
        selectedSkill = 0;

        mPercentage = 0f;
        mTimes = 0f;

        mPhase = 1;

        SetElementGameObject(false);
        SetSkillGameObject(false, 5);

        TurnButton.SetActive(true);
    }

    /// <summary>
    /// Element GameObject 전체의 SetActive
    /// </summary>
    /// <param name="set"><bool 값>
    private void SetElementGameObject(bool set)
    {
        elementFire.SetActive(set);
        elementWater.SetActive(set);
        elementLight.SetActive(set);
        elementDark.SetActive(set);
    }

    /// <summary>
    /// Skill Object의 SetActive
    /// </summary>
    /// <param name="set"><bool 값>
    /// <param name="value"><선택된 스킬>
    private void SetSkillGameObject(bool set, int value)
    {
        ReturnObject.SetActive(set);

        switch (value)
        {
            case 1:
                {
                    FireSkill_1.SetActive(set);
                    FireSkill_2.SetActive(set);
                    FireSkill_3.SetActive(set);
                    FireSkill_4.SetActive(set);
                }
                break;

            //case 2:
            //    {
            //        WaterSkill_1.SetActive(set);
            //        WaterSkill_2.SetActive(set);
            //        WaterSkill_3.SetActive(set);
            //        WaterSkill_4.SetActive(set);
            //    }
            //    break;

            //case 3:
            //    {
            //        LightSkill_1.SetActive(set);
            //        LightSkill_2.SetActive(set);
            //        LightSkill_3.SetActive(set);
            //        LightSkill_4.SetActive(set);
            //    }
            //    break;

            //case 4:
            //    {
            //        DarkSkill_1.SetActive(set);
            //        DarkSkill_2.SetActive(set);
            //        DarkSkill_3.SetActive(set);
            //        DarkSkill_4.SetActive(set);
            //    }
            //    break;

            case 5:
                {
                    FireSkill_1.SetActive(set);
                    FireSkill_2.SetActive(set);
                    FireSkill_3.SetActive(set);
                    FireSkill_4.SetActive(set);

                    //WaterSkill_1.SetActive(set);
                    //WaterSkill_2.SetActive(set);
                    //WaterSkill_3.SetActive(set);
                    //WaterSkill_4.SetActive(set);

                    //LightSkill_1.SetActive(set);
                    //LightSkill_2.SetActive(set);
                    //LightSkill_3.SetActive(set);
                    //LightSkill_4.SetActive(set);

                    //DarkSkill_1.SetActive(set);
                    //DarkSkill_2.SetActive(set);
                    //DarkSkill_3.SetActive(set);
                    //DarkSkill_4.SetActive(set);
                }
                break;

            default:
                break;
        }

    }
    #endregion

    #region Phase Methods

    #region Wait for Turn Phase

    private void EnemyTurnEntry()
    {
        mPhase = 0;
        selectedElement = 0;
        selectedSkill = 0;
    }

    private void EnemyTurn()
    {
        
    }

    public void EnemyTurnEnd()
    {
        TurnButton.SetActive(false);
        Debug.Log("Element Phase Start");
        ElementPhaseEntry();
    }

    #endregion

    #region Element Phase Methods
    private void ElementPhaseEntry()
    {
        SetElementGameObject(true);

        mPhase = 1;
    }

    private void ElementSelected()
    {
        SetElementGameObject(false);
        SkillPhaseEntry();
    }

    private void ElementPhase()
    {
        if (selectedElement != 0)
        {
            ElementSelected();
        }
    }

    public void SetElement(int value)
    {
        selectedElement = value;
    }
    #endregion


    #region Skill Phase Methods
    private void SkillPhaseEntry()
    {
        if (selectedElement != 0)
        {
            SetSkillGameObject(true, selectedElement);
            mPhase = 2;
        }
    }

    private void SkillSelected()
    {
        SetSkillGameObject(false, 5);
        MagicPhaseEntry();
    }

    private void SkillPhase()
    {
        if (selectedSkill != 0)
        {
            SkillSelected();
        }
    }
    #endregion

    #region Magic Phase Methods
    private void MagicPhaseEntry()
    {
        mPhase = 3;
    }
    private void MagicDrawing()
    {

    }
    private void MagicDrawed()
    {

    }
    private void MagicPhase()
    {

    }
    #endregion

    #region Shooting Phase Methods
    private void ShootingPhase()
    {

    }
    #endregion

    #region Caculate Phase Methods
    public void CalculatePhase(int HP)
    {
        mHP += HP;
    }
    #endregion

    #endregion


    #region Public Methods
    public int HP
    {
        get { return this.mHP; }
        set { this.mHP += value; }
    }

    public int MP
    {
        get { return this.mMP; }
        set { this.mMP += value; }
    }

    public bool MTurn
    {
        get { return this.isMyTurn; }
        set { this.isMyTurn = !isMyTurn; }
    }

    public int MSkill
    {
        get { return this.selectedSkill; }
    }

    public float MPercentage
    {
        get { return this.mPercentage; }
    }

    #endregion


    #region MonoBehaviour Callbacks
    // Start is called before the first frame update

    void Awake()
    {
        Initailize();
        Debug.Log("Initialize complete");

        EnemyTurnEntry();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (mPhase)
        {
            case 0:
                {

                }
                break;

            case 1:
                {
                    ElementPhase();
                }
                break;

            case 2:
                {

                }
                break;

            case 3:
                {

                }
                break;

            case 4:
                {

                }
                break;

            default:
                break;
        }

    }

    #endregion
}
