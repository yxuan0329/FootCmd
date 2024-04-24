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
        serialPort
    }
    public DataReceiveMode Mode;
    public string userName = "user0";
    public enum StudyMode {
        practice1,
        study1,
    }
    public StudyMode studyMode;
    public string dataReadPath = "D:/_xuan/StudyTask/"; // TODO: change your path
    public string dataWritePath = "D:/_xuan/StudyResult/"; //TODO: change your path

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
