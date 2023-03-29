using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using UnityEngine;

public class MagicFx : MonoBehaviour
{
    [SerializeField] private AudioClip m_Asset_RevealSfx;
    [SerializeField] private AudioClip m_Asset_HideSfx;
    [SerializeField] private MeshRenderer m_MagicMesh;
    [SerializeField] private float m_RevealDuration;
    [SerializeField] private float m_WaitingDuration;
    [SerializeField] private float m_HideDuration;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        Material _mat = m_MagicMesh.material;
        AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 1.0f);
    }

    public void ActiveShootingMagic(Vector3 target, Vector3 position)
    {
        StartCoroutine(ShootingCorountine(target, position));
    }

    IEnumerator PlayMagic()
    {
        Material _mat = m_MagicMesh.material;
        m_AudioSource.clip = m_Asset_RevealSfx;
        m_AudioSource.Play();

        float _startTime = Time.time;
        while (true)
        {
            float _delta = (Time.time - _startTime) / m_RevealDuration;
            float _clip = 1.0f - _delta;
            AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, _clip);

            if (_delta >= 1.0f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(m_WaitingDuration);

        m_AudioSource.clip = m_Asset_HideSfx;
        m_AudioSource.Play();

        _startTime = Time.time;
        while (true)
        {
            float _delta = (Time.time - _startTime) / m_HideDuration;
            AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, _delta);

            if (_delta >= 1.0f)
                break;

            yield return null;
        }
    }

    IEnumerator ShootMagic(Vector3 target, float speed)
    {
        while (gameObject.transform.position != target)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, Time.deltaTime * speed);
            yield return null;
        }

        yield return null;
    }

    IEnumerator ShootingCorountine(Vector3 target, Vector3 position)
    {
        gameObject.transform.position = position;

        StartCoroutine(PlayMagic());

        yield return new WaitForSeconds(m_RevealDuration);

        float _targetz = target.z;
        float _positionz = position.z;
        float _distance = Mathf.Abs(_targetz - _positionz);
        float _time = m_HideDuration + m_WaitingDuration;
        float _speed = _distance / _time;

        StartCoroutine(ShootMagic(target, _speed));
        yield return null;
    }

    public float GetTime()
    {
        float _time = m_RevealDuration + m_HideDuration + m_WaitingDuration;
        return _time;
    }
}
