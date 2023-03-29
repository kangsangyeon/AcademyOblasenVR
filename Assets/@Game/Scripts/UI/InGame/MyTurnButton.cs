using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyTurnButton : MonoBehaviour
{
    #region Private Fields
    private Button mButton;
    #endregion

    private void OnButtonClicked()
    {
        Debug.Log("Turn Button Clicked");


        var _components = GameObject.FindObjectsOfType<PlayerManagerCoroutine>();
        foreach (var _component in _components)
        {
            var _photonView = _component.gameObject.GetComponent<Photon.Pun.PhotonView>();
            if (_photonView.IsMine)
            {
                _component.InitCoroutine();
                gameObject.SetActive(false);
                Debug.Log("Turn Button SetActive (false)");
                Debug.Log("Turn Start");
                break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mButton = GetComponent<Button>();
        mButton.onClick.AddListener(OnButtonClicked);
    }
}
