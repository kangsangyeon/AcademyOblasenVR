using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject LocalXROriginGameObject;

    public GameObject AvatarBodyGameObject;
    public GameObject AvatarHairGameObject;
    public GameObject AvatarStickGameObject;

    
    // Start is called before the first frame update
    void Awake()
    {
        if(photonView.IsMine)
        {
            LocalXROriginGameObject.SetActive(true);

            SetLayerRecursively(AvatarBodyGameObject, 7);
            SetLayerRecursively(AvatarHairGameObject, 8);
            SetLayerRecursively(AvatarStickGameObject, 9);
        }
        else
        {
            LocalXROriginGameObject.SetActive(false);

            SetLayerRecursively(AvatarBodyGameObject, 0);
            SetLayerRecursively(AvatarHairGameObject, 0);
            SetLayerRecursively(AvatarStickGameObject, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}
