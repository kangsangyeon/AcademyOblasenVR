using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// photon
using Photon.Pun;
using Photon.Realtime;


public class SceneLoadTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    string gameVersion = "1";
    bool isConnecting;

    CodeInputManager cim;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        cim = GameObject.Find("CodePanel").GetComponent<CodeInputManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneTest()
    {
        LoadingSceneManager.LoadScene(cim.mRoomCodestr);

        //if (PhotonNetwork.IsConnected)
        //{
        //    PhotonNetwork.JoinRandomRoom();
        //}
        //else
        //{
        //    isConnecting = PhotonNetwork.ConnectUsingSettings();
        //    PhotonNetwork.GameVersion = gameVersion;
        //}
    }


    #region MonobehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher : OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();

            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher : OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);

        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayersPerRoom;
        PhotonNetwork.CreateRoom("TestRoom", options);      // 이름이 없고 옵션은 기본인 방을 생성.
    }

    public override void OnJoinedRoom()
    {

        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            PhotonNetwork.LoadLevel("VRRoomTest");

        }

    }
    #endregion
}
