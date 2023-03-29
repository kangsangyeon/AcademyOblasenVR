using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBar : MonoBehaviour
{
    public Slider sizeSlider;

    public void ChangeSize()
    {
        float _value = sizeSlider.value;
        Vector3 _size = new Vector3(_value, _value, _value);
        this.transform.localScale = _size;
    }

    void Update()
    {
        ChangeSize();
    }
}
