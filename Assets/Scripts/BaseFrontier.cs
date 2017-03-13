using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseFrontier : BaseComponent {

    protected override void Start() {
    }


    public override void calcule_i_p(float[] p, float[] i)
    {
        i[0] = p[0] / Rground;
        p[0] *= 0.99f;
        //i[0] = 0;
    }


}
