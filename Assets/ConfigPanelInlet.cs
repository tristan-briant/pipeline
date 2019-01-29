using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigPanelInlet : ConfigPanel {

    InletManager Inlet;

    public List<Text> ValueParameterText;
    public List<float> increment;

    public override void Start()
    {
        base.Start();
        Inlet = component as InletManager;
    }

    public void IncrementValue(int parameter)
    {
        ChangeValue(parameter, +1.0f);
    }

    public void DecrementValue(int parameter)
    {
        ChangeValue(parameter, -1.0f);
    }

    void ChangeValue(int parameter, float sign)
    {
        switch (parameter)
        {
            case 0:
                Inlet.pset += sign * increment[parameter]; break;
            case 1:
                Inlet.imax += sign * increment[parameter]; break;
        }
    }

    void Update () {
        int k = 0;
        ValueParameterText[k++].text = Inlet.pset.ToString("F1");
        ValueParameterText[k++].text = Inlet.imax.ToString("F1");
    }
}
