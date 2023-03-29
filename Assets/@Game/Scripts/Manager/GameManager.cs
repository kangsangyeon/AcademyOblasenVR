using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviour, IOnEventCallback, IPunObservable
{
    #region private variable

    private static int NetID = 1;
    bool _event = true;
    bool turnEnd = true;
    bool turnEndPlayerCheck = false;
    bool gameover = false;
    bool endgame = false;

    #endregion

    #region public variables

    public PhotonView photonview;

    #endregion

    #region Network Serialize Variables

    int player1ID = 0;
    int player2ID = 0;
    int turnPlayer = 0;
    int losePlayer = 0;

    #endregion

    #region Network Serialize Method
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(player1ID);
            stream.SendNext(player2ID);
            stream.SendNext(turnPlayer);
            stream.SendNext(losePlayer);
        }
        else
        {
            // Network player, receive data
            this.player1ID = (int)stream.ReceiveNext();
            this.player2ID = (int)stream.ReceiveNext();
            this.turnPlayer = (int)stream.ReceiveNext();
            this.losePlayer = (int)stream.ReceiveNext();
        }

    }

    #endregion

    #region Test Variables


    #endregion

    #region Unity Callback

    // Start is called before the first frame update
    void Start()
    {
        //if (photonview.IsMine)
        //{
        //    StartCoroutine(Test());
        //}
    }

    // Update is called once per frame
    int GameFlow = 0;

    void Update()
    {
        if (photonview.IsMine)
        {
            // �� �� �����ϸ�
            if (GameFlow == 0 && player2ID != 0)
            {
                GameFlow++;        // 0->1
                Debug.Log("Manager :: ���� �غ� Ȯ�� �޽����� ������. " + GameFlow);

                MSG_S2C_GAME_READY();
            }

            // �� �÷��̾ �غ�Ǹ� ���� ������ �˸�
            if (GameFlow == 3)
            {
                GameFlow++;        // 3->4
                Debug.Log("Manager :: ���� ������ �����Ѵ�... " + GameFlow);

                MSG_S2C_GAME_START();
            }

            // �� �÷��̾ ����
            if (GameFlow == 4)
            {
                GameFlow++;        // 4->5

                Debug.Log("Manager :: �� �÷��̾ ���ϰ� �˷�����." + GameFlow);

                int rand = Random.Range(1, 101);
                if (rand % 2 == 0)
                    turnPlayer = player1ID;
                else if (rand % 2 == 1)
                    turnPlayer = player2ID;

                Debug.Log("Manager :: �� �÷��̾�� " + turnPlayer);

                MSG_S2C_ANNOUNCE_STARTPLAYER(turnPlayer);
            }

            // ���ο� ���� ����
            if (GameFlow >= 7 && turnEnd == true && gameover == false)
            {
                GameFlow++;        // 8+
                Debug.Log("Manager :: ���� ���� �÷��̾�� " + turnPlayer + " �Դϴ�. ���� �������ּ���. " + GameFlow);

                MSG_S2C_START_TURN(turnPlayer);

                turnEnd = false;
            }

            // �������� ü���� �� �������� ������ ����.
            if (gameover == true && endgame == false)
            {
                GameFlow++;

                Debug.Log("Manager :: ViewID : " + losePlayer + "�÷��̾ �й�. �й� �˸� ����" + GameFlow);

                MSG_S2C_ENDGAME(losePlayer);

                endgame = true;
            }
        }
    }

    #endregion

    #region Test method

    // ������Ʈ�� ���� ���� ���� �̵�
    //IEnumerator Test()
    //{
    //    while (true)
    //    {
    //        //yield return new WaitForSeconds(1.0f);
    //        //yield return null;
    //        if (photonview.IsMine)
    //        {
    //            // �� �� �����ϸ�
    //            if (GameFlow == 0 && player2ID != 0)
    //            {
    //                GameFlow++;        // 0->1
    //                Debug.Log("Manager :: ���� �غ� Ȯ�� �޽����� ������. " + GameFlow);

    //                MSG_S2C_GAME_READY();
    //            }

    //            // �� �÷��̾ �غ�Ǹ� ���� ������ �˸�
    //            if (GameFlow == 3)
    //            {
    //                GameFlow++;        // 3->4
    //                Debug.Log("Manager :: ���� ������ �����Ѵ�... " + GameFlow);

    //                MSG_S2C_GAME_START();
    //            }

    //            // �� �÷��̾ ����
    //            if (GameFlow == 4)
    //            {
    //                GameFlow++;        // 4->5

    //                // ������ 5��
    //                //yield return new WaitForSeconds(5.0f);

    //                Debug.Log("Manager :: �� �÷��̾ ���ϰ� �˷�����." + GameFlow);

    //                int rand = Random.Range(1, 101);
    //                if (rand % 2 == 0)
    //                    turnPlayer = player1ID;
    //                else if (rand % 2 == 1)
    //                    turnPlayer = player2ID;

    //                Debug.Log("Manager :: �� �÷��̾�� " + turnPlayer);

    //                MSG_S2C_ANNOUNCE_STARTPLAYER(turnPlayer);
    //            }

    //            // ���ο� ���� ����
    //            if (GameFlow >= 7 && turnEnd == true && gameover == false)
    //            {
    //                GameFlow++;        // 8+
    //                Debug.Log("Manager :: ���� ���� �÷��̾�� " + turnPlayer + " �Դϴ�. ���� �������ּ���. " + GameFlow);

    //                MSG_S2C_START_TURN(turnPlayer);

    //                turnEnd = false;
    //            }

    //            // �������� ü���� �� �������� ������ ����.
    //            if (gameover == true && endgame == false)
    //            {
    //                GameFlow++;

    //                Debug.Log("Manager :: ViewID : " + losePlayer + "�÷��̾ �й�. �й� �˸� ����" + GameFlow);

    //                MSG_S2C_ENDGAME(losePlayer);

    //                endgame = true;
    //            }
    //        }
    //    }
    //}

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

    // �̺�Ʈ�� �����ϴ� �κ�
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;
        int senderViewID = (int)data[0];

        if (photonview.IsMine)
        {
            if (eventCode == (byte)NetworkCode.C2S_CONNECT_MANAGER)
            {
                if (player1ID == 0)
                {
                    player1ID = senderViewID;
                    Debug.Log("Manager :: ������ View ID Player1����. ViewID : " + senderViewID + " Player1 : " + player1ID);
                    MSG_S2C_CONNECT_MANAGER_OK(player1ID);
                }
                else
                {
                    player2ID = senderViewID;
                    Debug.Log("Manager :: ������ View ID Player2��. ViewID : " + senderViewID + " Player1 : " + player2ID);
                    MSG_S2C_CONNECT_MANAGER_OK(player2ID);
                }
            }

            if (eventCode == (byte)NetworkCode.C2S_GAME_READY_OK)
            {
                if (senderViewID == player1ID)
                {
                    GameFlow++;        // 1->2
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 ���� �غ� �Ϸ�  " + GameFlow);

                }
                else if (senderViewID == player2ID)
                {
                    GameFlow++;        // 2->3
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p2 ���� �غ� �Ϸ�  " + GameFlow);

                }

            }

            if (eventCode == (byte)NetworkCode.C2S_ANNOUNCE_STARTPLAYER_OK)
            {
                bool recvData = (bool)data[1];

                if (senderViewID == player1ID)
                {
                    GameFlow++;        // 5->6
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 �� �÷��̾�����" + recvData + " ���� �Ϸ�  " + GameFlow);

                }
                else if (senderViewID == player2ID)
                {
                    GameFlow++;        // 6->7
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 �� �÷��̾�����" + recvData + " ���� �Ϸ�  " + GameFlow);
                }
            }

            if (eventCode == (byte)NetworkCode.C2S_ATTACK_INFO)
            {
                int skillID = (int)data[1];
                int totalDMG = (int)data[2];

                if (senderViewID == turnPlayer)
                {
                    Debug.Log("Manager :: ViewID : " + senderViewID + "�� ���� ������ ����. ���� ���� " + turnPlayer + " ����� ��ų " + skillID + " ������ " + totalDMG);

                    MSG_S2C_ATTACK_RESULT(turnPlayer, skillID, totalDMG);
                }
                else
                {
                    Debug.Log("Manager :: ViewID : " + senderViewID + "�� ���� ������ ����. ���� ���� " + turnPlayer + " ��. ���� �߻�...!!");
                }
            }



            if (eventCode == (byte)NetworkCode.C2S_END_TURN)
            {

                if (turnEndPlayerCheck == false)
                {
                    Debug.Log("Manager :: �� �÷��̾ ���� �������ϴ�. ");
                    // ù �÷��̾ ���� �������� �˸�
                    turnEndPlayerCheck = true;
                }
                else
                {
                    Debug.Log("Manager :: �ι�° �÷��̾ ���� �������ϴ�. ");
                    if (turnPlayer == player1ID)
                        turnPlayer = player2ID;
                    else if (turnPlayer == player2ID)
                        turnPlayer = player1ID;

                    turnEnd = true;
                    turnEndPlayerCheck = false;
                }
            }

            if (eventCode == (byte)NetworkCode.C2S_HP_ZERO)
            {
                GameFlow++;

                losePlayer = senderViewID;
                Debug.Log("Manager :: ViewID : " + losePlayer + "�÷��̾��� ü���� �����ϴ�." + GameFlow);

                gameover = true;
            }

            if (eventCode == (byte)NetworkCode.C2S_ENDGAME_OK)
            {
                // ���� Ŭ�� ��� ������ ������ ó���� ������ �˸�.
                Debug.Log("��� ���� ���� ����!!!");
            }
        }
    }
    #endregion

    #region Network Event MSG
    private void MSG_S2C_CONNECT_MANAGER_OK(int playerID)
    {
        object[] content = new object[] { playerID };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_CONNECT_MANAGER_OK, content, eventOptions);
    }

    private void MSG_S2C_GAME_READY()
    {
        object[] content = new object[] { 0 };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_GAME_READY, content, eventOptions);
    }

    private void MSG_S2C_GAME_START()
    {
        object[] content = new object[] { 0 };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_GAME_START, content, eventOptions);
    }

    private void MSG_S2C_ANNOUNCE_STARTPLAYER(int turnPlayer)
    {
        object[] content = new object[] { 0, turnPlayer };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_ANNOUNCE_STARTPLAYER, content, eventOptions);
    }

    private void MSG_S2C_START_TURN(int turnPlayer)
    {
        object[] content = new object[] { 0, turnPlayer };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_START_TURN, content, eventOptions);
    }


    private void MSG_S2C_ATTACK_RESULT(int attackPlayer, int skillID, int totalDMG)
    {
        object[] content = new object[] { attackPlayer, skillID, totalDMG };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_ATTACK_RESULT, content, eventOptions);
    }


    private void MSG_S2C_ENDGAME(int losePlayer)
    {
        object[] content = new object[] { 0, losePlayer };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

        MSGSender(NetworkCode.S2C_ENDGAME, content, eventOptions);
    }


    private void MSGSender(NetworkCode Code, object[] content, RaiseEventOptions options)
    {
        PhotonNetwork.RaiseEvent((byte)Code, content, options, SendOptions.SendReliable);
    }

    #endregion
}

