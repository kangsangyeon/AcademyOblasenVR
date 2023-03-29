using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject Me;
    [SerializeField] GameObject TimerObject;


    private Vector3 position;
    private Vector3 target;
    private float timer;
    void Start()
    {
        position = Me.transform.position + Me.transform.forward * 1;
        target = Enemy.transform.position;
        timer = 0f;
        TimerObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }

        else if (timer <= 0f)
        {
            TimerObject.SetActive(false);
            timer = 0f;
        }

        KeyInput();
    }

    private void KeyInput()
    {
        if (timer == 0f)
        {
            if (Input.GetKeyDown("1"))
            {
                WitchEffectManager.instance.MeteorEffect(position,target);
                timer = WitchEffectManager.instance.GetMeteorTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("2"))
            {
                WitchEffectManager.instance.MagmaImpactEffect(position, target);
                timer = WitchEffectManager.instance.GetMagmaImpactTime(position, target);
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("3"))
            {
                WitchEffectManager.instance.HealingWaveEffect(position);
                timer = WitchEffectManager.instance.GetHealingWaveTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("4"))
            {
                WitchEffectManager.instance.ChainIceboltEffect(position,target);
                timer = WitchEffectManager.instance.GetIceboltTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("5"))
            {
                WitchEffectManager.instance.JudgementSwordEffect(position,target);
                timer = WitchEffectManager.instance.GetJudgementSwordTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("6"))
            {
                WitchEffectManager.instance.LighteningStrikeEffect(target);
                timer = WitchEffectManager.instance.GetLightningStrikeTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("7"))
            {
                WitchEffectManager.instance.CircleOfCurseEffect(target);
                timer = WitchEffectManager.instance.GetCircleOfCurseTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("8"))
            {
                WitchEffectManager.instance.DarkMistEffect(target);
                timer = WitchEffectManager.instance.GetDarkMistTime();
                TimerObject.SetActive(true);
            }

            else if (Input.GetKeyDown("9"))
            {
                WitchEffectManager.instance.LigtningStrikeEffect2(target);
            }
        }
    }
}
