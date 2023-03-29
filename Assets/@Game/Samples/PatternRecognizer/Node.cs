using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.TextCore.Text;
using UnityEngine;

[System.Serializable]
public struct NodePosition
{
    public int x;
    public int y;

    public NodePosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        var _other = (NodePosition)obj;

        bool _bEquals =
            this.x == _other.x
            && this.y == _other.y;

        return _bEquals;
    }

    public override string ToString()
    {
        return $"{{{this.x}, {this.y}}}";
    }
}

public class Node : MonoBehaviour
{
    [SerializeField] private AudioClip m_Asset_UserProgressChangedSfx;
    [SerializeField] private AudioClip m_Asset_UserFaultSfx;

    [SerializeField] private Material m_IdleMaterial;
    [SerializeField] private Material m_OnActivateMaterial;
    [SerializeField] private MeshRenderer m_NodeMesh;
    [SerializeField] private float m_RotateAnglePerSec = 20.0f;
    [SerializeField] private AudioSource m_AudioSource;
    private PatternRecognizer m_Recognizer;

    private NodePosition m_Position;

    public NodePosition Position
    {
        get => m_Position;
        set => m_Position = value;
    }

    private void Start()
    {
        SetActivate(false);
    }

    public void SetPatternRecognizer(PatternRecognizer _recognizer) => m_Recognizer = _recognizer;

    public void SetActivate(bool _activate)
    {
        m_NodeMesh.material = _activate ? m_OnActivateMaterial : m_IdleMaterial;
    }

    public void PlayUserProgressChangedSfx(float _progress)
    {
        m_AudioSource.clip = m_Asset_UserProgressChangedSfx;
        m_AudioSource.pitch = 1 + 0.2f * _progress;
        m_AudioSource.Play();
    }

    public void PlayUserFaultSfx()
    {
        m_AudioSource.clip = m_Asset_UserFaultSfx;
        m_AudioSource.Play();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, m_RotateAnglePerSec * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (enabled == false)
            return;
        
        if (other.CompareTag("PatternRecognizer") == false)
            return;

        if (m_Recognizer == null)
        {
            Debug.LogError("missing recognizer reference!!");
            return;
        }
        
        m_Recognizer.AttemptVisit(this);
    }
}