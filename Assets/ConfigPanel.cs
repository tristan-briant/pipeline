using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ConfigPanel : MonoBehaviour {

    public BaseComponent component;
    public GameObject selection;

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

    public void Update()
    {
        if (selec == null) // the component has been deleted
            Destroy(this.gameObject);
    }

}
