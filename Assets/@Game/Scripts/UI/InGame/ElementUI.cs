using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ElementUI : MonoBehaviour
{
    #region Private Fields
    XRBaseInteractable mInteractable;
    int mElement;
    #endregion

    #region Private Methods
    private void SelectEntered(SelectEnterEventArgs events)
    {
        Debug.Log("Element Selected");
        var _components = GameObject.FindObjectsOfType<PlayerManagerCoroutine>();
        foreach (var _component in _components)
        {
            var _photonView = _component.gameObject.GetComponent<Photon.Pun.PhotonView>();
            if (_photonView.IsMine)
            {
                _component.SetElement(mElement);
                break;
            }
        }
    }

    private void FindName()
    {
        switch (gameObject.name)
        {
            case "Fire Element":
                {
                    mElement = 1;
                }
                break;

            case "Water Element":
                {
                    mElement = 2;
                }
                break;

            case "Light Element":
                {
                    mElement = 3;
                }
                break;

            case "Dark Element":
                {
                    mElement = 4;
                }
                break;

            default:
                mElement = 5;
                break;

        }

    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mInteractable = GetComponent<XRSimpleInteractable>();
        FindName();
        mInteractable.selectEntered.AddListener(SelectEntered);
    }

}
