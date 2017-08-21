using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseFrontier : BaseComponent {

    protected override void Start() {
    }


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        calcule_i_p_blocked(p, i, alpha, 0);
    }


}
