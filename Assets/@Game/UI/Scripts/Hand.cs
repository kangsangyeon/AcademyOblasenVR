using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    //[SerializeField] PlayerManagerCoroutine player;
    Animator animator;

    [SerializeField] GameObject AuraHolder;
    [SerializeField] GameObject FireAura;
    [SerializeField] GameObject WaterAura;
    [SerializeField] GameObject LightAura;
    [SerializeField] GameObject DarkAura;

    private float gripCurrent;
    private float gripTarget;
    public float speed;
    private string boolParameter = "ElementSelect";
    private string floatParameter = "grip";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimateHand();
        AuraHolder.transform.position = transform.position;
    }
    internal void SetGrip(float v)
    {
        gripTarget = v;
    }

    public void SetBool(bool set)
    {
        animator.SetBool(boolParameter, set);
    }

    public void AuraSet(EElementType _type)
    {
        switch (_type)
        {
            case EElementType.None:
                {
                    FireAura.SetActive(false);
                    WaterAura.SetActive(false);
                    LightAura.SetActive(false);
                    DarkAura.SetActive(false);
                }
                break;

            case EElementType.Fire:
                {
                    FireAura.SetActive(true);
                    WaterAura.SetActive(false);
                    LightAura.SetActive(false);
                    DarkAura.SetActive(false);
                }
                break;

            case EElementType.Water:
                {
                    FireAura.SetActive(false);
                    WaterAura.SetActive(true);
                    LightAura.SetActive(false);
                    DarkAura.SetActive(false);
                }
                break;

            case EElementType.Light:
                {
                    FireAura.SetActive(false);
                    WaterAura.SetActive(false);
                    LightAura.SetActive(true);
                    DarkAura.SetActive(false);
                }
                break;

            case EElementType.Dark:
                {
                    FireAura.SetActive(false);
                    WaterAura.SetActive(false);
                    LightAura.SetActive(false);
                    DarkAura.SetActive(true);
                }
                break;
        }

    }

    void AnimateHand()
    {
        if (animator.GetBool(boolParameter))
        {
            animator.SetFloat(floatParameter, 1.0f);
        }
        //SetBool()
        else if (gripCurrent != gripTarget)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * speed);
            animator.SetFloat(floatParameter, gripCurrent);
        }
    }
}