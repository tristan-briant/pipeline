using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuccessValueManager : MonoBehaviour
{

    public float value;
    const float scoreSpeed = 2;

    float score = 0;

    void Update()
    {
        
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

        transform.GetComponent<Text>().text = Mathf.RoundToInt(score * 100) + "%";
        transform.GetComponent<Text>().fontSize = (int)(10 + 20 * score);
        transform.localScale = Vector3.one * (1 + score * 0.2f * Mathf.Cos(1.5f * Mathf.PI * Time.time));
    }
}
