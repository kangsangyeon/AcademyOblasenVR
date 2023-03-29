using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{


    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioClip[] bgmList;

    #region Enum
    public enum ESoundType
    {
        BGM,
        SFX,
    }
    #endregion

    #region Public Fields

    public static SoundManager instance;

    #endregion

    #region Public Methods
    public void SFXPlay(string soundName, AudioClip _clip)
    {
        if (_clip == null)
            return;
        
        GameObject soundPlayer = new GameObject(soundName + "Sound");
        AudioSource audioSource = soundPlayer.AddComponent<AudioSource>();
        audioSource.clip = _clip;
        audioSource.Play();

        Destroy(soundPlayer, _clip.length);
    }

    public void BGMPlay(AudioClip _clip)
    {
        if (_clip == null)
            return;

        bgm.clip = _clip;
        bgm.loop = true;
        bgm.volume = 0.5f;
        bgm.Play();
    }

    public void BGMStop()
    {
        bgm.Stop();
    }
    #endregion

    #region Private Methods
    private void OnScreenLoaded(Scene _scene, LoadSceneMode _mode)
    {
        foreach (AudioClip _name in bgmList)
        {
            if (_scene.name == _name.name)
            {
                BGMPlay(_name);
            }
        }
    }

    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnScreenLoaded;
            DontDestroyOnLoad(instance);

            if (SceneManager.GetActiveScene().name == "VRMain")
            {
                BGMPlay(bgmList[0]);
            }
        }

        else
        {
            Destroy(this);
        }
    }
}
