using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
///////VIDEO 03(pt2) 25:41

namespace DIALOGUE{
public class DialogueParser
{
    private const string commandRegexPattern="\\w*";
    public static DIALOGUE_LINE Parse(string rawLine)
        {
            Debug.Log($"Parsing line - '{rawLine}'");
            (string speaker, string dialogue, string commands)=RipContent(rawLine);

            return new DIALOGUE_LINE(speaker,dialogue,commands);
        }
        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker="", dialogue="", commands="";
            int dialogueStart =-1;
            int dialogueEnd=-1;
            bool isEscaped=false;

            for(int i = 0; i < rawLine.Length; i++)
            {
                char current=rawLine[i];
                if (current == '\\')
                    isEscaped=!isEscaped;
                else if (current == '"' && !isEscaped)
                {
                    if(dialogueStart==-1)
                        dialogueStart=i;
                    else if(dialogueEnd==-1)
                    dialogueEnd=i;
                }
                else
                    isEscaped=false;
            }
            //Identify Command Pattern
            
            
            return (speaker,dialogue,commands);
        }
}
}