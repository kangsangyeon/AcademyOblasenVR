using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroDoor : MonoBehaviour
{
    #region Private Fields

    private bool isSelected;

    #endregion

    #region Public Fields

    public GameObject doorObject;
    public GameObject InputCanvas;

    #endregion

    #region Public Mehthods

    #endregion

    #region MonoBehaviour Callbacks
    // Start is called before the first frame update
    void Start()
    {
        isSelected = false;
        InputCanvas.SetActive(false);
        doorObject.SetActive(true);
    }

    public void CodeCanvasSet()
    {
        InputCanvas.SetActive(true);
    }

    #endregion
}
