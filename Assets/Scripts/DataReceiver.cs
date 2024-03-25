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
    public Button btn1, btn2, btn3, btn4;
    private string filePath;
    // public static Dictionary<string, string> mp = new Dictionary<string, string>(); // change the alphacode to alphabet
    // Start is called before the first frame update
    void Start()
    {      
        // add data into mp, according to th stroke mode
        // LoadDictionaryFromStroke();

        if (Main.Mode == Main.DataReceiveMode.serialPort) {
            // bind the SG receiver with the tag
            sgLeftFoot = GameObject.FindGameObjectWithTag("leftFootData").GetComponent<SGReceiver>();
            sgRightFoot = GameObject.FindGameObjectWithTag("rightFootData").GetComponent<SGReceiver>();
        }
        if (Main.Mode == Main.DataReceiveMode.mobile) {
            // button listener
            btn1.onClick.AddListener(delegate { CheckButtonClicked(1); });
            btn2.onClick.AddListener(delegate { CheckButtonClicked(2); });
            btn3.onClick.AddListener(delegate { CheckButtonClicked(3); });
            btn4.onClick.AddListener(delegate { CheckButtonClicked(4); });     
        }

        if (Main.studyMode == Main.StudyMode.study1)
        {
            filePath = "D:/_xuan/UserStudy1/StudyResult/" + Main.userName + "/"+ Main.userName + "_frame.csv";
        }
        else
        {
            filePath = "D:/_xuan/UserStudy1/StudyResult/" + Main.userName + "/"+ Main.userName + "_frame_practice.csv";
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

        if (Main.Mode == Main.DataReceiveMode.serialPort) {
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
    private void LoadDictionaryFromStroke()
    {
        // switch (Main.stroke) {
        //     case Main.Stroke.synchronous:
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/syncList.txt", ref mp);
        //         break;
        //     case Main.Stroke.asynchronousTwoStep:
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/asyncTwoStepList.txt", ref mp);
        //         break;
        //     case Main.Stroke.asynchronousThreeStep:
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/asyncThreeStepList.txt", ref mp);
        //         break;
        //     case Main.Stroke.asynchronousAlphabet:
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/alphabetList.txt", ref mp);
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/numberList.txt", ref mp);
        //         break;
        //     case Main.Stroke.study1:
        //         FileUtils.CreateDictionaryFromFile("Assets/Files/strokeList.txt", ref mp);
        //         break;
        // }
        // Debug.Log("mp.Count: " + mp.Count);
    }
    
    private void CheckButtonClicked(int footNumber) 
    {
        DataReceiver.footDataList[footNumber] = 1000;
    }
}