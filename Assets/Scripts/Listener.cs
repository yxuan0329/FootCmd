using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class Listen
{
    public int val;
    public UnityEvent unityEvent;

    public void OnEnable(int code){
        if(val == code){
            this.unityEvent.Invoke();
        }
    }
}

public class Listener : MonoBehaviour
{
    public List<Listen> listen;
    private CallListener callfunction;
    // Start is called before the first frame update
    void Start()
    {
        if(callfunction == null){
            callfunction = new CallListener();
        }
        foreach (Listen l in listen) // Correct use of foreach for iteration
        {
            callfunction.AddListener(l.OnEnable);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && listen != null)
        {
            foreach (Listen l in listen) // Correct use of foreach for iteration
            {
                l.unityEvent.Invoke();
            }
        }
    }

    public void pass(int cmd){
        if (callfunction != null){
            callfunction.Invoke(cmd);
        }
    }
}
