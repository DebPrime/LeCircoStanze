using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING{
public class Testing_Architect : MonoBehaviour
{
    DialogueSystem ds;
    TextArchitect architect;

    string[] lines=new string[5]{
        "ciao sono davide",
        "ora gianna",
        "ora unstable smp",
        "123",
        "567"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ds= DialogueSystem.instance;
        architect=new TextArchitect(ds.dialogueContainer.dialogueText);
        architect.buildMethod=TextArchitect.BuildMethod.typewriter;
    }

    // Update is called once per frame
    void Update()
    {
        string longLine="this is a very long line that makes no sense but I am just populating it with stuff because, you know, stuff is good right? I like stuff, you like stuff, we all like stuff and the turkey gets stuffed.";
        if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp=true;
                    else
                        architect.ForceComplete();
                }
                else
                    //architect.Build(lines[Random.Range(0,lines.Length)]); 
                    architect.Build(longLine);
            }
        
        else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
                //architect.Append(lines[Random.Range(0, lines.Length)]);
            }
    }
}
}
