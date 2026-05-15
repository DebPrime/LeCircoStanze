using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INPUT;

namespace DIALOGUE
{

    public class ConversationManager
    {
        private DialogueSystem dialogueSystem = DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        private TextArchitect architect = null;
        private InputContainer inputContainer = null;
        private bool userPrompt = false;
        private bool backRequested = false;
        private bool waitingForInput = false;
        private bool waitingAtInputBar = false;
        private bool inputCompleted = false;
        private int currentLineIndex = 0;

        public ConversationManager(TextArchitect architect, InputContainer inputContainer)
        {
            this.architect = architect;
            this.inputContainer = inputContainer;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
            dialogueSystem.onUserPrompt_Back += OnUserPrompt_Back;
            dialogueSystem.onInputComplete += OnInputComplete;
        }

        private void OnUserPrompt_Next()
        {
            if (architect != null && architect.isBuilding)
            {
                userPrompt = true;
                backRequested = false;
                return;
            }

            if (waitingForInput)
            {
                userPrompt = true;
                backRequested = false;
                return;
            }

            if (waitingAtInputBar)
            {
                // Non fare nulla, l'input bar gestisce il completamento tramite CheckAnswer.
            }
        }

        private void OnUserPrompt_Back()
        {
            if (architect != null && architect.isBuilding)
            {
                userPrompt = true;
                backRequested = false;
                return;
            }

            if (waitingForInput)
            {
                userPrompt = true;
                backRequested = true;
                return;
            }

            if (waitingAtInputBar)
            {
                backRequested = true;
            }
        }

        private void OnInputComplete()
        {
            inputCompleted = true;
            waitingAtInputBar = false;
        }
        
        public void StartConversation(List<string> conversation)
        {
            StopConversation();
            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
        }
        
        public void StopConversation()
        {
            if(!isRunning)
                return;
            dialogueSystem.StopCoroutine(process);
            process = null;
        }
        IEnumerator RunningConversation(List<string> conversation)
        {
            currentLineIndex = 0;
            while (currentLineIndex < conversation.Count)
            {
                //don't show any blank lines or try to run any logic on them
                if (string.IsNullOrWhiteSpace(conversation[currentLineIndex]))
                {
                    currentLineIndex++;
                    continue;
                }

                DIALOGUE_LINE line = DialogueParser.Parse(conversation[currentLineIndex]);

                if (line.hasDialogue && !line.hasSpeaker && line.dialogue == "END")
                {
                    inputContainer.root.SetActive(true);
                    Debug.Log("activated inputbar");
                    
                    // Avvia il controllo risposta
                    CheckAnswer checkAnswerScript = Object.FindObjectOfType<CheckAnswer>();
                    if (checkAnswerScript != null)
                    {
                        checkAnswerScript.StartAnswerCheck();
                    }
                    else
                    {
                        CheckMultipleAnswer multipleAnswerScript = Object.FindObjectOfType<CheckMultipleAnswer>();
                        if (multipleAnswerScript != null)
                        {
                            multipleAnswerScript.StartAnswerCheck();
                        }
                        else
                        {
                            Debug.LogWarning("CheckAnswer o CheckMultipleAnswer script non trovato in scena!");
                        }
                    }

                    waitingAtInputBar = true;
                    inputCompleted = false;

                    while (waitingAtInputBar && !inputCompleted)
                    {
                        if (backRequested)
                        {
                            backRequested = false;
                            int previousIndex = FindPreviousDialogueLineIndex(conversation, currentLineIndex);
                            if (previousIndex >= 0)
                            {
                                currentLineIndex = previousIndex;
                                waitingAtInputBar = false;
                                break;
                            }
                        }
                        yield return null;
                    }

                    if (inputCompleted)
                    {
                        process = null;
                        yield break;
                    }
                    continue;
                }

                //Show dialogue
                if (line.hasDialogue)
                    yield return Line_RunDialogue(line);

                if (backRequested)
                {
                    backRequested = false;
                    int previousIndex = FindPreviousDialogueLineIndex(conversation, currentLineIndex);
                    if (previousIndex >= 0)
                    {
                        currentLineIndex = previousIndex;
                        continue;
                    }

                    // Se siamo al primo dialogo e viene premuto Back, ignoralo e rimani sullo stesso testo.
                    yield return WaitForUserInput();
                    continue;
                }

                //Run any commands
                if (line.hasCommands)
                    yield return Line_RunCommands(line);

                currentLineIndex++;
                yield return new WaitForSeconds(1);
            }
            process = null;
        }
        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            //Show or hide the speaker name if there is one present.
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speaker);

            //Build dialogue
            yield return BuildDialogue(line.dialogue);

            //Wait for user input
            yield return WaitForUserInput();
        }
        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            Debug.Log(line.commands);
            yield return null;
        }
        IEnumerator BuildDialogue(string dialogue)
        {
            architect.Build(dialogue);
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                    userPrompt = false;
                }
                yield return null;
            }
        }
        IEnumerator WaitForUserInput()
        {
            waitingForInput = true;
            while(!userPrompt)
                yield return null;
            waitingForInput = false;
            userPrompt=false;
        }

        private int FindPreviousDialogueLineIndex(List<string> conversation, int startIndex)
        {
            for (int i = startIndex - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;
                DIALOGUE_LINE line = DialogueParser.Parse(conversation[i]);
                if (line.hasDialogue)
                    return i;
            }
            return -1;
        }
    }

}