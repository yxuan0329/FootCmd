﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class CmdController : MonoBehaviour
{
    public Main main;
    public Canvas footIcon;
    public GameObject leftFore, leftRear, rightFore, rightRear; // icon for foot
    public GameObject background, CirclePrefab, FootDotPrefab, IconBackground;
    public Transform FootDotContainer, IconContainer;
    public Text enteredText, alphaCodes, state;
    public DataReceiver DataReceiver;
    private string alphaCode = ""; // use a number string to represent the line connection
    private int curr = 0, last = 0;
    private int[] hasDotIcon = new int[6]; 
    private bool showedIcon = false;
    private GameObject dotObject, circleObject, dotIconObject, circleIconObject;

    // check whether the foot is up
    private bool LeftForeisUp = false, LeftRearisUp = false, RightForeisUp = false, RightRearisUp = false;

    // check whether the foot is tapping
    private bool LeftForeisTapping = false, LeftRearisTapping = false, RightForeisTapping = false, RightRearisTapping = false;
    private int[] hasFootIcon = new int[6]; // 0: no dot, 1: has dot-1, 2: has dot-2
    private int[,] hasArrowIcon = new int[6, 6]; // 0: no arrow, 1: has arrow-1, 2: has arrow-2
    private float Timer = 0.0f, recogTimer = 0.0f, currClock = 0.0f, lastClock = 0.0f, syncThreshold = 0.1f; // timer 
    private bool isSynchronous = false;
    public enum State {
        Start,
        Idle,
        Tapping,
        Recognize
    }
    private State currState;
    private Vector3[] dotCoordinates = new Vector3[6];
    private Vector3[] dotOffsets = new Vector3[9];
    private int TapNumber = 0;
    private Dictionary<string, string> alphaDict;
    
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0.0f;
        recogTimer = 0.0f;

        // initialize the dot value and offset
        ResetDotsandArrows();
        InitDotCoordinates(ref dotCoordinates);
        CreateDict();

        currState = State.Idle;
    }

    void CreateDict()
    {
        // create Dict<string, string> for the alpha code
        alphaDict = new Dictionary<string, string>();

        // read D:\_xuan\UserStudy1\StudyTask\tasks_all.txt
        string[] lines = File.ReadAllLines(main.dataReadPath + "tasks_all.txt");
        foreach (string line in lines)
        {
            string[] words = line.Split(',');
            alphaDict.Add(words[0], words[1]);
        }
        Debug.Log(alphaDict.Count + " alpha codes are loaded.");
    }

    void ResetDotsandArrows() {
        for (int i=0; i<6; i++) {
            for (int j=0; j<6; j++) hasArrowIcon[i, j] = 0;
            hasFootIcon[i] = 0;
        }
        TapNumber = 0;
    }

    void InitDotCoordinates(ref Vector3[] dotCoordinates)
    {
        // set the four coordinates of the dot
        dotCoordinates[1] = new Vector3(0f, 0f, 0f);
        dotCoordinates[2] = new Vector3(0f, -100f, 0f);
        dotCoordinates[3] = new Vector3(100f, 0f, 0f);
        dotCoordinates[4] = new Vector3(100f, -100f, 0f);

        // set the four offset of the dot
        dotOffsets[1] = new Vector3(-50f, 0f, 0f);
        dotOffsets[2] = new Vector3(0f, -50f, 0f);
        dotOffsets[3] = new Vector3(50f, 0f, 0f);
        dotOffsets[4] = new Vector3(0f, -50f, 0f);
        dotOffsets[5] = new Vector3(0f, 50f, 0f);
        dotOffsets[6] = new Vector3(-50f, 0f, 0f);
        dotOffsets[7] = new Vector3(0f, 50f, 0f); 
        dotOffsets[8] = new Vector3(50f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        DataReceiver.ReadInputToFootDataList();

        // check the state
        switch (currState) 
        {
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Tapping:
                UpdateTappingState();
                break;
            case State.Recognize:
                UpdateRecogState();
                break;
            default:
                Debug.LogError("Unknown state!");
                break;
        }

        Timer += Time.deltaTime;
        currClock = Timer;

        // if press r, then go to idle state
        if (Input.GetKeyDown(KeyCode.R)) {
            ChangeState(State.Idle);
        }
    }

    void UpdateIdleState() 
    {
        background.SetActive(true);
        IconBackground.SetActive(false);
        state.text = "Idle";
        
        // clock reset
        recogTimer = 0.0f;

        // clear all arrow
        curr = 0;
        last = 0;
        alphaCode = "";
        
        ClearAllChildren(FootDotContainer);
        ClearAllChildren(IconContainer);
        enteredText.text = "";
        isSynchronous = false;

        // set all the arrowNumber to 0
        ResetDotsandArrows();

        // if there is a tapping, change the state to tapping
        TappingIconChecker();

        showedIcon  = false;
    }

    void UpdateTappingState() 
    {
        background.SetActive(true);
        state.text = "Tapping";
        recogTimer += Time.deltaTime;

        // add a new arrow according to the tapping
        alphaCodes.text = alphaCode;

        // if there is a tapping, change the state to tapping
        TappingIconChecker();
        
        // if there is a tapping then wait for 50 frames, then change the state to recognize
        if (recogTimer >= 1.0f){
            ChangeState(State.Recognize);
        }
    }

    void UpdateRecogState()
    {
        state.text = "Recognize";
        enteredText.text = alphaCode;

        // check if alphacode is inside the dictionary
        if (alphaDict.ContainsKey(alphaCode)) {
            // stay 1 second in this state
            // background.SetActive(false);

            if (showedIcon == false) {
                ShowIcon();
                showedIcon = true;
            }

            recogTimer += Time.deltaTime;
            if (recogTimer >= 2.0f) {
                ClearAllChildren(IconContainer);
                ClearAllChildren(FootDotContainer);
                foreach (GameObject child in IconContainer)
                {
                    Destroy(child);
                }
                IconBackground.SetActive(false);
                ChangeState(State.Idle);
            }
        } else {
            ClearAllChildren(IconContainer);
            ClearAllChildren(FootDotContainer);
            foreach (GameObject child in IconContainer)
            {
                Destroy(child);
            }
            ChangeState(State.Idle);
        }
    }

    void ShowIcon()
    {
        // draw the icon according to the alpha code on assigned text
            string assignedDotPosition = alphaDict[alphaCode];
            ClearAllChildren(IconContainer);
            IconBackground.SetActive(true);
            int order = 0;
            int pos = 1, lastPos = 1;
            for (int i=0; i<assignedDotPosition.Length; i++)
            {
                if (char.IsLetter(assignedDotPosition[i]))
                {
                    DrawDot((int)assignedDotPosition[i] - 96, order);
                    pos = (int)assignedDotPosition[i] - 96;
                }
                else
                {
                    DrawDot(int.Parse(assignedDotPosition[i].ToString()), ++order);
                    pos = int.Parse(assignedDotPosition[i].ToString());
                }

                lastPos = pos;
            }
    }

    public void ChangeState(State newState) 
    {
        currState = newState;
    }

    void CheckFootisUp(ref bool isUp, ref bool isTapping, int newTap,ref GameObject img, int threshold=550) {
        if (DataReceiver.footDataList[newTap] > threshold) {
            isUp = true;
            img.SetActive(false);
        } else {
            isUp = false;
            img.SetActive(true);
        }

        if (isUp && !isTapping) {
            isTapping = true;
        }
        if (!isUp && isTapping) { // then this moment is tapping (foot down)
            UpdateCurr(ref curr, ref last, newTap, ref currClock, ref lastClock, ref isSynchronous);
            GenerateAlphaCode(curr, last);
            DrawUserDot(curr);

            isTapping = false;
        }
    }

    void DrawDot(int pos, int order = 0) {   
        if (order == 2)
        {
            circleIconObject = Instantiate<GameObject>(CirclePrefab, IconContainer);
            circleIconObject.transform.localPosition = dotCoordinates[pos];
            hasDotIcon[pos]++;
        }
        else
        {
            dotIconObject = Instantiate<GameObject>(FootDotPrefab, IconContainer);
            dotIconObject.transform.localPosition = dotCoordinates[pos];
            hasDotIcon[pos]++;
        }
    }

    void TappingIconChecker() {
        CheckFootisUp(ref LeftForeisUp, ref LeftForeisTapping, 1, ref leftFore, 600);
        CheckFootisUp(ref LeftRearisUp, ref LeftRearisTapping, 2,ref leftRear, 600);
        CheckFootisUp(ref RightForeisUp, ref RightForeisTapping, 3, ref rightFore, 600);
        CheckFootisUp(ref RightRearisUp, ref RightRearisTapping, 4, ref rightRear, 600);    

        // if one of the foot is tapping, change the state to tapping
        if (LeftForeisUp || LeftRearisUp || RightForeisUp || RightRearisUp)
        {
            recogTimer = 0.0f;
            ChangeState(State.Tapping);
        }
    }

    void UpdateCurr(ref int curr, ref int last, int nowTapping, ref float currClock, ref float lastClock, ref bool isSynchronous) {
        last = curr;
        curr = nowTapping;
        
        // if the last clock and curr clock is within the threshold, then it is synchronous
        CheckisSynchronous();
        lastClock = currClock;
    }

    void CheckisSynchronous() {
        if (Mathf.Abs(currClock - lastClock) <= syncThreshold) isSynchronous = true;
        else {
            isSynchronous = false;
            TapNumber++;
        }
    }

    void GenerateAlphaCode(int end, int start) {
        if (isSynchronous) {
            if (start > end) { 
                int temp = start;
                start = end;
                end = temp;
            }
            alphaCode = RemoveLastCharacter(alphaCode);
            alphaCode += start.ToString() + ((char)('a' + end - 1)).ToString();
            return;
        }
        else
        {
            // not synchronous
            alphaCode += end.ToString();
        }
    }
    string RemoveLastCharacter(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        else
        {
            return str.Substring(0, str.Length - 1);
        }
    }

    // draw a dot in the foot dot container
    void DrawUserDot(int curr) {
        if (isSynchronous && hasFootIcon[curr] == TapNumber) return;
        if (TapNumber == 2) 
        {
            circleObject = Instantiate<GameObject>(CirclePrefab, FootDotContainer);
            circleObject.transform.localPosition = dotCoordinates[curr]; // + hasFootIcon[curr] * new Vector3(dotOffset, 0f, 0f);
            hasFootIcon[curr]++;
            circleObject.SetActive(true);
        }
        else if (TapNumber == 1) 
        {
            dotObject = Instantiate<GameObject>(FootDotPrefab, FootDotContainer);
            dotObject.transform.localPosition = dotCoordinates[curr]; // + hasFootIcon[curr] * new Vector3(dotOffset, 0f, 0f);
            hasFootIcon[curr]++;
            dotObject.SetActive(true);
        }   
        else if (TapNumber >= 3) 
        {
            ClearAllChildren(FootDotContainer);
            if (circleObject != null) {
                DestroyImmediate(circleObject);
            }
        }
    }

    void ClearAllChildren(Transform parent) {
        foreach (Transform child in parent) {
            DestroyImmediate(child.gameObject);
        }
    }
}