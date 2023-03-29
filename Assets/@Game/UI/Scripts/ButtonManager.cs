//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ButtonManager : MonoBehaviour
//{
//    public TMP_Dropdown m_dropDown;

//    public GameObject circleBtn;
//    public GameObject squareBtn;
//    public GameObject sceneBtn;

//    void Start()
//    {
//        circleBtn.SetActive(false);
//        squareBtn.SetActive(false);
//        sceneBtn.SetActive(false);

//        m_dropDown.options.Clear();

//        List<string> buttons = new List<string>();
//        buttons.Add("Circle");
//        buttons.Add("Square");
//        buttons.Add("Scene");

//        foreach(var button in buttons)
//        {
//            m_dropDown.options.Add(new TMP_Dropdown.OptionData() { text = button});
//        }

//        ChangeContents();

//        m_dropDown.onValueChanged.AddListener(delegate { ChangeContents(); });
//    }

//    private void ChangeContents()
//    {
//        int _temp = m_dropDown.value;

//        if (_temp == 0)
//        {
//            circleBtn.SetActive(true);
//            squareBtn.SetActive(false);
//            sceneBtn.SetActive(false);
//        }

//        else if (_temp == 1)
//        {
//            circleBtn.SetActive(false);
//            squareBtn.SetActive(true);
//            sceneBtn.SetActive(false);
//        }

//        else if (_temp == 2)
//        {
//            circleBtn.SetActive(false);
//            squareBtn.SetActive(false);
//            sceneBtn.SetActive(true);
//        }
//    }
//}
