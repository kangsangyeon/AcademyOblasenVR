using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class CodeButtons : MonoBehaviour
{
    #region Private Fields

    Button mButton;
    TMP_Text mTmp;
    string mNumber;
    int num;

    CodeInputManager cim;

    #endregion

    #region Public Methods

    public void InputCode()
    {
        if (cim.roomCode.Count < 5)
        {
            cim.roomCode.Add(num);
        }
    }

    public void DeleteCode()
    {
        if (cim.roomCode.Count > 0)
        {
            cim.roomCode.RemoveAt(cim.roomCode.Count - 1);
        }
    }
    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        cim = GameObject.Find("CodePanel").GetComponent<CodeInputManager>();
        mButton = GetComponent<Button>();
        mTmp = GetComponentInChildren<TMP_Text>();
        mNumber = mTmp.text;
        if (mNumber != "Delete")
        {
            num = int.Parse(mNumber);
            mButton.onClick.AddListener(InputCode);
        }
    }
    #endregion
}
