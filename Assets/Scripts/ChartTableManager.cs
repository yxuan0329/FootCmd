using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartTableManager : MonoBehaviour
{
    public ChartManager leftFront, leftRear, rightFront, rightRear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        leftFront.AddData(DataReceiver.footDataList[1], 600);
        leftRear.AddData(DataReceiver.footDataList[2], 600);
        rightFront.AddData(DataReceiver.footDataList[3], 600);
        rightRear.AddData(DataReceiver.footDataList[4], 600);
    }
}
