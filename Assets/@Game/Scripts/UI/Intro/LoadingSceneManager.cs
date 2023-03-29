using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class LoadingSceneManager : MonoBehaviourPunCallbacks
{
    public static string mRoomCodestr;

    private string nextSceneName;

    [SerializeField]
    public byte maxPlayersPerRoom = 2;
    string gameVersion = "1.0";
    bool isConnecting;

    private float timer = 0f;

    [SerializeField] Image progressIMG;

    // Start is called before the first frame update
    void Start()
    {
        progressIMG.fillAmount = 0f;
        PhotonNetwork.AutomaticallySyncScene = true;

        Debug.Log("VR Intro : Called EnterGame, RoomNum is " + mRoomCodestr);


        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOption = new RoomOptions() { IsVisible = false, MaxPlayers = maxPlayersPerRoom };
            PhotonNetwork.JoinOrCreateRoom(mRoomCodestr, roomOption, TypedLobby.Default);
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }


    }

    public static void LoadScene(string sceneName)
    {
        mRoomCodestr = sceneName;

        //SceneManager.LoadScene("VRLoadingScene");
        SceneManager.LoadScene("VRLoadingStatic");
    }

    IEnumerator LoadScene()
    {
        //yield return null;

        //AsyncOperation op = SceneManager.LoadSceneAsync("TestVRNetworkGameplay", LoadSceneMode.Additive);
        //op.allowSceneActivation = false;

        //float timer = 0.0f;
        //while (!op.isDone)
        //{
        //    yield return null;

        //    timer += Time.deltaTime;

        //    if (op.progress >= 0.9f)
        //    {
        //        progressIMG.fillAmount = Mathf.Lerp(progressIMG.fillAmount, 1f, timer);

        //        if (progressIMG.fillAmount == 1.0f)
        //        {
        //            op.allowSceneActivation = true;
        //            op = SceneManager.UnloadSceneAsync("VRLoadingScene");
        //        }
        //    }

        //    else
        //    {
        //        progressIMG.fillAmount = Mathf.Lerp(progressIMG.fillAmount, op.progress, timer);
        //        if (progressIMG.fillAmount >= op.progress)
        //        {
        //            timer = 0f;
        //        }

        while (progressIMG.fillAmount < 1.0f)
        {
            yield return null;
            timer += Time.deltaTime;

            progressIMG.fillAmount = timer * 1.0f;
        }

        progressIMG.fillAmount = 1.0f;

        SceneManager.UnloadSceneAsync(1);

        yield break;
    }


    // Update is called once per frame
    void Update()
    {

    }

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

        // TODO :: �̹� �� ��ȣ�� ����ϰ� �־ ������ �ȵ� �� �̸� �˷��ִ� �޽����� �ʿ��ϴ�.

        Debug.Log("VR Intro : OnJoinRandomFailed() was called by PUN. Already roomNum Used And Room is Full");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("VR Intro : OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the Room And Alone");

            //PhotonNetwork.LoadLevel("08 11 ProtoType");
            PhotonNetwork.LoadLevel("VRNetworkGameplay");
            StartCoroutine(LoadScene());
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("We load the Room And two.");
        }

    }

    #endregion
}
