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
            // 둘 다 접속하면
            if (GameFlow == 0 && player2ID != 0)
            {
                GameFlow++;        // 0->1
                Debug.Log("Manager :: 게임 준비 확인 메시지를 보낸다. " + GameFlow);

                MSG_S2C_GAME_READY();
            }

            // 두 플레이어가 준비되면 게임 시작을 알림
            if (GameFlow == 3)
            {
                GameFlow++;        // 3->4
                Debug.Log("Manager :: 게임 시작을 선언한다... " + GameFlow);

                MSG_S2C_GAME_START();
            }

            // 선 플레이어를 정함
            if (GameFlow == 4)
            {
                GameFlow++;        // 4->5

                Debug.Log("Manager :: 선 플레이어를 정하고 알려주자." + GameFlow);

                int rand = Random.Range(1, 101);
                if (rand % 2 == 0)
                    turnPlayer = player1ID;
                else if (rand % 2 == 1)
                    turnPlayer = player2ID;

                Debug.Log("Manager :: 선 플레이어는 " + turnPlayer);

                MSG_S2C_ANNOUNCE_STARTPLAYER(turnPlayer);
            }

            // 새로운 턴의 시작
            if (GameFlow >= 7 && turnEnd == true && gameover == false)
            {
                GameFlow++;        // 8+
                Debug.Log("Manager :: 지금 턴인 플레이어는 " + turnPlayer + " 입니다. 턴을 시작해주세요. " + GameFlow);

                MSG_S2C_START_TURN(turnPlayer);

                turnEnd = false;
            }

            // 누군가의 체력이 다 떨어져서 게임이 끝남.
            if (gameover == true && endgame == false)
            {
                GameFlow++;

                Debug.Log("Manager :: ViewID : " + losePlayer + "플레이어가 패배. 패배 알림 전송" + GameFlow);

                MSG_S2C_ENDGAME(losePlayer);

                endgame = true;
            }
        }
    }

    #endregion

    #region Test method

    // 업데이트로 서버 로직 동작 이동
    //IEnumerator Test()
    //{
    //    while (true)
    //    {
    //        //yield return new WaitForSeconds(1.0f);
    //        //yield return null;
    //        if (photonview.IsMine)
    //        {
    //            // 둘 다 접속하면
    //            if (GameFlow == 0 && player2ID != 0)
    //            {
    //                GameFlow++;        // 0->1
    //                Debug.Log("Manager :: 게임 준비 확인 메시지를 보낸다. " + GameFlow);

    //                MSG_S2C_GAME_READY();
    //            }

    //            // 두 플레이어가 준비되면 게임 시작을 알림
    //            if (GameFlow == 3)
    //            {
    //                GameFlow++;        // 3->4
    //                Debug.Log("Manager :: 게임 시작을 선언한다... " + GameFlow);

    //                MSG_S2C_GAME_START();
    //            }

    //            // 선 플레이어를 정함
    //            if (GameFlow == 4)
    //            {
    //                GameFlow++;        // 4->5

    //                // 딜레이 5초
    //                //yield return new WaitForSeconds(5.0f);

    //                Debug.Log("Manager :: 선 플레이어를 정하고 알려주자." + GameFlow);

    //                int rand = Random.Range(1, 101);
    //                if (rand % 2 == 0)
    //                    turnPlayer = player1ID;
    //                else if (rand % 2 == 1)
    //                    turnPlayer = player2ID;

    //                Debug.Log("Manager :: 선 플레이어는 " + turnPlayer);

    //                MSG_S2C_ANNOUNCE_STARTPLAYER(turnPlayer);
    //            }

    //            // 새로운 턴의 시작
    //            if (GameFlow >= 7 && turnEnd == true && gameover == false)
    //            {
    //                GameFlow++;        // 8+
    //                Debug.Log("Manager :: 지금 턴인 플레이어는 " + turnPlayer + " 입니다. 턴을 시작해주세요. " + GameFlow);

    //                MSG_S2C_START_TURN(turnPlayer);

    //                turnEnd = false;
    //            }

    //            // 누군가의 체력이 다 떨어져서 게임이 끝남.
    //            if (gameover == true && endgame == false)
    //            {
    //                GameFlow++;

    //                Debug.Log("Manager :: ViewID : " + losePlayer + "플레이어가 패배. 패배 알림 전송" + GameFlow);

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

    // 이벤트를 수신하는 부분
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
                    Debug.Log("Manager :: 전송한 View ID Player1으로. ViewID : " + senderViewID + " Player1 : " + player1ID);
                    MSG_S2C_CONNECT_MANAGER_OK(player1ID);
                }
                else
                {
                    player2ID = senderViewID;
                    Debug.Log("Manager :: 전송한 View ID Player2로. ViewID : " + senderViewID + " Player1 : " + player2ID);
                    MSG_S2C_CONNECT_MANAGER_OK(player2ID);
                }
            }

            if (eventCode == (byte)NetworkCode.C2S_GAME_READY_OK)
            {
                if (senderViewID == player1ID)
                {
                    GameFlow++;        // 1->2
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 게임 준비 완료  " + GameFlow);

                }
                else if (senderViewID == player2ID)
                {
                    GameFlow++;        // 2->3
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p2 게임 준비 완료  " + GameFlow);

                }

            }

            if (eventCode == (byte)NetworkCode.C2S_ANNOUNCE_STARTPLAYER_OK)
            {
                bool recvData = (bool)data[1];

                if (senderViewID == player1ID)
                {
                    GameFlow++;        // 5->6
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 선 플레이어정보" + recvData + " 수신 완료  " + GameFlow);

                }
                else if (senderViewID == player2ID)
                {
                    GameFlow++;        // 6->7
                    Debug.Log("Manager :: ViewID : " + senderViewID + "p1 선 플레이어정보" + recvData + " 수신 완료  " + GameFlow);
                }
            }

            if (eventCode == (byte)NetworkCode.C2S_ATTACK_INFO)
            {
                int skillID = (int)data[1];
                int totalDMG = (int)data[2];

                if (senderViewID == turnPlayer)
                {
                    Debug.Log("Manager :: ViewID : " + senderViewID + "가 공격 정보를 보냄. 지금 턴은 " + turnPlayer + " 사용한 스킬 " + skillID + " 데미지 " + totalDMG);

                    MSG_S2C_ATTACK_RESULT(turnPlayer, skillID, totalDMG);
                }
                else
                {
                    Debug.Log("Manager :: ViewID : " + senderViewID + "가 공격 정보를 보냄. 지금 턴은 " + turnPlayer + " 임. 문제 발생...!!");
                }
            }



            if (eventCode == (byte)NetworkCode.C2S_END_TURN)
            {

                if (turnEndPlayerCheck == false)
                {
                    Debug.Log("Manager :: 한 플레이어가 턴을 끝났습니다. ");
                    // 첫 플레이어가 턴을 끝냈음을 알림
                    turnEndPlayerCheck = true;
                }
                else
                {
                    Debug.Log("Manager :: 두번째 플레이어가 턴을 끝났습니다. ");
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
                Debug.Log("Manager :: ViewID : " + losePlayer + "플레이어의 체력이 없습니다." + GameFlow);

                gameover = true;
            }

            if (eventCode == (byte)NetworkCode.C2S_ENDGAME_OK)
            {
                // 양쪽 클라가 모두 게임이 끝나는 처리를 했음을 알림.
                Debug.Log("모든 게임 로직 종료!!!");
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

