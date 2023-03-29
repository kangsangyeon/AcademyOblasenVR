using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class PlayerLogic : MonoBehaviour, IOnEventCallback
{
    #region Public Variables

    public PhotonView photonview;
    public int StartHP = 100;
    public int StartMP = 100;
    public int MaxHP = 100;
    public int MaxMP = 100;

    public PlayerManagerCoroutine _PlayerCorutine;

    #endregion

    #region Private Variables

    bool gameover = false;

    private int Electric_Count = 0;

    #endregion

    #region Getter Setter

    public int GetHP()
    {
        return StartHP;
    }
    // amount 만큼 감소합니다.
    public void DownHP(int amount)
    {
        StartHP -= amount;
    }
    // amount 만큼 증가합니다.
    public void UpHP(int amount)
    {
        StartHP += amount;

        if (StartHP > MaxHP)
        {
            StartHP = MaxHP;
        }
    }

    public int GetMP()
    {
        return StartMP;
    }
    // amount 만큼 감소합니다.
    public void DownMP(int amount)
    {
        StartMP -= amount;
    }
    // amount 만큼 증가합니다.
    public void UpMP(int amount)
    {
        StartMP += amount;

        if (StartMP > MaxMP)
        {
            StartMP = MaxMP;
        }
    }

    public int GetElectric_Count()
    {
        return Electric_Count;
    }
    public void ChangeElectric_Count(int amount)
    {
        Electric_Count += amount;

        // 스킬 한번에 두틱이 적용.
        // 틱 끼리는 중접 가능.
        // 스킬을 계속 쓰면 2틱 2틱 2틱 2틱...

        // 이것을 구현하기 위해서...
        // count가 3이면 한번에 2번 줄이고 2번 데미지를 받도록 하자.
    }

    #endregion

    #region Unity Callback

    // Start is called before the first frame update
    void Start()
    {
        if (photonview.IsMine == true)
        {
            MSG_C2S_CONNECT_MANAGER();
        }

        // StartCoroutine(Test());
    }

    int test_flow = 0;

    //IEnumerator Test()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1.0f);

    //        if (photonview.IsMine)
    //        {
    //            if (test_flow == 1)
    //            {
    //                test_flow++;    // 1->2
    //                Debug.Log("Client view ID :" + photonview.ViewID + " 게임 준비 확인 메시지 전송 " + test_flow);

    //                MSG_C2S_GAME_READY_OK();
    //            }

    //            if (isMyTurn)
    //            {
    //                yield return new WaitForSeconds(1.0f);
    //                Debug.Log("Client view ID :" + photonview.ViewID + " 40 데미지 전송 엠피는 5소모" + test_flow);

    //                ///////////////////////////////////////////////////////////////////////
    //                // 임시로 대충 넣어둠. 공격 메시지 수정 필요함
    //                MSG_C2S_ATTACK_INFO(11, 40);

    //                isMyTurn = false;

    //            }
    //        }
    //    }
    //}

    // Update is called once per frame

    void Update()
    {
        if (photonview.IsMine)
        {
            // 게임오버 상태가 아닌데 체력이 0이면...
            if (gameover == false && StartHP <= 0)
            {
                // 체력이 다 달았다고 알리고
                MSG_C2S_HP_ZERO();

                // 게임 종료 상태로 만들어서 여러번 호출을 막음
                gameover = true;
            }
        }
    }

    #endregion

    #region Network OnEvent

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonview.IsMine)
        {
            byte eventCode = photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;
            int viewID = (int)data[0];

            if (eventCode == (byte)NetworkCode.S2C_CONNECT_MANAGER_OK)
            {
                // 턴을 선택할 때 나를 위한 메시지인지 확인
                if (viewID == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "연결된 플레이어 ID : " + viewID + "같으면 연결 완료!");
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "연결된 플레이어 ID : " + viewID + "다른 플레이어 연결 완료");
                }
            }

            if (eventCode == (byte)NetworkCode.S2C_GAME_READY)
            {
                test_flow++;        // 0 -> 1
                Debug.Log("Client view ID :" + photonview.ViewID + "게임 준비 요청 메시지 수신 " + test_flow);

                _PlayerCorutine.S2C_Game_Ready();
            }

            if (eventCode == (byte)NetworkCode.S2C_GAME_START)
            {
                test_flow++;     // 2->3
                Debug.Log("Client view ID :" + photonview.ViewID + "게임 시작 메시지 수신 " + test_flow);

                _PlayerCorutine.S2C_Game_Start();
            }

            if (eventCode == (byte)NetworkCode.S2C_ANNOUNCE_STARTPLAYER)
            {
                test_flow++;    // 3->4
                int startPlayer = (int)data[1];

                Debug.Log("Client view ID :" + photonview.ViewID + "시작 플레이어 정보 :  " + startPlayer + " " + test_flow);

                if (startPlayer == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "내가 선공!!!");

                    _PlayerCorutine.S2C_Announce_StartPlayer(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "내가 후공...");

                    _PlayerCorutine.S2C_Announce_StartPlayer(false);
                }

                // MSG_C2S_ANNOUNCE_STARTPLAYER_OK(startPlayer);
            }

            if (eventCode == (byte)NetworkCode.S2C_START_TURN)
            {
                test_flow++;    // 5 +
                int turnPlayer = (int)data[1];

                Debug.Log("Client view ID :" + photonview.ViewID + "턴의 시작 정보 정보 :  " + turnPlayer + " " + test_flow);

                if (turnPlayer == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "나의 공격!!!!");
                    //isMyTurn = true;
                    _PlayerCorutine.S2C_Start_Turn(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "수비턴....");
                    //isMyTurn = false;
                    _PlayerCorutine.S2C_Start_Turn(false);
                }
            }

            if (eventCode == (byte)NetworkCode.S2C_ATTACK_RESULT)
            {
                int skillID = (int)data[1];
                int totalDMG = (int)data[2];

                // 내가 공격한거면
                if (viewID == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "스킬명 : " + skillID + " 로 때렸다...!" + "데미지 : " + totalDMG);

                    Debug.Log("Client view ID :" + photonview.ViewID + "나의 체력 : " + StartHP + " 나의 마나 : " + StartMP);

                    _PlayerCorutine.S2C_Attack_Result(true, skillID, totalDMG);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "스킬명 : " + skillID + " 로 맞았다 ㅠㅠㅠ" + "데미지 : " + totalDMG);

                    Debug.Log("Client view ID :" + photonview.ViewID + "나의 체력 : " + StartHP + " 나의 마나 : " + StartMP);

                    _PlayerCorutine.S2C_Attack_Result(false, skillID, totalDMG);
                }
            }


            if (eventCode == (byte)NetworkCode.S2C_ENDGAME)
            {
                // 게임이 종료되었음. 적당한 처리를 하고 종료 확인 메시지를 보내자.
                Debug.Log("Client view ID :" + photonview.ViewID + " 게임 종료 처리");

                int losePlayer = (int)data[1];

                if (photonview.ViewID == losePlayer)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + " 패배 ㅠㅠ");
                    _PlayerCorutine.S2C_EndGame(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + " 승리!!!!");
                    _PlayerCorutine.S2C_EndGame(false);
                }

                //MSG_C2S_END_GAME_OK();
            }
        }
    }

    #endregion

    #region Network Event MSG
    private void MSG_C2S_CONNECT_MANAGER()
    {
        object[] content = new object[] { photonview.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_CONNECT_MANAGER, content, eventOptions);
    }

    public void MSG_C2S_GAME_READY_OK()
    {
        object[] content = new object[] { photonview.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_GAME_READY_OK, content, eventOptions);
    }


    public void MSG_C2S_ANNOUNCE_STARTPLAYER_OK(bool isMyturn)
    {
        object[] content = new object[] { photonview.ViewID, isMyturn };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_ANNOUNCE_STARTPLAYER_OK, content, eventOptions);
    }

    public void MSG_C2S_ATTACK_INFO(int SkillNum, int TotalDMG)
    {
        object[] content = new object[] { photonview.ViewID, SkillNum, TotalDMG };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_ATTACK_INFO, content, eventOptions);
    }

    public void MSG_C2S_ENDTURN(int MyHP, int MyMP)
    {
        object[] content = new object[] { photonview.ViewID, MyHP, MyMP };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_END_TURN, content, eventOptions);
    }


    private void MSG_C2S_HP_ZERO()
    {
        object[] content = new object[] { photonview.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_HP_ZERO, content, eventOptions);
    }

    public void MSG_C2S_END_GAME_OK()
    {
        object[] content = new object[] { photonview.ViewID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.C2S_ENDGAME_OK, content, eventOptions);
    }


    private void MSGSender(NetworkCode Code, object[] content, RaiseEventOptions options)
    {
        PhotonNetwork.RaiseEvent((byte)Code, content, options, SendOptions.SendReliable);
    }

    #endregion

}


