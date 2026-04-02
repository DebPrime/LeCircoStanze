using UnityEngine;
using DIALOGUE;

namespace TESTING{
public class TestParsing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string line="Speaker \"Dialogue Goes in here\"";
        DialogueParser.Parse(line);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}