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
    public ArduinoHM10Test ArduinoHM10Left, ArduinoHM10Right;
    public Main Main;   
    OneEuroFilter OEfilter1 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter2 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter3 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);
    OneEuroFilter OEfilter4 = new OneEuroFilter(30.0f, 1.0f, 0.0f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {      
        if (Main.Mode == Main.DataReceiveMode.serialPort)
        {
            // bind the SG receiver with the tag
            sgLeftFoot = GameObject.FindGameObjectWithTag("leftFootData").GetComponent<SGReceiver>();
            sgRightFoot = GameObject.FindGameObjectWithTag("rightFootData").GetComponent<SGReceiver>();
        }      
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadInputToFootDataList()
    {
        void SetFootData(KeyCode key, int index)
        {
            footDataList[index] = Input.GetKeyDown(key) ? 1000 : 0;
        }

        if (Main.Mode == Main.DataReceiveMode.bluetooth)
        {
            AssignValueToFootDataList(ArduinoHM10Left, ref footDataList, 0);
            AssignValueToFootDataList(ArduinoHM10Right, ref footDataList, 2);
        }
        else if (Main.Mode == Main.DataReceiveMode.serialPort)
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

    private void AssignValueToFootDataList(ArduinoHM10Test arduinoHM10, ref float[] footDataList, int index)
    {
        if (arduinoHM10.BLEMessage.Contains("0"))
        {
            footDataList[1+index] = 0;
            footDataList[2+index] = 0;
        }
        if (arduinoHM10.BLEMessage.Contains("1"))
        {
            footDataList[1+index] = 1000;
            footDataList[2+index] = 0;
        }
        if (arduinoHM10.BLEMessage.Contains("2"))
        {
            footDataList[1+index] = 0;
            footDataList[2+index] = 1000;
        }
    }
}