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
    // amount ��ŭ �����մϴ�.
    public void DownHP(int amount)
    {
        StartHP -= amount;
    }
    // amount ��ŭ �����մϴ�.
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
    // amount ��ŭ �����մϴ�.
    public void DownMP(int amount)
    {
        StartMP -= amount;
    }
    // amount ��ŭ �����մϴ�.
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

        // ��ų �ѹ��� ��ƽ�� ����.
        // ƽ ������ ���� ����.
        // ��ų�� ��� ���� 2ƽ 2ƽ 2ƽ 2ƽ...

        // �̰��� �����ϱ� ���ؼ�...
        // count�� 3�̸� �ѹ��� 2�� ���̰� 2�� �������� �޵��� ����.
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
    //                Debug.Log("Client view ID :" + photonview.ViewID + " ���� �غ� Ȯ�� �޽��� ���� " + test_flow);

    //                MSG_C2S_GAME_READY_OK();
    //            }

    //            if (isMyTurn)
    //            {
    //                yield return new WaitForSeconds(1.0f);
    //                Debug.Log("Client view ID :" + photonview.ViewID + " 40 ������ ���� ���Ǵ� 5�Ҹ�" + test_flow);

    //                ///////////////////////////////////////////////////////////////////////
    //                // �ӽ÷� ���� �־��. ���� �޽��� ���� �ʿ���
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
            // ���ӿ��� ���°� �ƴѵ� ü���� 0�̸�...
            if (gameover == false && StartHP <= 0)
            {
                // ü���� �� �޾Ҵٰ� �˸���
                MSG_C2S_HP_ZERO();

                // ���� ���� ���·� ���� ������ ȣ���� ����
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
                // ���� ������ �� ���� ���� �޽������� Ȯ��
                if (viewID == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "����� �÷��̾� ID : " + viewID + "������ ���� �Ϸ�!");
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "����� �÷��̾� ID : " + viewID + "�ٸ� �÷��̾� ���� �Ϸ�");
                }
            }

            if (eventCode == (byte)NetworkCode.S2C_GAME_READY)
            {
                test_flow++;        // 0 -> 1
                Debug.Log("Client view ID :" + photonview.ViewID + "���� �غ� ��û �޽��� ���� " + test_flow);

                _PlayerCorutine.S2C_Game_Ready();
            }

            if (eventCode == (byte)NetworkCode.S2C_GAME_START)
            {
                test_flow++;     // 2->3
                Debug.Log("Client view ID :" + photonview.ViewID + "���� ���� �޽��� ���� " + test_flow);

                _PlayerCorutine.S2C_Game_Start();
            }

            if (eventCode == (byte)NetworkCode.S2C_ANNOUNCE_STARTPLAYER)
            {
                test_flow++;    // 3->4
                int startPlayer = (int)data[1];

                Debug.Log("Client view ID :" + photonview.ViewID + "���� �÷��̾� ���� :  " + startPlayer + " " + test_flow);

                if (startPlayer == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "���� ����!!!");

                    _PlayerCorutine.S2C_Announce_StartPlayer(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "���� �İ�...");

                    _PlayerCorutine.S2C_Announce_StartPlayer(false);
                }

                // MSG_C2S_ANNOUNCE_STARTPLAYER_OK(startPlayer);
            }

            if (eventCode == (byte)NetworkCode.S2C_START_TURN)
            {
                test_flow++;    // 5 +
                int turnPlayer = (int)data[1];

                Debug.Log("Client view ID :" + photonview.ViewID + "���� ���� ���� ���� :  " + turnPlayer + " " + test_flow);

                if (turnPlayer == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "���� ����!!!!");
                    //isMyTurn = true;
                    _PlayerCorutine.S2C_Start_Turn(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "������....");
                    //isMyTurn = false;
                    _PlayerCorutine.S2C_Start_Turn(false);
                }
            }

            if (eventCode == (byte)NetworkCode.S2C_ATTACK_RESULT)
            {
                int skillID = (int)data[1];
                int totalDMG = (int)data[2];

                // ���� �����ѰŸ�
                if (viewID == photonview.ViewID)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "��ų�� : " + skillID + " �� ���ȴ�...!" + "������ : " + totalDMG);

                    Debug.Log("Client view ID :" + photonview.ViewID + "���� ü�� : " + StartHP + " ���� ���� : " + StartMP);

                    _PlayerCorutine.S2C_Attack_Result(true, skillID, totalDMG);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + "��ų�� : " + skillID + " �� �¾Ҵ� �ФФ�" + "������ : " + totalDMG);

                    Debug.Log("Client view ID :" + photonview.ViewID + "���� ü�� : " + StartHP + " ���� ���� : " + StartMP);

                    _PlayerCorutine.S2C_Attack_Result(false, skillID, totalDMG);
                }
            }


            if (eventCode == (byte)NetworkCode.S2C_ENDGAME)
            {
                // ������ ����Ǿ���. ������ ó���� �ϰ� ���� Ȯ�� �޽����� ������.
                Debug.Log("Client view ID :" + photonview.ViewID + " ���� ���� ó��");

                int losePlayer = (int)data[1];

                if (photonview.ViewID == losePlayer)
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + " �й� �Ф�");
                    _PlayerCorutine.S2C_EndGame(true);
                }
                else
                {
                    Debug.Log("Client view ID :" + photonview.ViewID + " �¸�!!!!");
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


