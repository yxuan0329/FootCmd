using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestionnaireToolkit.Scripts;

public class StartQTQuestionnaire : MonoBehaviour
{
    public GameObject questionnaireManager;
    // Start is called before the first frame update
    void Start()
    {
        questionnaireManager.GetComponent<QTQuestionnaireManager>().StartQuestionnaire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
