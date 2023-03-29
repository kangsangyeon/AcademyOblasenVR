using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Examples;

public class EnableComponentsIfMine : MonoBehaviour
{
    [SerializeField] private PhotonView m_PhotonView;
    [SerializeField] private MonoBehaviour[] m_Components;
    [SerializeField] private GameObject[] m_GameObjects;

    private void Awake()
    {
        List<MonoBehaviour> _components = new List<MonoBehaviour>();

        // VR 컴포넌트를 추가합니다.
        var _xrOrigin = GetComponentInChildren<XROrigin>();
        var _xrLocomotionSystem = GetComponentInChildren<LocomotionSystem>();
        var _xrLocomotionProviders = GetComponentsInChildren<LocomotionProvider>();
        var _xrLocomotionSchemeManager = GetComponentInChildren<LocomotionSchemeManager>();
        var _xrCharacterControllerDriver = GetComponentInChildren<CharacterControllerDriver>();
        var _xrTrackedPoseDriver = GetComponentInChildren<TrackedPoseDriver>();
        var _xrBaseControllers = GetComponentsInChildren<XRBaseController>();

        _components.Add(_xrOrigin);
        _components.Add(_xrLocomotionSystem);
        _components.AddRange(_xrLocomotionProviders);
        _components.Add(_xrLocomotionSchemeManager);
        _components.Add(_xrCharacterControllerDriver);
        _components.Add(_xrTrackedPoseDriver);
        _components.AddRange(_xrBaseControllers);

        // 인스펙터에서 추가한 추가 컴포넌트들을 대상으로 추가합니다.
        _components.AddRange(m_Components);

        // photon view가 없다면 모든 컴포넌트들을 활성화합니다.
        // photon view가 있다면, isMine여부에 따라 컴포넌트를 활성화합니다.
        bool _enable = m_PhotonView ? m_PhotonView.IsMine : true;
        _components.ForEach(c => c.enabled = _enable);
        
        m_GameObjects.ToList().ForEach(g => g?.SetActive(_enable));
    }
}