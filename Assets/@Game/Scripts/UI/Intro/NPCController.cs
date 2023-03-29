using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] int ScriptCount;
    [SerializeField] float ScriptTimer;
    List<GameObject> myScripts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        FindScripts();
        StartCoroutine(StartScript());
    }

    private void FindScripts()
    {
        string _name = "Script_";
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i <= ScriptCount; i++)
        {
            string _tmp = _name + i.ToString();
            GameObject _script = canvas.Find(_tmp).gameObject;
            myScripts.Add(_script);
            _script.SetActive(false);
        }
    }

    private void SetScript(int index)
    {
        if (index < 0)
            return;

        if (index > 0)
        {
            myScripts[index - 1].SetActive(false);
        }

        myScripts[index].SetActive(true);
    }

    IEnumerator StartScript()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i <= ScriptCount; i++)
        {
            yield return StartCoroutine(AutoScript(i));

        }
    }

    IEnumerator AutoScript(int i)
    {
        SetScript(i);
        yield return new WaitForSeconds(ScriptTimer);
        if (i == ScriptCount)
        {
            myScripts[i].SetActive(false);
        }
    }
}
