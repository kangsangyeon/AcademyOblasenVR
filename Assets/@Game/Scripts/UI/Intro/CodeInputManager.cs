using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CodeInputManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [HideInInspector]
    public List<int> roomCode = new List<int>();
    [HideInInspector]
    public string mRoomCodestr;
    #endregion

    #region Private Fields

    private int result = 0;
    private TMP_Text mText;
    string gameVersion = "0.1";
    bool isConnecting;


    [SerializeField]
    public byte maxPlayersPerRoom = 2;

    #endregion

    #region Private Methods

    private void MakeCodeToString()
    {
        int count = roomCode.Count - 1;

        for (int i = 0; i < roomCode.Count; i++)
        {
            result += roomCode[i] * (int)Math.Pow(10, count);
            count -= 1;
        }

        mRoomCodestr = result.ToString();
    }

    private void ShowCode()
    {
        mText.text = mRoomCodestr;
        result = 0;
    }

    #endregion

    #region Public Methods

    public void EneterGame()
    {
        //Debug.Log("VR Intro : Called EnterGame, RoomNum is " + mRoomCodestr);

        //if (PhotonNetwork.IsConnected)
        //{
        //    RoomOptions roomOption = new RoomOptions() { IsVisible = false, MaxPlayers = maxPlayersPerRoom };
        //    PhotonNetwork.JoinOrCreateRoom(mRoomCodestr, roomOption, TypedLobby.Default);
        //}
        //else
        //{
        //    isConnecting = PhotonNetwork.ConnectUsingSettings();
        //    PhotonNetwork.GameVersion = gameVersion;
        //}

        LoadingSceneManager.LoadScene(mRoomCodestr);
    }

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
       // PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        roomCode = new List<int>();
        roomCode.Clear();
        result = 0;
        mText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        MakeCodeToString();
        ShowCode();
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("VR Intro : OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            RoomOptions roomOption = new RoomOptions() { IsVisible = false, MaxPlayers = maxPlayersPerRoom };
            PhotonNetwork.JoinOrCreateRoom(mRoomCodestr, roomOption, TypedLobby.Default);

            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        Debug.LogWarningFormat("VR Intro : OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // TODO :: 이미 방 번호를 사용하고 있어서 접속이 안될 때 이를 알려주는 메시지가 필요하다.

        Debug.Log("VR Intro : OnJoinRandomFailed() was called by PUN. Already roomNum Used And Room is Full");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("VR Intro : OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the Room And Alone");

            //PhotonNetwork.LoadLevel("VRRoom");
            PhotonNetwork.LoadLevel("08 11 ProtoType");
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("We load the Room And two.");
        }

    }

    #endregion
}
