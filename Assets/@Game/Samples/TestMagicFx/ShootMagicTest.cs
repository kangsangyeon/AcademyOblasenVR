using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMagicTest : MonoBehaviour
{
    public GameObject magic1;


    public void Shoot()
    {
        magic1.SetActive(true);
       // magic1.GetComponent<MagicFx>().ActiveMagic(target, position);
    }
}
