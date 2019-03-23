using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuccessValueManager : MonoBehaviour
{

    public float value;
    const float scoreSpeed = 2;

    float score = 0;
    protected BaseComponent component;
    protected Animator animator;

    private void Start()
    {
        component = GetComponentInParent<BaseComponent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        value = component.success;

        if (score < value)
        {
            score += Time.deltaTime * scoreSpeed;
            if (score > value) score = value;
        }
        else if (score > value)
        {
            score -= Time.deltaTime * scoreSpeed;
            if (score < 0) score = 0;
        }

        if (score == 1)
            animator.SetTrigger("success");
        else
        {
            animator.ResetTrigger("success");
            animator.SetFloat("value", score);
        }


        transform.GetComponent<Text>().text = Mathf.RoundToInt(score * 100) + "%";
    }
}
