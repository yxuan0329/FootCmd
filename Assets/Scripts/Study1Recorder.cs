﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Study1Recorder : MonoBehaviour
{
    public Main Main;
    public AssignedTextController assignedTextController;
    public static bool RecordResult = false;
    public bool shouldRecord = false, hasError = false, recordRound = false;
    public float totalTime = 0.0f, executionTime = 0.0f, thinkingTime = 0.0f, roundTime = 0.0f;
    public string assignedStroke = "", usersActions = "";
    public int id = 1, round = 1;
    public int roundCorrects = 0, roundErrors = 0, roundTrials = 0;
    private string filePath, filePath2;

    // Start is called before the first frame update
    void Start()
    {
        // check if the study scene is loaded
        if (SceneManager.GetSceneByName("Study1").isLoaded)
        {
            RecordResult = true;
        }

        // if the study scene is not loaded, do not record the data
        if (RecordResult == false)
        {
            return;
        }
        else
        {
            if (Main.studyMode == Main.StudyMode.practice1)
            {
                filePath = Main.dataWritePath + Main.userName + "_rawData_practice.csv";
                filePath2 = Main.dataWritePath + Main.userName + "_rounds_practice.csv";
            }
            else
            {
                filePath = Main.dataWritePath + Main.userName + "_rawData.csv";
                filePath2 = Main.dataWritePath + Main.userName + "_rounds.csv";
            }

            string[] studyInfo = new string[2] {Main.userName, Main.studyMode.ToString()};
            string studyInfoString = string.Join(",", studyInfo);
            System.IO.File.WriteAllText(filePath, studyInfoString + "\n");
            
            string[] columnNames = new string[7] {"id", "stroke", "totalTime", "thinkingTime", "executionTime", "error", "usersActions,"};
            string columnNamesString = string.Join(",", columnNames);
            System.IO.File.WriteAllText(filePath, columnNamesString + "\n");

            string[] columnNames2 = new string[5] {"Round", "CompletionTime", "Tasks", "Correct", "Incorrect,"};
            string columnNamesString2 = string.Join(",", columnNames2);
            System.IO.File.WriteAllText(filePath2, columnNamesString2 + "\n");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // check if the study scene is loaded
        if (RecordResult == false)
        {
            return;
        }
        
        // start from line 2, because line 1 is the column header
        // write the data to the file
        if (shouldRecord)
        {
            string[] writeLine = new string[7]; 
            writeLine[0] = id.ToString();
            writeLine[1] = assignedStroke;
            writeLine[2] = totalTime.ToString();
            writeLine[3] = thinkingTime.ToString();
            executionTime = executionTime - thinkingTime; // todo
            writeLine[4] = executionTime.ToString();
            writeLine[5] = hasError ? "1" : "0";
            writeLine[6] = usersActions;
            string newLine = string.Join(",", writeLine);
            System.IO.File.AppendAllText(filePath, newLine + "\n");

            if (hasError == true)
            {
                roundErrors++;
            }
            else
            {
                roundCorrects++;
            }
            roundTrials++;

            id++;
            shouldRecord = false;
            hasError = false;
            usersActions = "";
            roundTime += totalTime;
            thinkingTime = 0.0f;
            executionTime = 0.0f;
        } 

        if (assignedTextController.hasGivenWord)
        {
            totalTime += Time.deltaTime;
            executionTime += Time.deltaTime;
        }
        else
        {
            totalTime = 0.0f;
            thinkingTime = 0.0f;
            executionTime = 0.0f;
        } 

        if (recordRound)
        {
            string[] writeLine = new string[5];
            writeLine[0] = round.ToString();
            writeLine[1] = roundTime.ToString();
            writeLine[2] = roundTrials.ToString();
            writeLine[3] = roundCorrects.ToString();
            writeLine[4] = roundErrors.ToString();
            string newLine = string.Join(",", writeLine);
            System.IO.File.AppendAllText(filePath2, newLine + "\n");

            round ++;
            recordRound = false;
            roundTime = 0.0f;
            roundTrials = 0;
            roundCorrects = 0;
            roundErrors = 0;
        }   
    }
}
