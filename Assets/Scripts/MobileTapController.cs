using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class MobileTapController : MonoBehaviour
{
    public enum MobileTapMode {
        training,
        testing
    }
    public AssignedTextController assignedTextController;
    public MobileTapMode mobileTapMode = MobileTapMode.training;
    public GameObject background, FootDotPrefab, FootDot2Prefab, linePrefab;
    public Transform FootDotContainer, LineContainer;
    public Text enteredText, alphaCodes, state, clockTimer, hitRateText, modeText;
    public float lineWidth, offset, dotOff;
    public Button btn1, btn2, btn3, btn4;
    List<bool> clickedButton = new List<bool>();
    SGReceiver sgLeftFoot, sgRightFoot;

    string alphaCode = ""; // use a number string to represent the line connection
    int curr = 0, last = 0;

    // check whether the foot is tapping
    bool LeftForeisTapping = false, LeftRearisTapping = false, RightForeisTapping = false, RightRearisTapping = false;

    List<int> leftList, rightList;

    int[] hasFootIcon = new int[6]; // 0: no dot, 1: has dot-1, 2: has dot-2
    int[] hasArrowIcon = new int[6];  //  whether the arrow is generated, 0: not generated, 1: generated one

    float Timer = 0.0f, recogTimer = 0.0f, currClock = 0.0f, lastClock = 0.0f, syncThreshold = 0.1f;
    bool isSynchronous = false;
    public enum State {
        Idle,
        Tapping,
        Recognize
    }
    private State currState;

    private Vector3[] dotCoordinates = new Vector3[6];
    private Vector3[] dotOffset = new Vector3[9];
    private int sortingOrder = 0;
    private int hitCount = 0, missCount = 0;
    private float hitRate = 0.0f;
    
    // change the alphacode to alphabet
    Dictionary<string, string> mp = new Dictionary<string, string>();
    
    // Start is called before the first frame update
    void Start()
    {
        // initialize the leftlist as length=2, value = 0
        leftList = new List<int>();
        rightList = new List<int>();
        for (int i=0; i<2; i++) {
            leftList.Add(0);
            rightList.Add(0);
        }


        Timer = 0.0f;
        recogTimer = 0.0f;

        // arrowNumber = all 0
        for (int i=0; i<6; i++) {
            clickedButton.Add(false);
            hasArrowIcon[i] = 0;
            hasFootIcon[i] = 0;
            dotCoordinates[i] = new Vector3(0f, 0f, 0f);
        }

        // dotOffset = 0
        for (int i=0; i<9; i++) {
            dotOffset[i] = new Vector3(0f, 0f, 0f);
        }

        // set the four coordinates of the dot
        dotCoordinates[1] = new Vector3(0f, 0f, 0f);
        dotCoordinates[2] = new Vector3(0f, -100f, 0f);
        dotCoordinates[3] = new Vector3(100f, 0f, 0f);
        dotCoordinates[4] = new Vector3(100f, -100f, 0f);

        // set the four offset of the dot
        dotOffset[1] = new Vector3(-dotOff, 0f, 0f);
        dotOffset[2] = new Vector3(0f, -dotOff, 0f);
        dotOffset[3] = new Vector3(dotOff, 0f, 0f);
        dotOffset[4] = new Vector3(0f, -dotOff, 0f);
        dotOffset[5] = new Vector3(0f, dotOff, 0f);
        dotOffset[6] = new Vector3(-dotOff, 0f, 0f);
        dotOffset[7] = new Vector3(0f, dotOff, 0f); 
        dotOffset[8] = new Vector3(dotOff, 0f, 0f);

        // button listener
        btn1.onClick.AddListener(delegate { CheckButtonClicked(1, ref leftList, ref rightList); });
        btn2.onClick.AddListener(delegate { CheckButtonClicked(2, ref leftList, ref rightList); });
        btn3.onClick.AddListener(delegate { CheckButtonClicked(3, ref leftList, ref rightList); });
        btn4.onClick.AddListener(delegate { CheckButtonClicked(4, ref leftList, ref rightList); });
        
        FileUtils.CreateDictionaryFromFile("Assets/Files/chartList.txt", ref mp);
        modeText.text = mobileTapMode.ToString();
        currState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {   
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
        
        hitRateText.text = hitRate.ToString("F2") + "%";
        clockTimer.text = recogTimer.ToString("F3"); // 3 digits
        // syncTimer.text = (currClock - lastClock).ToString();
        Timer += Time.deltaTime;
        currClock = Timer;

        // if press r, then go to idle state
        if (Input.GetKeyDown(KeyCode.R)) {
            ChangeState(State.Idle);
        }

        // if press t, then go to tapping state
        if (Input.GetKeyDown(KeyCode.T)) {
            ChangeState(State.Tapping);
        }
    }

    void UpdateIdleState() 
    {
        background.SetActive(true);
        state.text = "Idle";
        
        // clock reset
        recogTimer = 0.0f;

        // clear all arrow
        curr = 0;
        last = 0;
        alphaCode = "";
        
        // ClearAllArrow();
        ClearAllChildren(FootDotContainer, ref hasFootIcon);
        ClearAllChildren(LineContainer);
        enteredText.text = "";
        isSynchronous = false;

        // set all the arrowNumber to 0
        for (int i=0; i<6; i++) {
            hasArrowIcon[i] = 0;
        }

        // if there is a tapping, change the state to tapping
        TappingIconChecker(leftList, rightList);
    }

    void UpdateTappingState() 
    {
        background.SetActive(true);
        state.text = "Tapping";
        recogTimer += Time.deltaTime;

        // add a new arrow according to the tapping
        alphaCodes.text = alphaCode;

        // if there is a tapping, change the state to tapping
        TappingIconChecker(leftList, rightList);
        
        // if there is a tapping then wait for 50 frames, then change the state to recognize
        if (recogTimer >= 1.0f){
            ChangeState(State.Recognize);
        }
    }

    void UpdateRecogState()
    {
        background.SetActive(false);
        state.text = "Recognize";
        
        // recognize the code and show the text
        if (mp.TryGetValue(alphaCode, out string alphabet)) {
            // if alphabet == assignedText, then set color black, else set color red
            if (alphabet == assignedTextController.assignedText.text) {
                enteredText.color = Color.black;
                hitCount++;
            } else {
                enteredText.color = Color.red;
                missCount++;
            }
            enteredText.text = alphabet;
            
        } else {
            enteredText.text = "";
            missCount++;
        }
        // calculate the hit rate
        hitRate = (float)hitCount / (float)(hitCount + missCount) * 100.0f;
        
        // stay 50 frames in this state
        recogTimer += Time.deltaTime;
        if (recogTimer >= 1.5f) {
            // if entered text == assigned text, then hasGivenWord = false
            if (enteredText.text == assignedTextController.assignedText.text) {
                assignedTextController.hasGivenWord = false;
            }
            ChangeState(State.Idle);
        }
    }

    void ChangeState(State newState) 
    {
        currState = newState;
    }

    void CheckFootisUp(int value, ref bool isTapping, int newTap, int threshold=550) {
        if (value > threshold && !isTapping) {
            isTapping = true;
        }
        if (isTapping) { // then this moment is tapping (foot down)
            recogTimer = 0.0f;

            updateCurr(ref curr, ref last, newTap, ref currClock, ref lastClock, ref isSynchronous);
            ArrowChecker(curr, last);
            GenerateFootDot();
            isTapping = false;
            ChangeState(State.Tapping);
        }
    }

    void TappingIconChecker(List<int> leftList, List<int> rightList) {
        CheckFootisUp(leftList[0], ref LeftForeisTapping, 1, 500);
        CheckFootisUp(leftList[1], ref LeftRearisTapping, 2, 500);
        CheckFootisUp(rightList[0], ref RightForeisTapping, 3, 590);
        CheckFootisUp(rightList[1], ref RightRearisTapping, 4, 500);    
    }

    void GenerateFootDot()
    {
        // if leftfore is tapping, then generate a dot prefab and put it in the leftfore position
        if (LeftForeisTapping && hasFootIcon[1] == 0)
        {
            DrawDot(1);
            return;
        }

        // if leftrear is tapping, then generate a dot prefab and put it in the leftrear position
        if (LeftRearisTapping && hasFootIcon[2] == 0)
        {
            DrawDot(2);
            return;
        }

        // if rightfore is tapping, then generate a dot prefab and put it in the rightfore position
        if (RightForeisTapping && hasFootIcon[3] == 0)
        {
            DrawDot(3);
            return;
        }

        // if rightrear is tapping, then generate a dot prefab and put it in the rightrear position
        if (RightRearisTapping && hasFootIcon[4] == 0)
        {
            DrawDot(4);
            return;
        }

        // ------------------ generate dot-2 prefab ------------------
        // if leftFore is Tapping and generated[0] == true, then generate dot-2 prefab and put it in the leftfore position
        if (LeftForeisTapping && hasFootIcon[1] == 1)
        {

            if (last == 2  && hasArrowIcon[0]>1)
            {
                DrawDot(1, 2);
            }
            else if (last == 3  && hasArrowIcon[3]>1) 
            {
                DrawDot(1, 3, 1);
            }
        }

        // if leftRear is Tapping and generated[1] == true, then generate dot-2 prefab and put it in the leftrear position
        if (LeftRearisTapping && hasFootIcon[2] == 1)
        {
            if (last == 1  && hasArrowIcon[0]>1) 
            {
                DrawDot(2, 1);
            }
            else if (last == 4  && hasArrowIcon[1]>1) 
            {
                DrawDot(2, 4, 1);
            }
        }

        // if rightFore is Tapping and generated[2] == true, then generate dot-2 prefab and put it in the rightfore position
        if (RightForeisTapping && hasFootIcon[3] == 1)
        {
            if (last == 1  && hasArrowIcon[3]>1)
            {
                DrawDot(3, 1, 1);
            }
            else if (last == 4 && hasArrowIcon[2]>1)
            {
                DrawDot(3, 4);
            }
        }

        // if rightRear is Tapping and generated[3] == true, then generate dot-2 prefab and put it in the rightrear position
        if (RightRearisTapping && hasFootIcon[4] == 1)
        {
            if (last == 2 && hasArrowIcon[1]>1)
            {
                DrawDot(4, 2, 1);
            }
            else if (last == 3  && hasArrowIcon[2]>1)
            {
                DrawDot(4, 3);
            }
        }
        leftList[0] = 0;
        leftList[1] = 0;
        rightList[0] = 0;
        rightList[1] = 0;
    }

    void updateCurr(ref int curr, ref int last, int nowTapping, ref float currClock, ref float lastClock, ref bool isSynchronous) {
        last = curr;
        curr = nowTapping;
        
        // if the last clock and curr clock is within the threshold, then it is synchronous
        CheckisSynchronous();
        lastClock = currClock;
    }

    void CheckisSynchronous() {
        if (Mathf.Abs(currClock - lastClock) <= syncThreshold) isSynchronous = true;
        else isSynchronous = false;
    }

    void ArrowChecker(int end, int start) {
        if (isSynchronous) {
            // if 1 and 2
            if ((start ==1 && end == 2) || (start == 2 && end == 1)) {
                DrawLine(new Vector3(0, 0, 0), new Vector3(0, -100, 0), true);
            }
            // if 1 and 3
            else if ((start == 1 && end == 3) || (start == 3 && end == 1)) {
                DrawLine(new Vector3(0, 0, 0), new Vector3(100, 0, 0), true);
            }
            // if 1 and 4
            else if ((start == 1 && end == 4) || (start == 4 && end == 1)) {
                DrawLine(new Vector3(0, 0, 0), new Vector3(100, -100, 0), true);
            }
            // if 2 and 3
            else if ((start == 2 && end == 3) || (start == 3 && end == 2)) {
                DrawLine(new Vector3(0, -100, 0), new Vector3(100, 0, 0), true);
            }
            // if 2 and 4
            else if ((start == 2 && end == 4) || (start == 4 && end == 2)) {
                DrawLine(new Vector3(0, -100, 0), new Vector3(100, -100, 0), true);
            }
            // if 3 and 4
            else if ((start == 3 && end == 4) || (start == 4 && end == 3)) {
                DrawLine(new Vector3(100, 0, 0), new Vector3(100, -100, 0), true);
            }
            return;
        }
        if (start == 1) {
            if (end == 2) {
                if (hasArrowIcon[0] == 1) {
                    DrawLine(new Vector3(0 - offset, 0, 0), new Vector3(0 - offset, -100, 0));
                    hasArrowIcon[0] = 2;
                }
                else if (hasArrowIcon[0] == 0) {
                    hasArrowIcon[0] = 1;
                    DrawLine(new Vector3(0, 0, 0), new Vector3(0, -100, 0));
                }
                alphaCode += "12";
            }
            else if (end == 3) {
                if (hasArrowIcon[3] == 1) {
                    DrawLine(new Vector3(0, 0 + offset, 0), new Vector3(100, 0+offset, 0));
                    hasArrowIcon[3] = 2;
                }
                else if (hasArrowIcon[3] == 0) {
                    hasArrowIcon[3] = 1;
                    DrawLine(new Vector3(0, 0, 0), new Vector3(100, 0, 0));
                }
                alphaCode += "13";
            }
            else if (end == 4) {
                hasArrowIcon[4] = 1;
                DrawLine(new Vector3(0, 0, 0), new Vector3(100, -100, 0));
                alphaCode += "14";
            }
        }
        if (start == 2) {
            if (end == 1) {
                if (hasArrowIcon[0] == 1) {
                    DrawLine(new Vector3(0 - offset, -100, 0), new Vector3(0 - offset, 0, 0));
                    hasArrowIcon[0] = 2;
                }
                else if (hasArrowIcon[0] == 0) {
                    DrawLine(new Vector3(0, -100, 0), new Vector3(0, 0, 0));
                    hasArrowIcon[0] = 1;
                }
                alphaCode += "21";
            }
            else if (end == 3) {
                DrawLine(new Vector3(0-5f, -100, 0), new Vector3(100-5f, 0, 0));
                hasArrowIcon[5] = 1;
                alphaCode += "23";
            }
            else if (end == 4) {
                if (hasArrowIcon[1] == 1) {
                    DrawLine(new Vector3(0, -100 - offset, 0), new Vector3(100, -100 - offset, 0));
                    hasArrowIcon[1] = 2;
                }
                else if (hasArrowIcon[1] == 0) {
                    hasArrowIcon[1] = 1;
                    DrawLine(new Vector3(0, -100, 0), new Vector3(100, -100, 0));
                }
                alphaCode += "24";
            }
        }
        if (start == 3) {
            if (end == 1) {
                if (hasArrowIcon[3] == 1) {
                    DrawLine(new Vector3(100, 0 + offset, 0), new Vector3(0, 0 + offset, 0));
                    hasArrowIcon[3] = 2;
                }
                else if (hasArrowIcon[3] == 0) {
                    hasArrowIcon[3] = 1;
                    DrawLine(new Vector3(100, 0, 0), new Vector3(0, 0, 0));
                }
                alphaCode += "31";
            }
            else if (end == 2) {
                if (hasArrowIcon[5] == 1) {
                    DrawLine(new Vector3(100 + offset-5, 0, 0), new Vector3(0 + offset-5, -100, 0));
                    hasArrowIcon[5] = 2;
                }
                else if (hasArrowIcon[5] == 0) {
                    hasArrowIcon[5] = 1;
                    DrawLine(new Vector3(100, 0, 0), new Vector3(0, -100, 0));
                }
                // a32.SetActive(true);
                // hasArrowIcon[5] = 1;
                alphaCode += "32";
            }
            else if (end == 4) {
                if (hasArrowIcon[2] == 1) {
                    DrawLine(new Vector3(100 + offset, 0, 0), new Vector3(100 + offset, -100, 0));
                    hasArrowIcon[2] = 2;
                }
                else if (hasArrowIcon[2] == 0) {
                    hasArrowIcon[2] = 1;
                    DrawLine(new Vector3(100, 0, 0), new Vector3(100, -100, 0));
                }
                alphaCode += "34";
            }
        }
        if (start == 4) {
            if (end == 1) {
                DrawLine(new Vector3(100, -100, 0), new Vector3(0, 0, 0));
                hasArrowIcon[4] = 1;
                alphaCode += "41";
            }
            else if (end == 2) {
                if (hasArrowIcon[1] == 1) {
                    DrawLine(new Vector3(100, -100 - offset, 0), new Vector3(0, -100 - offset, 0));
                    hasArrowIcon[1] = 2;
                }
                else if (hasArrowIcon[1] == 0) {
                    hasArrowIcon[1] = 1;
                    DrawLine(new Vector3(100, -100, 0), new Vector3(0, -100, 0));
                }
                alphaCode += "42";
            }
            else if (end == 3) {
                if (hasArrowIcon[2] == 1) {
                    DrawLine(new Vector3(100 + offset, -100, 0), new Vector3(100 + offset, 0, 0));
                    hasArrowIcon[2] = 2;
                }
                else if (hasArrowIcon[2] == 0) {
                    hasArrowIcon[2] = 1;
                    DrawLine(new Vector3(100, -100, 0), new Vector3(100, 0, 0));
                }
                alphaCode += "43";
            }
        }
    }

    
    void DrawLine(Vector3 start, Vector3 end, bool isSynchronous = false) {
        // draw gradient line in line renderer
        GameObject lineObject = Instantiate<GameObject>(linePrefab, LineContainer);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        
        lineRenderer.sortingOrder = sortingOrder;
        sortingOrder++;

        // if not synchronous, then set the color to gradient yellow to orange
        if (isSynchronous == false) {
            Color orange = new Color(1.0f, 0.5f, 0.0f);
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(orange, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );

            lineRenderer.colorGradient = gradient;
        }
        // else, set the color to yellow
        else {
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
        }

        // width
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineObject.SetActive(true);
    }

    // draw a dot in the foot dot container
    void DrawDot(int curr, int last=0, int rotate=0) {
        // if hasDotIcon ==0, then generate a dot prefab and put it in the position
        if (last == 0 && hasFootIcon[curr] == 0) {
            GameObject dotObject = Instantiate<GameObject>(FootDotPrefab, FootDotContainer);
            
            // set the position of the dot from the dotCoordinates
            Vector3 dotPosition = dotCoordinates[curr];
            dotObject.transform.localPosition = dotPosition;

            dotObject.SetActive(true);
            hasFootIcon[curr] = 1;
        }
        else 
        {
            GameObject dotObject = Instantiate<GameObject>(FootDotPrefab, FootDotContainer);
            if (last % 2 == 0) {
                dotObject.transform.localPosition = dotCoordinates[curr] + 2 * dotOffset[curr];
            } 
            else {
                dotObject.transform.localPosition = dotCoordinates[curr] + 2 * dotOffset[curr+4];
            }
            
            dotObject.SetActive(true);
            hasFootIcon[curr] = 3;

            GameObject dot2Object = Instantiate<GameObject>(FootDot2Prefab, FootDotContainer);
            if (rotate == 1) {
                dot2Object.transform.Rotate(0f, 0f, 90f);
            }

            // if last is even, then the dot offset is the first four
            // else, the dot offset is the last four
            if (last % 2 == 0) {
                dot2Object.transform.localPosition = dotCoordinates[last] + dotOffset[curr];
            }
            else {
                dot2Object.transform.localPosition = dotCoordinates[last] + dotOffset[curr+4];
            }

            dot2Object.SetActive(true);
            hasFootIcon[last] = 2;
        }
        
    }

    // press four button as the foot tap, if button is clicked, then the foot is tapping
    public void CheckButtonClicked(int footNumber, ref List<int> leftList, ref List<int> rightList) {
        if (footNumber == 1) {
            leftList[0] = 1000;
        }
        else if (footNumber == 2) {
            leftList[1] = 1000;
        }
        else if (footNumber == 3) {
            rightList[0] = 1000;
        }
        else if (footNumber == 4) {
            rightList[1] = 1000;
        }
    }

    private void ClearAllChildren(Transform parent, ref int[] hasFootIcon) {
        foreach (Transform child in parent) {
            DestroyImmediate(child.gameObject);
        }

        // for each element in generated, set it to false
        for (int i=0; i<6; i++) {
            hasFootIcon[i] = 0;
        }
    }

    void ClearAllChildren(Transform parent) {
        foreach (Transform child in parent) {
            DestroyImmediate(child.gameObject);
        }
    }
}
