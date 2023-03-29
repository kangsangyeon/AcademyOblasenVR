using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MicInput : MonoBehaviour
{
    [SerializeField] private bool m_bUseDefaultMicrophone = true;
    [SerializeField] private int m_SampleWindowSize = 2048;
    [SerializeField] private float m_LevelMultiplier = 10.0f;
    
    private string[] m_MicNameList;
    private int m_MicIndex;
    private int m_MicSampleRate;
    private string m_LastMicName;

    private AudioClip m_AudioClip;
    private bool m_bIsReady;

    public string GetCurrentMicName() => m_bUseDefaultMicrophone ? null : m_MicNameList[m_MicIndex];

    void Start()
    {
        Setup();
    }

    private void OnEnable()
    {
        // UpdateMicrophone();
    }

    private void OnDisable()
    {
        // EndMicrophone();
    }

    // private void OnDestroy()
    // {
    //     EndMicrophone();
    // }

    public void Setup()
    {
        m_MicNameList = Microphone.devices;
        m_LastMicName = GetCurrentMicName();
        UpdateMicrophone();

        enabled = false;
    }

    void UpdateMicrophone()
    {
        //Start recording to audioclip from the mic
        int _micMinFreq, _micMaxFreq;
        Microphone.GetDeviceCaps(GetCurrentMicName(), out _micMinFreq, out _micMaxFreq);
        m_MicSampleRate = _micMinFreq;
        m_AudioClip = Microphone.Start(GetCurrentMicName(), true, 10, m_MicSampleRate);

        if (Microphone.IsRecording(GetCurrentMicName()))
        {
            //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
            while (!(Microphone.GetPosition(GetCurrentMicName()) > 0))
            {
            } // Wait until the recording has started. 

            Debug.Log("recording started with " + GetCurrentMicName());
        }
        else
        {
            //microphone doesn't work for some reason

            Debug.Log(GetCurrentMicName() + " doesn't work!");
        }
        
        m_bIsReady = true;
    }

    void EndMicrophone()
    {
        Microphone.End(m_LastMicName);
        m_bIsReady = false;
    }

    public float GetAveragedLevel()
    {
        if (m_bIsReady == false)
            return -1.0f;

        float[] data = new float[m_SampleWindowSize];
        float totalLoudness = 0;

        int micClipPosition = Microphone.GetPosition(GetCurrentMicName()) - m_SampleWindowSize - 1;
        m_AudioClip.GetData(data, micClipPosition);

        float _totalVolume = 0.0f;
        foreach (float s in data)
        {
            _totalVolume = _totalVolume + Mathf.Abs(s);
        }

        return (_totalVolume / m_SampleWindowSize) * m_LevelMultiplier;
    }
    
    public float GetHighestLevel()
    {
        if (m_bIsReady == false)
            return -1.0f;

        float[] data = new float[m_SampleWindowSize];
        float totalLoudness = 0;

        int micClipPosition = Microphone.GetPosition(GetCurrentMicName()) - m_SampleWindowSize - 1;
        m_AudioClip.GetData(data, micClipPosition);

        float _maxLevel = float.NegativeInfinity;
        foreach (float s in data)
        {
            // totalLoudness += Mathf.Abs(s);
            _maxLevel = Mathf.Max(_maxLevel, Mathf.Abs(s));
        }

        return _maxLevel * m_LevelMultiplier;
    }

    public float GetTotalLevel()
    {
        if (m_bIsReady == false)
            return -1.0f;

        float[] data = new float[m_SampleWindowSize];
        float totalLoudness = 0;

        int micClipPosition = Microphone.GetPosition(GetCurrentMicName()) - m_SampleWindowSize - 1;
        m_AudioClip.GetData(data, micClipPosition);

        float _totalVolume = 0.0f;
        foreach (float s in data)
        {
            _totalVolume = _totalVolume + Mathf.Abs(s);
        }

        return _totalVolume * m_LevelMultiplier;
    }
}