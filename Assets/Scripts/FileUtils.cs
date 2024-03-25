using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class FileUtils
{
    public static void CreateDictionaryFromFile(string filePath, ref Dictionary<string, string> mp)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(','); // split by ","

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    mp.Add(key, value);
                }
                else
                {
                    Debug.Log("Invalid line format: " + line);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error reading file: " + ex.Message);
        }
    }

    // shuffle by lines in a file and write to another file
    public void ShuffleFile(string filePath, string newFilePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            List<string> shuffledLines = new List<string>();

            foreach (string line in lines)
            {
                shuffledLines.Add(line);
            }

            // shuffle
            System.Random rng = new System.Random();
            int n = shuffledLines.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = shuffledLines[k];
                shuffledLines[k] = shuffledLines[n];
                shuffledLines[n] = value;
            }

            // write to new file
            File.WriteAllLines(newFilePath, shuffledLines.ToArray());
        }
        catch (Exception ex)
        {
            Debug.Log("Error reading file: " + ex.Message);
        }
    }
}
