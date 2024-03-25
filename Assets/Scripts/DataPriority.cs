using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPriority : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (DataReceiver.stroke != DataReceiver.Stroke.synchronous) {
            // check DataReceiver.FootDataList, if [1] > [2], then [2] = 0, and vice versa
            if (DataReceiver.footDataList[1] > DataReceiver.footDataList[2]) {
                DataReceiver.footDataList[2] = 0;
            } else {
                DataReceiver.footDataList[1] = 0;
            }

            // check DataReceiver.FootDataList, if [3] > [4], then [4] = 0, and vice versa
            if (DataReceiver.footDataList[3] > DataReceiver.footDataList[4]) {
                DataReceiver.footDataList[4] = 0;
            } else {
                DataReceiver.footDataList[3] = 0;
            }
        // }
    }
}
