using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour
{
    public Transform FirePoint => m_FirePoint;

    [SerializeField] private Transform m_FirePoint;
    
    // TODO: 특정 particle 재생 요청 등의 코드가 작성되어야 합니다.
}
