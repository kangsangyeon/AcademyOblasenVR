using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandGripElement : MonoBehaviour
{
    ActionBasedController controller;
    public Hand hand;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Test();
        hand.SetGrip(controller.activateAction.action.ReadValue<float>());
    }

    //void Test()
    //{
    //    if (controller.selectAction.action.IsPressed())
    //    {
    //        Debug.Log("´­¸²");
    //    }
    //}
}
