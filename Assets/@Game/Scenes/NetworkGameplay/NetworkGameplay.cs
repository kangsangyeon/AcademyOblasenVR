using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkGameplay : MonoBehaviour, IOnEventCallback, IPunObservable
{
    struct NetworkPlayer
    {
        public int playerIndex;
        public int viewID;

        public override string ToString()
        {
            return $"Player{playerIndex}(viewId: {viewID})";
        }
    }

    [SerializeField] private GameplayScene m_Scene;

    private NetworkPlayer[] m_NetworkPlayers = new NetworkPlayer[2];
    private int m_PlayerCount = 0;

    private NetworkPlayer GetNetworkPlayerByIndex(int index) => m_NetworkPlayers.First(np => np.playerIndex == index);
    private NetworkPlayer GetNetworkPlayerById(int id) => m_NetworkPlayers.First(np => np.viewID == id);

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
        if (PhotonNetwork.IsMasterClient == false)
        {
            // 마스터 클라이언트(=호스트)가 아닌 경우, 아무런 네트워크 이벤트도 발생시키지 않도록 필터링합니다.
            // 이는, 게임에서 중요한 메인 로직을 한 컴퓨터에서만 실행되도록 하기 위함입니다.
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

        if (eventCode == (byte)NetworkCode.C2S_CONNECT_MANAGER)
        {
            ++m_PlayerCount;
            int _index = m_PlayerCount - 1;

            m_NetworkPlayers[_index].playerIndex = _index;
            m_NetworkPlayers[_index].viewID = senderViewID;

            MSG_S2C_CONNECT_MANAGER_OK(senderViewID, _index);
            Debug.Log("Manager :: Player index 정보 전송. ViewID : " + senderViewID + " _index : " + _index);
        }

        if (eventCode == (byte)NetworkCode.C2S_GAME_READY_OK)
        {
            NetworkPlayer _player = m_NetworkPlayers.First(np => np.viewID == senderViewID);
            Debug.Log($"Manager :: ViewID : {_player.ToString()} 게임 준비 완료  ");
        }

        if (eventCode == (byte)NetworkCode.C2S_ATTACK_INFO)
        {
            int skillID = (int)data[1];
            int totalDMG = (int)data[2];

            if (GetNetworkPlayerById(senderViewID).playerIndex == m_TurnPlayerIndex)
            {
                Debug.Log("Manager :: ViewID : " + senderViewID + "가 공격 정보를 보냄. 지금 턴은 " + m_TurnPlayerIndex + " 사용한 스킬 " + skillID + " 데미지 " + totalDMG);

                MSG_S2C_ATTACK_RESULT(senderViewID, skillID, totalDMG);
            }
            else
            {
                Debug.Log("Manager :: ViewID : " + senderViewID + "가 공격 정보를 보냄. 지금 턴은 " + m_TurnPlayerIndex + " 임. 문제 발생...!!");
            }
        }


        if (eventCode == (byte)NetworkCode.C2S_END_TURN)
        {
            ++m_TurnEndPlayerCount;

            NetworkPlayer _player = m_NetworkPlayers.First(np => np.viewID == senderViewID);
            Debug.Log($"Manager :: {_player.ToString()}가 턴을 끝났습니다. ");

            if (m_TurnEndPlayerCount < m_PlayerCount)
                return;

            turnEnd = true;
        }

        if (eventCode == (byte)NetworkCode.C2S_HP_ZERO)
        {
            losePlayerId = senderViewID;
            Debug.Log("Manager :: ViewID : " + losePlayerId + "플레이어의 체력이 없습니다.");

            m_gameover = true;
        }

        if (eventCode == (byte)NetworkCode.C2S_ENDGAME_OK)
        {
            // 양쪽 클라가 모두 게임이 끝나는 처리를 했음을 알림.
            Debug.Log("모든 게임 로직 종료!!!");
        }
    }

    private IEnumerator Start()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            yield break;
        }

        PhotonNetwork.CurrentRoom.MaxPlayers = 2;

        // 플레이어 2명이 채워질 때까지 대기한 뒤 마저 초기화합니다.
        yield return StartCoroutine(WaitUntilAllPlayerJoin());

        // 모든 플레이어가 준비 완료될 때까지 기다립니다.
        yield return StartCoroutine(WaitUntilPlayerReady());

        // 모든 플레이어가 준비되었다는 사실을 모두에게 알립니다.
        yield return StartCoroutine(AnnounceGameStart());

        // 선턴 플레이어가 누구인지 모두에게 알립니다.
        AnnounceStartPlayer();

        // 전투를 시작하고, 전투가 끝날 때까지 기다립니다.
        yield return StartCoroutine(StartBattleAndWaitUntilGameOver());

        // 모든 플레이어에게 패배한 플레이어가 누구인지 알립니다.
        AnnounceGameOver();
    }

    private IEnumerator WaitUntilAllPlayerJoin()
    {
        while (true)
        {
            if (m_PlayerCount == 2)
                break;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator WaitUntilPlayerReady()
    {
        Debug.Log("Manager :: 게임 준비 확인 메시지를 보낸다. ");

        MSG_S2C_GAME_READY();

        yield return null;
    }

    private IEnumerator AnnounceGameStart()
    {
        Debug.Log("Manager :: 게임 시작을 선언한다... ");
        MSG_S2C_GAME_START();
        m_firstTurn = true;

        yield return new WaitForSeconds(5.0f);
    }

    private void AnnounceStartPlayer()
    {
        Debug.Log("Manager :: 선 플레이어를 정하고 알려주자.");

        int rand = Random.Range(0, 10000);
        m_TurnPlayerIndex = rand % 2;

        Debug.Log($"Manager :: 선 플레이어는 {m_NetworkPlayers[m_TurnPlayerIndex].ToString()}");

        MSG_S2C_ANNOUNCE_STARTPLAYER(m_NetworkPlayers[m_TurnPlayerIndex].viewID);
    }

    private IEnumerator StartBattleAndWaitUntilGameOver()
    {
        turnEnd = true;

        while (true)
        {
            if (m_gameover == true)
                break;

            if (turnEnd == true)
            {
                // TODO :: 첫 턴에는 동작하면 안된다.
                if (m_firstTurn == false)
                {
                    m_TurnPlayerIndex = (m_TurnPlayerIndex + 1) % m_PlayerCount;
                    m_TurnEndPlayerCount = 0;
                }
                // 첫 턴 이후에 적용한다.
                m_firstTurn = false;

                var player = GetNetworkPlayerByIndex(m_TurnPlayerIndex);
                Debug.Log($"Manager :: 지금 턴인 플레이어는 {player.ToString()} 입니다.");

                MSG_S2C_START_TURN(player.viewID);

                turnEnd = false;
            }

            yield return null;
        }
    }

    private void AnnounceGameOver()
    {
        Debug.Log("Manager :: ViewID : " + losePlayerId + "플레이어가 패배. 패배 알림 전송");

        MSG_S2C_ENDGAME(losePlayerId);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(m_TurnPlayerIndex);
            stream.SendNext(losePlayerId);

            Resources.Load<GameObject>("DebugUI");
        }
        else
        {
            // Network player, receive data
            this.m_TurnPlayerIndex = (int)stream.ReceiveNext();
            this.losePlayerId = (int)stream.ReceiveNext();
        }
    }

    ///
    ///
    ///
    bool turnEnd = true;
    private int m_TurnEndPlayerCount = 0;
    bool m_firstTurn = false;
    bool m_gameover = false;

    private int m_TurnPlayerIndex = 0;
    private int losePlayerId = 0;

    #region Network Event MSG

    private void MSG_S2C_CONNECT_MANAGER_OK(int playerID, int playerIndex)
    {
        object[] content = new object[] { playerID, playerIndex };
        RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

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