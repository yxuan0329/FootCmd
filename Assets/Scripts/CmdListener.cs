using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class CMDListen
{
    public string tag;
    public UnityEvent<string> unityEvent;

    public void CallCMDFunction(string cmd){
        //Debug.Log("Call" + cmd);
        if(tag == cmd){
            this.unityEvent.Invoke(cmd);
        }
    }
}

public class CmdListener : MonoBehaviour
{
    public List<CMDListen> CMDlisten;
    private UpdateCMD updateCMD;
    // Start is called before the first frame update
    void Start()
    {
        if(updateCMD == null){
            updateCMD = new UpdateCMD();
        }
        foreach (CMDListen l in CMDlisten) // Correct use of foreach for iteration
        {
            updateCMD.AddListener(l.CallCMDFunction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pass(string cmd){
        Debug.Log(cmd);
        if (updateCMD != null){
            updateCMD.Invoke(cmd);
        }
    }
}
