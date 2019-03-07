using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WatchManager : BaseComponent {

    protected Animation Animation;
    public float timeOut = 4.0f;
    public float TimeOut { get => timeOut; set => timeOut = value; }

    protected override void Start()
    {
        base.Start();
        configPanel = Resources.Load("ConfigPanel/ConfigWatch") as GameObject;
    }

    public override void OnClick()
    {
        audios[2].Play();
        Animation = GetComponent<Animation>();
        Animation["WatchAnimation"].speed = 4 / timeOut;
        Animation.Play("WatchAnimation");

        GameObject.Find("Playground").BroadcastMessage("TriggerStart", SendMessageOptions.DontRequireReceiver);
        GameObject.Find("CanvasDragged").BroadcastMessage("TriggerStart", SendMessageOptions.DontRequireReceiver);

        Invoke("EndTrig", timeOut);
    
    }

    void EndTrig()
    {
        GameObject.Find("Playground").BroadcastMessage("TriggerEnd", SendMessageOptions.DontRequireReceiver);
        GameObject.Find("CanvasDragged").BroadcastMessage("TriggerEnd", SendMessageOptions.DontRequireReceiver);

    }
    
}
