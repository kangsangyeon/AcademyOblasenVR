using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : MonoBehaviour
{
    #region Public Fields

    public GameObject tutorialManager;
    public GameObject codeInputCanvas;
    public GameObject tutoBot;

    #endregion

    #region Private Fields



    #endregion

    #region Public Methods



    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        tutorialManager.SetActive(true);
        codeInputCanvas.SetActive(false);
        tutoBot.SetActive(true);
    }

    void Update()
    {
            
    }

    #endregion



}
