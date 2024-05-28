using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Main : MonoBehaviour
{
    public enum DataReceiveMode {
        keyboard,
        serialPort,
        bluetooth,
    }
    public DataReceiveMode Mode;
    public string userName = "user0";
    public enum StudyMode {
        practice1,
        study1,
    }
    public StudyMode studyMode;
    public string dataReadPath; // TODO: change path in the inspector
    public string dataWritePath; //TODO: change path in the inspector

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
