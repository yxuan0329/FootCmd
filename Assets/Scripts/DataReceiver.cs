using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class DataReceiver : MonoBehaviour
{
    public static float[] footDataList = new float[6]; // store SG data from the SG receiver
    public SGReceiver sgLeftFoot, sgRightFoot;
    public Main Main;   
    OneEuroFilter OEfilter1 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter2 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter3 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter4 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    public Text studyModeText;
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {      
        if (Main.Mode == Main.DataReceiveMode.serialPort) {
            // bind the SG receiver with the tag
            sgLeftFoot = GameObject.FindGameObjectWithTag("leftFootData").GetComponent<SGReceiver>();
            sgRightFoot = GameObject.FindGameObjectWithTag("rightFootData").GetComponent<SGReceiver>();
        }

        if (Main.studyMode == Main.StudyMode.study1)
        {
            filePath = Main.dataWritePath + Main.userName + "/"+ Main.userName + "_frame.csv";
        }
        else
        {
            filePath = Main.dataWritePath + Main.userName + "/"+ Main.userName + "_frame_practice.csv";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float[] datainEachFrame = new float[9];
        datainEachFrame[0] = Time.time;
        datainEachFrame[1] = footDataList[1];
        datainEachFrame[2] = footDataList[2];
        datainEachFrame[3] = footDataList[3];
        datainEachFrame[4] = footDataList[4];
        datainEachFrame[5] = (float)sgLeftFoot.footSGdata[0];
        datainEachFrame[6] = (float)sgLeftFoot.footSGdata[1];
        datainEachFrame[7] = (float)sgRightFoot.footSGdata[0];
        datainEachFrame[8] = (float)sgRightFoot.footSGdata[1];
        string newLine = string.Join(",", datainEachFrame);
        System.IO.File.AppendAllText(filePath, newLine + "\n");
    }

    public void ReadInputToFootDataList()
    {
        void SetFootData(KeyCode key, int index)
        {
            footDataList[index] = Input.GetKeyDown(key) ? 1000 : 0;
        }

        if (Main.Mode == Main.DataReceiveMode.serialPort)
        {
            footDataList[1] = OEfilter1.Filter((float)sgLeftFoot.footSGdata[0]);
            footDataList[2] = OEfilter2.Filter((float)sgLeftFoot.footSGdata[1]);
            footDataList[3] = OEfilter3.Filter((float)sgRightFoot.footSGdata[0]);
            footDataList[4] = OEfilter4.Filter((float)sgRightFoot.footSGdata[1]);           
        }
        else if (Main.Mode == Main.DataReceiveMode.keyboard)
        {
            SetFootData(KeyCode.Keypad7, 1);
            SetFootData(KeyCode.Keypad1, 2);
            SetFootData(KeyCode.Keypad9, 3);
            SetFootData(KeyCode.Keypad3, 4);
        }
    }
}