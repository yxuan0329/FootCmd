using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

public class AssignedTextController : MonoBehaviour
{
    public Main main;
    public Canvas canvas;
    public Study1Recorder study1Recorder;
    public Text assignedText, countDownText, breakText;
    public bool hasGivenWord = false;
    public GameObject FootDotPrefab, CirclePrefab;
    public Transform DotContainer;
    public List<string> incorrectTrials = new List<string>();
    public enum State { start, study, breaked, finished };
    public State state = State.start;
    private Vector3[] dotCoordinates = new Vector3[6];
    private int[] hasDotIcon = new int[6];  // 0 means no dot, 1 means has 1 dot, 2 means has 2 dots
    private int dataLineIndex = 0;
    private string[] lines;
    private string assignedAplhaCode, assignedDotPosition;
    private int round = 1, trials = 72;
    public int correctedCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        dotCoordinates[1] = new Vector3(0f, 0f, 0f);
        dotCoordinates[2] = new Vector3(0f, -100f, 0f);
        dotCoordinates[3] = new Vector3(100f, 0f, 0f);
        dotCoordinates[4] = new Vector3(100f, -100f, 0f);

        if (main.studyMode == Main.StudyMode.study1)
        {
            lines = File.ReadAllLines(main.dataReadPath + "tasks_" + main.userName + ".txt");
        }
        else
        {
            lines = File.ReadAllLines(main.dataReadPath + "practice_" + main.userName + ".txt");
            trials = 24;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            study1Recorder.recordRound = true;
            StartCoroutine(CountdownCoroutine());
            ChangeState(State.study);
        }

        switch (state)
        {   case State.start:
                break;
            case State.study:
                AssignStudyTask();
                break;
            case State.breaked:
                StudyBreak();
                break;
            case State.finished:
                break;
            default:
                break;
        }
    }

    void AssignStudyTask()
    {
        breakText.gameObject.SetActive(false);
        if (!hasGivenWord)
        {
            // set all hasDotIcon to 0
            for (int i = 0; i < 6; i++)
            {
                hasDotIcon[i] = 0;
            }

            // if dataLineIndex == lines.Length, then the study is finished
            if (correctedCount >= lines.Length && incorrectTrials.Count == 0)
            {
                StudyFinished();
            }
            else if (dataLineIndex >= round * trials)
            {
                if (incorrectTrials.Count > 0) DoIncorrectTrials();
                else {
                    StudyBreak();
                }
            }
            else
            {   
                Debug.Log("line: " + dataLineIndex + " (Round " + round + ")");
                string[] parts = lines[dataLineIndex].Split(',');
                assignedAplhaCode = parts[0].Trim();
                assignedDotPosition = parts[1].Trim();
                assignedText.text = assignedAplhaCode;
                dataLineIndex++;  
            

                // clear all the children in the container
                ClearAllChildren(DotContainer);

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
                hasGivenWord = true; 
            }          
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    void DrawDot(int pos, int order = 0)
    {   
        if (hasDotIcon[pos] == 1 || order == 2)
        {
            GameObject circleObject = Instantiate<GameObject>(CirclePrefab, DotContainer);
            circleObject.transform.localPosition = dotCoordinates[pos];
            hasDotIcon[pos]++;
        }
        else if (hasDotIcon[pos] == 0)
        {
            GameObject dotObject = Instantiate<GameObject>(FootDotPrefab, DotContainer);

            // set the position of the dot from the dotCoordinates
            dotObject.transform.localPosition = dotCoordinates[pos];
            hasDotIcon[pos]++;
        }
    }

    void ClearAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    void DoIncorrectTrials() {
        // pop first element from incorrectTrials
        assignedAplhaCode = incorrectTrials[0];
        assignedDotPosition = incorrectTrials[0];
        assignedText.text = assignedAplhaCode; 
        incorrectTrials.RemoveAt(0);

        // clear all the children in the container
        ClearAllChildren(DotContainer);

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
        hasGivenWord = true; 
    }

    void StudyBreak()
    {
        breakText.gameObject.SetActive(true);
        breakText.text = "Round " + round + " break!";
        canvas.gameObject.SetActive(false);
    }

    void StudyFinished()
    {
        study1Recorder.recordRound = true;
        Debug.Log("Study Finished!");
        canvas.gameObject.SetActive(false);
        countDownText.text = "Finished!";
        countDownText.gameObject.SetActive(true);
    }

    IEnumerator CountdownCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            countDownText.text = i.ToString();
            countDownText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        breakText.gameObject.SetActive(false);
        countDownText.text = "Start!";
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        round++;
        study1Recorder.totalTime = 0.0f;
        study1Recorder.executionTime = 0.0f;
        study1Recorder.thinkingTime = 0.0f;
    }
}