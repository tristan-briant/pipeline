using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ConfigPanel : MonoBehaviour {

    public BaseComponent component;
    public GameObject selection;

    public List<Text> ValueParameterText = new List<Text>();
    public List<float> increment = new List<float>();


    public List<Image> ValueParameterToggle = new List<Image>();

 
    protected List<Func<float, float>> ParameterIncrementer = new List<Func<float, float>>();
    protected List<Func<bool, bool>> ParameterToggler = new List<Func<bool, bool>>();


    GameObject selec;

    public virtual void Start()
    {
        selec=Instantiate(selection);
        selec.transform.SetParent(component.transform);
        selec.transform.localScale = Vector3.one;
        selec.transform.localPosition = Vector3.zero;
    }

    public void Close()
    {
        Destroy(this.gameObject);
        Destroy(selec);
    }

    /*public void IncrementValue(int parameter)
    {
        ChangeValue(parameter, +1.0f);
    }

    public void DecrementValue(int parameter)
    {
        ChangeValue(parameter, -1.0f);
    }

    public void IncrementFastValue(int parameter)
    {
        ChangeValue(parameter, +20.0f);
    }

    public void DecrementFastValue(int parameter)
    {
        ChangeValue(parameter, -20.0f);
    }

    void ChangeValue(int index, float sign)
    {
        ParameterIncrementer[index](sign * increment[index]);
    }

    public void ToggleValue(int parameter)
    {
            ParameterToggler[parameter](true);    // true means a toggle occure 
    }*/


    /*void Update()
    {
        for (int i = 0; i < ValueParameterText.Count; i++)
            ValueParameterText[i].text = ParameterIncrementer[i](0).ToString("F1");

        for (int i = 0; i < ValueParameterText.Count; i++)
            ValueParameterToggle[i].enabled = ParameterToggler[i](false);
    }*/

}
