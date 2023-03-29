using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Button_ChangeScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text_SceneName;
    [SerializeField] private Button m_Button_ChangeScene;

    public string SceneName
    {
        get => m_Text_SceneName.text;
        set
        {
            m_Text_SceneName.text = value;
            m_Button_ChangeScene.onClick.RemoveAllListeners();
            m_Button_ChangeScene.onClick.AddListener(() => SceneManager.LoadScene(value));
        }
    }
}