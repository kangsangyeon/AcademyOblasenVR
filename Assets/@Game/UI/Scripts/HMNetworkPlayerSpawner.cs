using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HMNetworkPlayerSpawner : MonoBehaviourPunCallbacks
{

    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate("HMTest", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
