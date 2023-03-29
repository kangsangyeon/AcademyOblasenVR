using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

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

public class NetworkCharacter : MonoBehaviour, IOnEventCallback
{
    public UnityEvent OnHPChanged;
    public UnityEvent OnHPIsZero;
    public UnityEvent OnMPChanged;
    public UnityEvent OnMPIsZero;

    public UnityEvent OnReady;
    public UnityEvent OnFight;
    public UnityEvent OnStartTurn;
    public UnityEvent OnGameSet;

    public UnityEvent<int> OnChangedLockedElement;


    [SerializeField] private int RecoveryManaPerTurn;
    [SerializeField] private Transform m_HandPoint;
    [SerializeField] private ActionBasedController m_LeftHandController;
    [SerializeField] private ActionBasedController m_RightHandController;
    [SerializeField] private Animator animator;
    [SerializeField] private bool m_bIsGirl;

    [SerializeField] private Hand m_Hand;

    private PhotonView m_PhotonView;
    private NetworkGameplay m_NetworkGame;
    private GameplayScene m_Scene;

    private int m_PlayerIndex;

    private bool m_bIsMyTurn;
    private float m_SkillTime;
    private bool m_SecondPlayer;
    private int m_Dmg;

    public bool GetIsGirl() => m_bIsGirl;
    public int GetPlayerIndex() => m_PlayerIndex;
    public int GetOppositePlayerIndex() => m_PlayerIndex == 0 ? 1 : 0;
    public Transform GetMyPoint() => m_Scene.GetPlayerPoint(m_PlayerIndex);
    public Transform GetMyForwardPoint() => m_Scene.GetPlayerForwardPoint(m_PlayerIndex);
    public Transform GetMyUIPoint() => m_Scene.GetPlayerUIPoint(m_PlayerIndex);
    public Transform GetOppositePlayerPoint() => m_Scene.GetPlayerPoint(m_PlayerIndex == 0 ? 1 : 0);
    public Transform GetOppositePlayerForwardPoint() => m_Scene.GetPlayerForwardPoint(m_PlayerIndex == 0 ? 1 : 0);

    private int MaxHP = 150;
    private int MaxMP = 100;

    private int m_HP;
    private int m_MP;
    private int m_ElectricCount = 0;
    private bool UI_ElementLock = false;
    private bool UI_DMGDown = false;
    private bool UI_DefenseDown = false;

    public PhotonView GetPhotonView() => m_PhotonView;
    public int GetHP() => m_HP;
    public int GetMP() => m_MP;

    public void AddHP(int _amount)
    {
        m_HP = m_HP + _amount;
        OnHPChanged.Invoke();

        if (m_HP > MaxHP)
        {
            m_HP = MaxHP;
        }
    }

    public void AddMP(int _amount)
    {
        m_MP = m_MP + _amount;
        OnMPChanged.Invoke();

        if (m_MP <= 0)
        {
            OnMPIsZero.Invoke();
            return;
        }

        if (m_MP > MaxMP)
        {
            m_MP = MaxMP;
        }
    }

    public void SetTotalDamageUI()
    {
        m_Scene.GetTotalDamageText().text = Convert.ToString(m_Dmg);
        EElementType _tmp = m_Scene.GetGameplayTurn().GetSelectSkillPhase().GetSelectedElement();
        switch (_tmp)
        {
            case EElementType.Fire:
                {
                    m_Scene.GetTotalDamageText().color = Color.red;
                }
                break;

            case EElementType.Water:
                {
                    m_Scene.GetTotalDamageText().color = Color.blue;
                }
                break;

            case EElementType.Light:
                {
                    m_Scene.GetTotalDamageText().color = Color.yellow;
                }
                break;

            case EElementType.Dark:
                {
                    m_Scene.GetTotalDamageText().color = Color.black;
                }
                break;
        }

        m_Scene.GetTotalDamageUI().SetActive(true);
    }

    public void AddElectricCount(int amount) => m_ElectricCount += amount;

    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        m_NetworkGame = GameObject.FindObjectOfType<NetworkGameplay>();
        m_Scene = GameObject.FindObjectOfType<GameplayScene>();


    }

    void Start()
    {
        if (m_PhotonView.IsMine == true)
        {
            MSG_C2S_CONNECT_MANAGER();

            m_bIsMyTurn = false;
            m_SecondPlayer = false;
            m_SkillTime = 0f;
            m_HP = MaxHP;
            m_MP = MaxMP;

            m_Scene.GetGameplayTurn().GetPatternRecognitionPhase().HandPoint = m_HandPoint;
            m_Scene.GetGameplayTurn().GetPatternRecognitionPhase().Controller = m_RightHandController;

            animator.SetBool("Victory", false);
            animator.SetBool("Lose", false);

            m_PhotonView.RPC("rpcAni", RpcTarget.All, "Idle");

        }

        if (m_PhotonView.IsMine)
        {
            m_Scene.SetMyCharacter(this);
            //m_Scene.GetUIHolder().gameObject.SetActive(true);
            //m_Scene.GetUIHolder().transform.position = m_Scene.GetPlayerUIPoint(m_PlayerIndex).position;
            //m_Scene.GetUIHolder().transform.rotation = m_Scene.GetPlayerUIPoint(m_PlayerIndex).rotation;
            m_Scene.GetGameplayTurn().GetSelectSkillPhase().OnElementChanged.AddListener(m_Hand.AuraSet);
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        m_Scene.GetGameplayTurn().GetSelectSkillPhase().OnElementChanged.RemoveListener(m_Hand.AuraSet);
    }


    public void OnEvent(EventData photonEvent)
    {
        if (m_PhotonView.IsMine == false)
        {
            // 나를 대변하는 플레이어 캐릭터에 대해서만 이벤트를 처리하도록 필터링합니다.
            // 이는, 씬에 나를 대변하는 캐릭터와 다른 플레이어를 대변하는 캐릭터에게도 동일한 메세지가 송신됩니다.
            // 이런 상황은 네트워크 내 모든 클라이언트들이 동일합니다.
            // 만약 서로 자기 자신의 캐릭터가 아닌 캐릭터를 조작하고 그것이 동기화되면, 동일한 메세지에 대해 중복된 처리가 클라이언트 수만큼 늘어날 수 있습니다.
            // 이를 막기 위해, 모든 클라이언트들이 나 자신이 조종하는 캐릭터에 대해서만 처리하도록 필터링하는 것입니다.
            return;
        }

        byte eventCode = photonEvent.Code;

        if (eventCode > 250)
        {
            // photon 내부적으로 소통하기 위해 사용하는 이벤트 메세지는 무시합니다.
            return;
        }

        object[] data = (object[])photonEvent.CustomData;
        int senderViewID = (int)data[0];

        if (eventCode == (byte)NetworkCode.S2C_CONNECT_MANAGER_OK)
        {
            // 클라이언트 연결 메세지가 본인으로 인해 발생한 메세지인지 확인합니다.
            // 본인의 입장에 의해 발생한 메세지라면, 그 메세지 안에 담긴 player index로 멤버 변수를 초기화합니다.

            if (senderViewID == m_PhotonView.ViewID)
            {
                m_PlayerIndex = (int)data[1];
                if (m_PlayerIndex == 0)
                {
                    m_bIsGirl = true;
                }
                else
                {
                    m_bIsGirl = false;
                }

                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "연결된 플레이어 ID : " + senderViewID + "같으면 연결 완료!");
                Debug.Log($"my player index: {m_PlayerIndex}");

                SetUIHolderTransform();
                m_Scene.GetPlayerHPMPUI().SetIsGirl(m_bIsGirl);

                // 결과 씬을 부르기 위해 호스트와의 자동 씬 동기화를 끕니다.
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.AutomaticallySyncScene = false;
                }
            }
            else
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "연결된 플레이어 ID : " + senderViewID + "다른 플레이어 연결 완료");
            }
        }

        if (eventCode == (byte)NetworkCode.S2C_GAME_READY)
        {
            Debug.Log("Client view ID :" + m_PhotonView.ViewID + "게임 준비 요청 메시지 수신 ");

            // TODO: 게임 준비 완료를 물어보는 UI를 표시합니다.
        }

        if (eventCode == (byte)NetworkCode.S2C_GAME_START)
        {
            // UI 정보를 갱신합니다.
            MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);
            Debug.Log("Client view ID :" + m_PhotonView.ViewID + "게임 시작 메시지 수신 ");
            StartCoroutine(On_GAME_START());

        }

        if (eventCode == (byte)NetworkCode.S2C_ANNOUNCE_STARTPLAYER)
        {
            // UI 정보를 갱신합니다.
            MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

            int startPlayerId = (int)data[1];

            if (startPlayerId == m_PhotonView.ViewID)
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "내가 선공!!!");
                m_SecondPlayer = false;
            }
            else
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "내가 후공...");
                m_SecondPlayer = true;
            }
        }

        if (eventCode == (byte)NetworkCode.S2C_START_TURN)
        {
            int turnPlayer = (int)data[1];

            Debug.Log("Client view ID :" + m_PhotonView.ViewID + "턴의 시작 정보 정보 :  " + turnPlayer + " ");

            if (turnPlayer == m_PhotonView.ViewID)
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "나의 공격!!!!");
                //isMyTurn = true;
                On_Start_Turn(true);
            }
            else
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "수비턴....");
                //isMyTurn = false;
                On_Start_Turn(false);
            }
        }

        if (eventCode == (byte)NetworkCode.S2C_ATTACK_RESULT)
        {
            int skillID = (int)data[1];
            int totalDMG = (int)data[2];

            Debug.Log($"attacker id: {senderViewID}");

            // 내가 공격한거면
            if (senderViewID == m_PhotonView.ViewID)
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "스킬명 : " + skillID + " 로 때렸다...!" + "데미지 : " + totalDMG);

                StartCoroutine(On_Attack_Result(senderViewID, skillID, totalDMG));
            }
            else
            {
                Debug.Log("Client view ID :" + m_PhotonView.ViewID + "스킬명 : " + skillID + " 로 맞았다 ㅠㅠㅠ" + "데미지 : " + totalDMG);

                StartCoroutine(On_Attack_Result(senderViewID, skillID, totalDMG));
            }
        }


        if (eventCode == (byte)NetworkCode.S2C_ENDGAME)
        {
            // 게임이 종료되었음. 적당한 처리를 하고 종료 확인 메시지를 보내자.
            Debug.Log("Client view ID :" + m_PhotonView.ViewID + " 게임 종료 처리");

            int losePlayer = (int)data[1];
            StartCoroutine(On_GAME_SET(losePlayer));
        }
    }


    #region Network Event MSG

    private void MSG_C2S_CONNECT_MANAGER()
    {
        object[] content = new object[] { m_PhotonView.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_CONNECT_MANAGER, content, eventOptions);
    }

    public void MSG_C2S_GAME_READY_OK()
    {
        object[] content = new object[] { m_PhotonView.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_GAME_READY_OK, content, eventOptions);
    }

    public void MSG_C2S_ATTACK_INFO(int SkillNum, int TotalDMG)
    {
        object[] content = new object[] { m_PhotonView.ViewID, SkillNum, TotalDMG };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_ATTACK_INFO, content, eventOptions);
    }

    public void MSG_C2S_ENDTURN()
    {
        object[] content = new object[] { m_PhotonView.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_END_TURN, content, eventOptions);
    }


    private void MSG_C2S_HP_ZERO()
    {
        object[] content = new object[] { m_PhotonView.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_HP_ZERO, content, eventOptions);
    }

    public void MSG_C2S_END_GAME_OK()
    {
        object[] content = new object[] { m_PhotonView.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_ENDGAME_OK, content, eventOptions);
    }

    public void MSG_C2S_PLAYER_STATE(int myHp, int myMp, int ElectricCount, bool ElementLock, bool DMGDown, bool DefenseDown)
    {
        object[] content = new object[] { m_PhotonView.ViewID, myHp, myMp, ElectricCount, ElementLock, DMGDown, DefenseDown };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_PLAYER_STATE, content, eventOptions);
    }

    public void MSG_C2S_PLAYER_STATE()
    {
        MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);
    }

    private void MSGSender(NetworkCode Code, object[] content, RaiseEventOptions options)
    {
        PhotonNetwork.RaiseEvent((byte)Code, content, options, SendOptions.SendReliable);
    }


    [PunRPC]
    void rpcAni(string strAni)
    {
        if(strAni == "Damaged_Elec")
        {
            WitchEffectManager.instance.LigtningStrikeEffect2(gameObject.transform.position);
            animator.Play("Damaged");
        }
        else
        {
            Debug.Log(strAni);
            animator.Play(strAni);
        }
    }

    #endregion

    private void SetUIHolderTransform()
    {
        m_Scene.GetUIHolder().gameObject.SetActive(true);
        m_Scene.GetUIHolder().transform.position = m_Scene.GetPlayerUIPoint(m_PlayerIndex).position;
        m_Scene.GetUIHolder().transform.rotation = m_Scene.GetPlayerUIPoint(m_PlayerIndex).rotation;
    }

    private void ResetAll()
    {
        m_bIsMyTurn = false;
        m_SkillTime = 0.0f;
        m_Dmg = 0;
    }

    private float PlaySkillEffect(Skills skill, int attackerId)
    {
        Vector3 _fromPosition;
        Vector3 _toPosition;

        if (attackerId == m_PhotonView.ViewID)
        {
            _fromPosition = GetMyPoint().position;
            if (skill == Skills.eFire_2 || skill == Skills.eWater_2)
            {
                // 예외적으로, 투사체 이펙트의 시작 위치를 공격자 캐릭터의 앞 위치로 설정합니다.
                _fromPosition = GetMyForwardPoint().position;
            }

            _toPosition = GetOppositePlayerPoint().position;
        }

        else
        {
            _fromPosition = GetOppositePlayerPoint().position;
            if (skill == Skills.eFire_2 || skill == Skills.eWater_2)
            {
                // 예외적으로, 투사체 이펙트의 시작 위치를 공격자 캐릭터의 앞 위치로 설정합니다.
                _fromPosition = GetOppositePlayerForwardPoint().position;
            }

            _toPosition = GetMyPoint().position;
        }

        float skillTime = 0.0f;

        switch (skill)
        {
            case Skills.eFire_1:
                {
                    WitchEffectManager.instance.MeteorEffect(_fromPosition, _toPosition);
                    skillTime = WitchEffectManager.instance.GetMeteorTime();
                }
                break;

            case Skills.eFire_2:
                {
                    WitchEffectManager.instance.MagmaImpactEffect(_fromPosition, _toPosition);
                    skillTime = WitchEffectManager.instance.GetMagmaImpactTime(_fromPosition, _toPosition);
                }
                break;

            case Skills.eWater_1:
                {
                    WitchEffectManager.instance.HealingWaveEffect(_fromPosition);
                    skillTime = WitchEffectManager.instance.GetHealingWaveTime();
                }
                break;

            case Skills.eWater_2:
                {
                    WitchEffectManager.instance.ChainIceboltEffect(_fromPosition, _toPosition);
                    skillTime = WitchEffectManager.instance.GetIceboltTime();
                }
                break;

            case Skills.eDark_1:
                {
                    WitchEffectManager.instance.CircleOfCurseEffect(_toPosition);
                    skillTime = WitchEffectManager.instance.GetCircleOfCurseTime();
                }
                break;

            case Skills.eDark_2:
                {
                    WitchEffectManager.instance.DarkMistEffect(_toPosition);
                    skillTime = WitchEffectManager.instance.GetDarkMistTime();
                }
                break;

            case Skills.eLight_1:
                {
                    WitchEffectManager.instance.JudgementSwordEffect(_fromPosition, _toPosition);
                    skillTime = WitchEffectManager.instance.GetJudgementSwordTime();
                }
                break;

            case Skills.eLight_2:
                {
                    WitchEffectManager.instance.LighteningStrikeEffect(_toPosition);
                    skillTime = WitchEffectManager.instance.GetLightningStrikeTime();
                }
                break;
        }

        return skillTime;
    }

    public IEnumerator MyTurn()
    {
        // my turn 이미지가 2초간 보인 뒤 사라집니다.
        m_Scene.GetMyTurnImage().gameObject.SetActive(true);
        m_Scene.GetMyTurnImage().color = Color.white;
        var _tweener = m_Scene.GetMyTurnImage().DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 2.0f);
        yield return _tweener.WaitForCompletion();
        m_Scene.GetMyTurnImage().gameObject.SetActive(false);

        // TODO :: m_SecondPlayer 가 true 면 첫 스킬 마나 소모가 절반임을 알리는 UI를 표시한다.

        // 한 턴에 진행해야 하는 일련의 phase들의 미션을 모두 수행할 때까지 기다립니다.
        yield return StartCoroutine(m_Scene.GetGameplayTurn().StartTurn());

        // 공격 애니메이션 실행
        m_PhotonView.RPC("rpcAni", RpcTarget.All, "Attack_Up");

        Skills selectedSkill = (Skills)(
            (int)m_Scene.GetGameplayTurn().GetSelectSkillPhase().GetSelectedElement() * 10
            + (int)m_Scene.GetGameplayTurn().GetSelectSkillPhase().GetSelectedSkill()
        );

        // TODO: 데미지 계산식을 수정해야 합니다.
        // 버프에 의한 데미지 증가량이 고려되어 있지 않습니다.
        //// 나의 공격력 감소버프가 있다면 적용하고 버프를 끕니다.
        ///
        if (UI_DMGDown == true)
        {
            m_Dmg = (int)((m_Scene.GetGameplayTurn().GetSkillBaseDamage() * m_Scene.GetGameplayTurn().GetPatternDamageMultiplier() + m_Scene.GetGameplayTurn().GetSpellExtraDamage()) * 0.7);
            UI_DMGDown = false;
        }
        else
        {
            m_Dmg = (int)(m_Scene.GetGameplayTurn().GetSkillBaseDamage() * m_Scene.GetGameplayTurn().GetPatternDamageMultiplier() + m_Scene.GetGameplayTurn().GetSpellExtraDamage());
        }

        SetTotalDamageUI();
        
        // 속성 봉인이 있었다면 그것을 해제합니다.
        OnChangedLockedElement.Invoke(0);
        UI_ElementLock = false;
        m_Scene.GetGameplayTurn().GetSelectSkillPhase().OnElementChanged.Invoke(EElementType.None);
        yield return new WaitForSeconds(1.5f);
        m_Scene.GetTotalDamageUI().SetActive(false);

        // 공격 결과 정보를 서버에다가 보내자.
        MSG_C2S_ATTACK_INFO((int)selectedSkill, m_Dmg);
        Debug.Log("선택한 스킬 = " + selectedSkill + " 데미지 = " + m_Dmg);

        // 턴 종료는 Attack_Result에서 체력, 마나 수치 다 변경하고 난 후에 하자.
    }

    // 2중 감전 효과를 재대로 보기 위해 필요한 딜레이를 위한 코루틴
    IEnumerator DoubleElectric(float time)
    {
        // 첫번째 감전
        AddHP(m_Scene.GetSkillInfoFinder().Find(EElementType.Light, ESkillType.Second).ExtraDamage * -1);
        m_PhotonView.RPC("rpcAni", RpcTarget.All, "Damaged_Elec");
        AddElectricCount(-1); // 중첩 감소

        yield return new WaitForSeconds(time);

        // 두번째 감전
        AddHP(m_Scene.GetSkillInfoFinder().Find(EElementType.Light, ESkillType.Second).ExtraDamage * -1);
        m_PhotonView.RPC("rpcAni", RpcTarget.All, "Damaged_Elec");
        AddElectricCount(-1);

        yield return new WaitForSeconds(time);

        // 턴 시작cv
        // m_bIsMyTurn = true;
    }

    IEnumerator OneElectric(float time)
    {
        AddHP(m_Scene.GetSkillInfoFinder().Find(EElementType.Light, ESkillType.Second).ExtraDamage * -1);
        m_PhotonView.RPC("rpcAni", RpcTarget.All, "Damaged_Elec");
        AddElectricCount(-1); // 중첩 감소

        yield return new WaitForSeconds(time);
    }

    public void On_Start_Turn(bool myTurn)
    {
        // UI 정보를 갱신합니다.
        MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

        m_bIsMyTurn = myTurn;

        OnStartTurn.Invoke();

        if (myTurn == true)
        {
            // 감전을 처리하자. 이펙트도 잘 넣어주세요
            // 독이 중첩되어있으면
            if (m_ElectricCount > 3)
            {
                StartCoroutine(DoubleElectric(3.0f));
            }
            else if (m_ElectricCount > 0)
            {
                StartCoroutine(OneElectric(3.0f));
            }

            // 체력 정보를 확인하여 체력이 0 이라면 전투 종료 메시지를 보냅니다.
            if (m_HP <= 0)
            {
                // 정보를 갱신합니다.
                MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);
                OnHPIsZero.Invoke();
                MSG_C2S_HP_ZERO();
            }
            else
            {
                // 턴 시작 시 마나를 회복합니다.
                AddMP(RecoveryManaPerTurn);

                // 턴을 시작하며 회복한 마나 정보를 갱신합니다.
                MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

                StartCoroutine(MyTurn());
                Debug.Log("My Turn Start");
            }
        }
        else
        {
            // UI 정보를 갱신합니다.
            MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

            StartCoroutine(EnemyTurn());
            Debug.Log("Enemy Turn Start");
        }

    }

    // 공격을 하거나 공격을 맞는 것의 체력 마나 변화를 여기서 처리한다.
    public IEnumerator On_Attack_Result(int attackerId, int skillInfo, int totalDMG)
    {
        // UI 정보를 갱신합니다.
        MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

        EElementType _elemType = (EElementType)((int)skillInfo / 10);
        ESkillType _skillType = (ESkillType)((int)skillInfo % 10);
        var _skillInfo = m_Scene.GetSkillInfoFinder().Find(_elemType, _skillType);

        Debug.Log("Attack_Result :: 내 ID : " + m_PhotonView.ViewID + " 공격자 : " + attackerId + " 스킬정보 : " + skillInfo + " 최종 데미지 : " + totalDMG);


        // 내가 공격자면 팔을 내리는 애니메이션을 재생합니다.
        if (m_PhotonView.ViewID == attackerId)
        {
            m_PhotonView.RPC("rpcAni", RpcTarget.All, "Attack_Down");
        }

        float _skillTime = PlaySkillEffect((Skills)skillInfo, attackerId);

        string _gender;
        AudioClip _audioClip;

        if (m_PhotonView.ViewID == attackerId)
        {
            _gender = m_bIsGirl ? "Girl" : "Male";
            _audioClip = m_bIsGirl ? _skillInfo.SfxGirlVoice : _skillInfo.SfxMaleVoice;
        }

        else
        {
            _gender = m_bIsGirl ? "Male" : "Girl";
            _audioClip = m_bIsGirl ? _skillInfo.SfxMaleVoice : _skillInfo.SfxGirlVoice;
        }

        SoundManager.instance.SFXPlay($"Sfx_SkillVoice_{_skillInfo}_{_gender}", _audioClip);

        Debug.Log("스킬 시전시간 : " + _skillTime);
        yield return new WaitForSeconds(_skillTime);


        // 나의 공격에 의해 발생한 이벤트일 때에만 처리하도록 필터링합니다.
        if (m_PhotonView.ViewID == attackerId)
        {

            if (m_SecondPlayer == true)
            {
                AddMP((_skillInfo.UsedMP * -1) / 2);
                m_SecondPlayer = false;
            }
            else
            {
                AddMP((_skillInfo.UsedMP * -1));
            }

            AddMP(_skillInfo.HealMP);
            AddHP(_skillInfo.HealHP);

        }
        // 상대의 공격에 대한 이벤트를 처리합니다.
        else
        {
            // 나의 방어력이 떨어져 있다면 더 강하게 맞습니다.
            if (UI_DefenseDown == true)
            {
                AddHP((int)(totalDMG * 1.2) * -1);
                UI_DefenseDown = false;
            }
            else
            {
                AddHP(totalDMG * -1);
            }

            // 피격 애니메이션을 재생합니다.
            if (_elemType != EElementType.Water)
            {
                m_PhotonView.RPC("rpcAni", RpcTarget.All, "Damaged");
            }

            if (_elemType == EElementType.Light)
            {
                // 속성 봉인
                if (_skillType == ESkillType.First)
                {
                    OnChangedLockedElement.Invoke(Random.Range(1, 5));
                    UI_ElementLock = true;
                }

                // 감전 스택을 추가합니다.
                if (_skillType == ESkillType.Second)
                {
                    m_ElectricCount = Mathf.Clamp(m_ElectricCount + 3, 0, 6);
                }
            }


            // => 나의 UI에 디버프를 표시합니다.
            if (_elemType == EElementType.Dark)
            {
                // 나의 다음 공격력 30% 감소합니다.
                if (_skillType == ESkillType.First)
                {
                    UI_DMGDown = true;
                }

                // 나의 방어력이 20% 감소합니다.
                if (_skillType == ESkillType.Second)
                {
                    UI_DefenseDown = true;
                }
            }
        }


        // 전투 이후의 나의 체력, 마나 정보를 갱신합니다.
        MSG_C2S_PLAYER_STATE(m_HP, m_MP, m_ElectricCount, UI_ElementLock, UI_DMGDown, UI_DefenseDown);

        Debug.Log("Client view ID :" + m_PhotonView.ViewID + "나의 체력 : " + m_HP + " 나의 마나 : " + m_MP);

        ResetAll();

        // 턴을 넘기기 전에 이펙트와 애니메이션이 끝나길 기다립니다.
        yield return new WaitForSeconds(3);

        // 체력 정보를 확인하여 체력이 0 이라면 전투 종료 메시지를 보냅니다.
        if (m_HP <= 0)
        {
            OnHPIsZero.Invoke();
            MSG_C2S_HP_ZERO();
            // 새로운 턴이 시작되면 안되므로 서버로부터 전투 종료 메시지를 받을 시간을 확보합니다.
            yield return new WaitForSeconds(3);
        }

        // 공,수 처리가 끝났으니 턴을 종료한다.
        MSG_C2S_ENDTURN();
    }

    private IEnumerator EnemyTurn()
    {
        // enemy turn 이미지를 보여준 후 서서히 숨깁니다.
        m_Scene.GetEnemyTurnImage().gameObject.SetActive(true);
        m_Scene.GetEnemyTurnImage().color = Color.white;
        var _tweener = m_Scene.GetEnemyTurnImage().DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 2.0f);
        yield return _tweener.WaitForCompletion();
        m_Scene.GetEnemyTurnImage().gameObject.SetActive(false);
    }

    private IEnumerator On_GAME_START()
    {
        // Ready 애니메이션을 재생합니다.
        m_PhotonView.RPC("rpcAni", RpcTarget.All, "Ready");

        // 초반부가 너무 빨라서 임시로 기다리는 시간을 넣었습니다.
        yield return new WaitForSeconds(3);

        // ready, fight 이미지가 순서대로 나타났다 사라집니다.
        m_Scene.GetReadyImage().gameObject.SetActive(true);
        m_Scene.GetFightImage().gameObject.SetActive(true);

        m_Scene.GetReadyImage().color = Color.white;
        m_Scene.GetFightImage().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(m_Scene.GetReadyImage().DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 2.0f))
            .AppendCallback(() => OnReady.Invoke())
            .Append(m_Scene.GetFightImage().DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 2.0f))
            .AppendCallback(() => OnFight.Invoke());



        yield return mySequence.WaitForCompletion();

        m_Scene.GetReadyImage().gameObject.SetActive(false);
        m_Scene.GetFightImage().gameObject.SetActive(false);
    }

    private IEnumerator On_GAME_SET(int _losePlayer)
    {
        bool m_isWin = false;
        if (m_PhotonView.ViewID == _losePlayer)
        {
            Debug.Log("Client view ID :" + m_PhotonView.ViewID + " 패배 ㅠㅠ");
            m_isWin = false;
        }
        else
        {
            Debug.Log("Client view ID :" + m_PhotonView.ViewID + " 승리!!!!");
            m_isWin = true;
        }

        SoundManager.instance.BGMStop();

        OnGameSet.Invoke();

        // 게임의 완전 종료 처리를 위해 서버에게 알린다. (사실 꼭 필요한건 아님)
        MSG_C2S_END_GAME_OK();

        if (m_bIsGirl == true)
        {
            if (m_isWin == true)
            {
                Debug.Log("LoadScene 1");
                SceneManager.LoadScene("Result_G_Win");
            }
            else if (m_isWin == false)
            {
                Debug.Log("LoadScene 2");
                SceneManager.LoadScene("Result_M_Win");
            }
        }
        else if (m_bIsGirl == false)
        {
            if (m_isWin == true)
            {
                Debug.Log("LoadScene 3");
                SceneManager.LoadScene("Result_M_Win");
            }
            else if (m_isWin == false)
            {
                Debug.Log("LoadScene 4");
                SceneManager.LoadScene("Result_G_Win");
            }
        }

        yield return null;
    }
}