using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigPanelCapacitor : ConfigPanel {


    public override void Start()
    {
        capacitorManager Cap = component as capacitorManager;

        ParameterIncrementer.Add(x => Cap.Cin += x); // lambda expression
        ParameterToggler.Add(x => Cap.locked ^= x);
        
        base.Start();
    }

}
