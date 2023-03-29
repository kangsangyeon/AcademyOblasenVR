using Photon.Pun;
using UnityEngine;

public class NetworkGameSpawner : MonoBehaviour
{
    public GameObject GirlVRPlayerPrefab;
    public GameObject MaleVRPlayerPrefab;

    [SerializeField] private GameplayScene m_Scene;
    
    void Start()
    {
        int pCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            Vector3 _position = m_Scene.GetPlayerPoint(pCount - 1).position;
            Quaternion _rotation = m_Scene.GetPlayerPoint(pCount - 1).rotation;

            string _prefabName = pCount == 1 ? GirlVRPlayerPrefab.name : MaleVRPlayerPrefab.name;
            
            PhotonNetwork.Instantiate(_prefabName, _position ,_rotation);
        }
    }
}
