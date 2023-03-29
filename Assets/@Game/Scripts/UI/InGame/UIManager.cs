using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager mInstance;
    public static UIManager instance
    {
        get
        {
            if (mInstance == null)
                mInstance = FindObjectOfType<UIManager>();
            return mInstance;
        }
    }

    [SerializeField] GameObject ReadyObejct;
    [SerializeField] GameObject FightObejct;
    [SerializeField] GameObject EnemyTurnObejct;
    [SerializeField] GameObject MyTurnObejct;

    // Start is called before the first frame update
    void Start()
    {
        ReadyObejct.SetActive(false);
        FightObejct.SetActive(false);
        EnemyTurnObejct.SetActive(false);
        MyTurnObejct.SetActive(false);
    }

    IEnumerator UIScaleUp(GameObject _object)
    {
        Vector3 scaleTmp = new Vector3(0.1f, 0.1f, 0.1f);
        _object.transform.localScale = scaleTmp;

        while (_object.transform.localScale.x <= 1f)
        {
            _object.transform.localPosition += scaleTmp;
            yield return new WaitForSeconds(0.1f);
        }

        _object.transform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator UICloseUp(GameObject _object)
    {
        Vector3 _originalPostion = _object.transform.position;
        Vector3 _direction = _object.transform.forward;

        _object.transform.position -=  _direction * 5;

        while (_object.transform.position == _originalPostion)
        {
            _object.transform.TransformDirection(_direction);
           // _object.transform.position += _direction * 0.1f;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        _object.SetActive(false);
    }

    public void MoveUIandScale(GameObject _object)
    {
        _object.SetActive(true);
        StartCoroutine(UIScaleUp(_object));
        StartCoroutine(UICloseUp(_object));
    }
}
