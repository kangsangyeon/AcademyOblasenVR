using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public AudioClip buttonHover;
    public AudioClip buttonClick;
    public AudioClip matchSuccess;

    public void ButtonHover()
    {
        SoundManager.instance.SFXPlay("ButtonHover", buttonHover);
    }

    public void ButtonClick()
    {
        SoundManager.instance.SFXPlay("ButtonClick", buttonClick);
    }

    public void MatchSuccess()
    {
        SoundManager.instance.SFXPlay("Match", matchSuccess);
    }

}
