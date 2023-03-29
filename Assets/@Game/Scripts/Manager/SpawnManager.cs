using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject GirlVRPlayerPrefab;
    public GameObject MaleVRPlayerPrefab;

    public float Init_XPos = 0;
    public float Init_YPos = 2;
    public float Init_ZPos = 3;
    public float Distance_X = 0;
    public float Distence_Z = 3;

    // Start is called before the first frame update
    void Start()
    {
        int pCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (pCount == 1)
            {
                PhotonNetwork.Instantiate(GirlVRPlayerPrefab.name, new Vector3((Init_XPos + Distance_X * (pCount - 1)), Init_YPos, Init_ZPos + (Distence_Z * (pCount - 1))), Quaternion.Euler(new Vector3(0, 30 + 180 * (pCount - 1), 0)));
            }
            else if (pCount == 2)
            {
                PhotonNetwork.Instantiate(MaleVRPlayerPrefab.name, new Vector3((Init_XPos + Distance_X * (pCount - 1)), Init_YPos, Init_ZPos + (Distence_Z * (pCount - 1))), Quaternion.Euler(new Vector3(0, 30 + 180 * (pCount - 1), 0)));
            }
        }
    }
}
