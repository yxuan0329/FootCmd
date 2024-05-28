using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;
using UnityEngine.UI;

public class SGBLEReceive : MonoBehaviour
{
    public SGBLEReceive AnotherSG;
    public ArduinoHM10Test ArduinoHM10Test;
    public List<int> footSGdata = new List<int>();
    public bool isLeft = false, isConnected = false, isStarted = false;
    private float getStartClock = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(isLeft)
        {
            ArduinoHM10Test.StartProcess(true);
            isStarted = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLeft && AnotherSG.isConnected && !isStarted)
        {
            ArduinoHM10Test.StartProcess(false);
            isStarted = true;
        }

        if(getStartClock <= 3.0f)
        {
            getStartClock += Time.deltaTime;
        }
        else
        {
            if(ArduinoHM10Test._state == ArduinoHM10Test.States.Subscribe)
            {
                isConnected = true;
                if(ArduinoHM10Test.BLEMessage.Contains("1"))
                {
                    footSGdata[0] = 800;
                    footSGdata[1] = 10;

                }
                else if(ArduinoHM10Test.BLEMessage.Contains("2"))
                {
                    footSGdata[0] = 10; 
                    footSGdata[1] = 800;
                }
                else if(ArduinoHM10Test.BLEMessage.Contains("0"))
                {
                    footSGdata[0] = 10;
                    footSGdata[1] = 10;
                }
            }
            else
            {
                footSGdata[0] = 0;
                footSGdata[1] = 0;
            }
        }
    }

    IEnumerator OpenConnect(float time = 1)
    {
        yield return new WaitForSeconds(time);

        ArduinoHM10Test.StartProcess(true);
    }
}
