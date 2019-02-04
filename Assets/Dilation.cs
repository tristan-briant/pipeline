using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dilation : MonoBehaviour
{
    BaseComponent bc;
    Material material;

    protected void Start()
    {
        Material mat = GetComponent<Image>().material;

        material = GetComponent<Image>().material = new Material(mat);

        bc = GetComponentInParent<BaseComponent>();
    }

    private void Update()
    {
       material.SetFloat("_DilationCoefficent", bc.pressure);
    }
}
