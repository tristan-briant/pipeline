using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatParameterModifier : MonoBehaviour {

    public string ParameterName; // Name of the property
    public float increment=0.1f;
    public Text ValueText;

    object ob;

    private void Start()
    {
        ob = transform.GetComponentInParent<ConfigPanel>().component;

        ValueText.text = ((float)ob.GetType().GetProperty(ParameterName).GetValue(ob, null)).ToString("F1");
    }

    public void IncrementValue(float multiplier) {
        float a = (float) ob.GetType().GetProperty(ParameterName).GetValue(ob, null);
        a += multiplier * increment;
        ob.GetType().GetProperty(ParameterName).SetValue(ob, a, null);

        ValueText.text = ((float) ob.GetType().GetProperty(ParameterName).GetValue(ob, null)).ToString("F1");
    }

}
