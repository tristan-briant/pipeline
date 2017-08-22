using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WatchManager : BaseComponent {

    protected Animation Animation;
    public float timeOut=4.0f;

    protected void Awake()
    {
        Animation = GetComponent<Animation>();
        Animation["WatchAnimation"].speed = 4 / timeOut;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        audios[2].Play();
        Animation.Play("WatchAnimation");

        broadcastTrigger();
    }


    public void broadcastTrigger() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Triggerable")) {
            go.transform.GetComponent<BaseComponent>().trigged=true;
        }

    }
    
}
