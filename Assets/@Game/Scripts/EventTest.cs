using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class EventTest : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public const byte MoveUnitsToTargetPositionEventCode = 1;

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == MoveUnitsToTargetPositionEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 targetPosition = (Vector3)data[0];
            //for (int index = 1; index < data.Length; ++index)
            //{
            //    int unitId = (int)data[index];
            //    UnitList[unitId].TargetPosition = targetPosition;
            //}
            Debug.Log("Dummy 이벤트 수신." + targetPosition.x);
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Dummy Net ID : " + NetID);
    }


}
