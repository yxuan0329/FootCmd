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
        mobile
    }
    public DataReceiveMode Mode;
    public enum Stroke {
        synchronous,
        asynchronousTwoStep,
        asynchronousThreeStep,
        asynchronousAlphabet,
        study1
    }
    public Stroke stroke;
    public string userName = "user0";
    public enum StudyMode {
        practice1,
        study1,
    }
    public StudyMode studyMode;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
