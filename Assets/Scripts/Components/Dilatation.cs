using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dilatation : MonoBehaviour
{
    BaseComponent bc;
    Material material;
    public int index = 0; // 

    protected void Start()
    {
        Material mat = GetComponent<Image>().material;

        material = GetComponent<Image>().material = new Material(mat);

        bc = GetComponentInParent<BaseComponent>();
    }

    private void Update()
    {
       material.SetFloat("_DilationCoefficent", bc.GetPressure(index));
    }
}
