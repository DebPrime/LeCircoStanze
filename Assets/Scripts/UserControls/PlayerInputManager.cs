using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DIALOGUE{

public class PlayerInputManager : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            PromptAdvance();

    }
    public void PromptAdvance()
    {
        DialogueSystem.instance.OnUserPrompt_Next();
    }
    public void PromptReturn()
    {
        DialogueSystem.instance.OnUserPrompt_Back();
    }
}
}
