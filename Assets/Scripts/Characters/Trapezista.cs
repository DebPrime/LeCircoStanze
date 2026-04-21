using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;

public class Trapezista : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartConversation();
    }

    // Update is called once per frame
    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset("Trapezista", false);
        DialogueSystem.instance.Say(lines);
    }
}
