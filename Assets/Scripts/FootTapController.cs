using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class FootTapController : MonoBehaviour
{
    public Study1Recorder study1Recorder;
    public AssignedTextController assignedTextController;
    public Canvas canvas;
    public Canvas footIcon;
    public GameObject leftFore, leftRear, rightFore, rightRear; // icon for foot
    public GameObject background, CirclePrefab, FootDotPrefab;
    public Transform FootDotContainer;
    public Text enteredText, alphaCodes, state, clockTimer, correctionText;
    public DataReceiver DataReceiver;
    public Text countDownText;
    private string alphaCode = ""; // use a number string to represent the line connection
    private int curr = 0, last = 0;
    private GameObject dotObject;

    // check whether the foot is up
    private bool LeftForeisUp = false, LeftRearisUp = false, RightForeisUp = false, RightRearisUp = false;

    // check whether the foot is tapping
    private bool LeftForeisTapping = false, LeftRearisTapping = false, RightForeisTapping = false, RightRearisTapping = false;
    private int[] hasFootIcon = new int[6]; // 0: no dot, 1: has dot-1, 2: has dot-2
    private float Timer = 0.0f, recogTimer = 0.0f, currClock = 0.0f, lastClock = 0.0f, syncThreshold = 0.1f; // timer 
    private bool isSynchronous = false;
    public enum State
    {
        Start,
        Idle,
        Tapping,
        Recognize
    }
    private State currState;
    private Vector3[] dotCoordinates = new Vector3[6];
    private int TapNumber = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        Timer = 0.0f;
        recogTimer = 0.0f;

        // initialize the dot value and offset
        ResetDotsandArrows();
        InitDotCoordinates(ref dotCoordinates);
        footIcon.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        assignedTextController.state = AssignedTextController.State.study;
        currState = State.Start;
    }

    void ResetDotsandArrows()
    {
        for (int i=0; i<6; i++) {
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
    }

    // Update is called once per frame
    void Update()
    {
        DataReceiver.ReadInputToFootDataList();

        // if press space bar, then close the start button
        if (currState == State.Start && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CountdownCoroutine());
        }

        // check the state
        switch (currState) 
        {
            case State.Start:
                UpdateStartState();
                break;
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

        clockTimer.text = recogTimer.ToString("F3"); // 3 digits
        Timer += Time.deltaTime;
        currClock = Timer;

        // if press r, then go to idle state
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeState(State.Idle);
        }
    }

    void UpdateStartState() 
    {
        state.text = "Start";
    }

    void UpdateIdleState() 
    {
        background.SetActive(true);
        correctionText.text = "";
        state.text = "Idle";
        
        // clock reset
        recogTimer = 0.0f;
        study1Recorder.thinkingTime += Time.deltaTime;

        // clear all arrow
        curr = 0;
        last = 0;
        alphaCode = "";
        
        ClearAllChildren(FootDotContainer);
        enteredText.text = "";
        isSynchronous = false;

        // set all the arrowNumber to 0
        ResetDotsandArrows();

        // if there is a tapping, change the state to tapping
        TappingIconChecker();
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
        
        // if there is no more tapping in 1 second, then change the state to recognize
        if (recogTimer >= 1.0f){
            ChangeState(State.Recognize);
        }
    }

    void UpdateRecogState()
    {
        background.SetActive(false);
        state.text = "Recognize";
        
        enteredText.text = alphaCode;
        if (isCorrect(alphaCode))
        {
            ShowCorrectionText("O");
            
        }
        else
        {
            ShowCorrectionText("X");
        }
        
        // stay 1 second in this state
        recogTimer += Time.deltaTime;
        if (recogTimer >= 1.5f)
        {
            // add the alpha code to the users actions
            study1Recorder.usersActions += alphaCode + ",";
            study1Recorder.shouldRecord = true;
            study1Recorder.assignedStroke = assignedTextController.assignedText.text; // todo: -> mp[]
            
            // if entered text == assigned text, then hasGivenWord = false
            if (isCorrect(alphaCode))
            {
                assignedTextController.correctedCount++;
            }
            else 
            {
                // add the wrong word into the wrong word list
                assignedTextController.incorrectTrials.Add(assignedTextController.assignedText.text);
            }
            // turn to next given word
            assignedTextController.hasGivenWord = false;
            ChangeState(State.Idle);
        }
    }

    void ChangeState(State newState) 
    {
        currState = newState;
    }

    void CheckFootisUp(ref bool isUp, ref bool isTapping, int newTap,ref GameObject img, int threshold=550)
    {
        if (DataReceiver.footDataList[newTap] > threshold)
        {
            isUp = true;
            img.SetActive(false);
        }
        else
        {
            isUp = false;
            img.SetActive(true);
        }

        if (isUp && !isTapping)
        {
            isTapping = true;
        }
        if (!isUp && isTapping)
        { 
            // then this moment is tapping (foot down)
            UpdateCurr(ref curr, ref last, newTap, ref currClock, ref lastClock);
            GenerateAlphaCode(curr, last);
            DrawWhiteDot(curr);

            isTapping = false;
        }
    }

    void TappingIconChecker()
    {
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

    void UpdateCurr(ref int curr, ref int last, int nowTapping, ref float currClock, ref float lastClock)
    {
        last = curr;
        curr = nowTapping;
        
        // if the last clock and curr clock is within the threshold, then it is synchronous
        CheckisSynchronous();
        lastClock = currClock;
    }

    void CheckisSynchronous()
    {
        if (Mathf.Abs(currClock - lastClock) <= syncThreshold)
        {
            isSynchronous = true;
        }
        else
        {
            isSynchronous = false;
            TapNumber++;
        }
    }

    void GenerateAlphaCode(int end, int start)
    {
        if (isSynchronous)
        {
            // swap the start and end if start > end
            if (start > end)
            { 
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
    void DrawWhiteDot(int curr)
    {
        if (isSynchronous && hasFootIcon[curr] == TapNumber)
        {
            Debug.Log(TapNumber);
            return;
        }   
        if (TapNumber >= 3) 
        {
            ClearAllChildren(FootDotContainer);
            if (dotObject != null)
            {
                DestroyImmediate(dotObject);
            }
        }
        if (TapNumber == 1) dotObject = Instantiate<GameObject>(FootDotPrefab, FootDotContainer);
        else if (TapNumber == 2) dotObject = Instantiate<GameObject>(CirclePrefab, FootDotContainer);

        dotObject.transform.localPosition = dotCoordinates[curr];
        hasFootIcon[curr]++;
        dotObject.SetActive(true);
    }

    void ClearAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void ShowCorrectionText(string str)
    {
        if (str == "O")
        {
            correctionText.color = Color.black;
        }
        else
        { // str == "X"
            correctionText.color = Color.red;
            study1Recorder.hasError = true;
        }
        correctionText.text = str;
    }

    bool isCorrect(string enteredText)
    {
        return enteredText == assignedTextController.assignedText.text;
    }

    IEnumerator CountdownCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countDownText.text = "Start!";
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        footIcon.gameObject.SetActive(true);
        assignedTextController.state = AssignedTextController.State.study;
        ChangeState(State.Idle);
    }
}