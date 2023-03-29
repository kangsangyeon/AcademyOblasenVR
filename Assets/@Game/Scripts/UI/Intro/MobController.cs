using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobController : MonoBehaviour
{
    [SerializeField] GameObject MobEffect;
    [SerializeField] AudioClip TutoMobSound;
    [SerializeField] GameObject Mob;
    [SerializeField] int ScriptCount;
    List<GameObject> myScripts = new List<GameObject>();

    IEnumerator Start()
    {
        SoundManager.instance.SFXPlay("TutoMob", TutoMobSound);
        MobEffect.SetActive(true);
        FindScripts();
        yield return new WaitForSeconds(0.3f);

        Mob.SetActive(true);
    }

    private void FindScripts()
    {
        string _name = "Script_";
        Transform canvas = transform.GetChild(0);
        for (int i = 0; i <= ScriptCount; i++)
        {
            string _tmp = _name + i.ToString();
            GameObject _script = canvas.Find(_tmp).gameObject;
            myScripts.Add(_script);
            _script.SetActive(false);
        }
    }

    public void SetScript(int index)
    {
        if (index < 0)
            return;

        if (index > 0)
        {
            myScripts[index - 1].SetActive(false);
        }
        
        myScripts[index].SetActive(true);
    }

    public void OffScript(int index)
    {
        myScripts[index].SetActive(false);
    }
}
