using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIPointerOnOff : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Ect1;

    // Start is called before the first frame update
    void Start()
    {
        Ect1.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Ect1.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Ect1.SetActive(false);
    }

}
