using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class SGReceiver : MonoBehaviour
{
    public string comName; // com name of serial port
    int baudrate = 115200;
    SerialPort MainSerialPort;
    Thread myThread;
    private string[] getCommandLine;
    public List<int> footSGdata = new List<int>();
    float getStartClock = 0.0f;
    public static bool isKeyboardMode = false;
    public Main main;

    // Start is called before the first frame update
    void Start()
    {
        try 
        {
            MainSerialPort = new SerialPort(comName, baudrate);
            MainSerialPort.Open(); // open serial port for receiving data
            myThread = new Thread(new ThreadStart(ReadArduinoCommandLine)); 
            myThread.Start(); // start a new thread for receiving data
        }
        catch (System.Exception e)
        {
            Debug.Log("Serial port open failed: " + e.Message);
            SwitchToKeyboardMode();
        }

        for (int i=0; i<2; i++)
            footSGdata.Add(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (main.Mode == Main.DataReceiveMode.serialPort)
        {
            if(getStartClock <= 3.0f)
            {
                getStartClock += Time.deltaTime;
            }else{
                footSGdata[0] = int.Parse(getCommandLine[0]);   // single fore foot
                footSGdata[1] = int.Parse(getCommandLine[1]);   // single rear foot
            }
        }
    }

    private void ReadArduinoCommandLine()
    {
        while(myThread.IsAlive && MainSerialPort.IsOpen) // if serial is open then constantly read the line
        {
            try
            {
                getCommandLine = MainSerialPort.ReadLine().Split(','); // read the arduino command line, separate the data fore, rear
            }
            catch(InvalidCastException e) 
            {
                getCommandLine = new string[]{"0","0","0","0"}; // not work
            }
        }
    }

    private void SwitchToKeyboardMode()
    {
        Debug.Log("Switch to keyboard mode");
        main.Mode = Main.DataReceiveMode.keyboard;
    }

    private void OnApplicationQuit() {
        MainSerialPort.Close();      
        myThread.Abort();   
    }
}
