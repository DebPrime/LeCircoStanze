using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DIALOGUE;

public class CheckMultipleAnswer : MonoBehaviour
{
    public InputPanel inputPanel;
    [SerializeField] private TextAsset[] answerFiles;
    [SerializeField] private TextAsset[] followupDialogueFiles;
    public GameObject[] buttonsToShow;
    public TMP_Text[] questionTexts;

    private List<string> rightAnswers = new List<string>();
    private Dictionary<string, int> answerIndexMap = new Dictionary<string, int>();
    private int currentQuestionIndex = 0;

    void Start()
    {
        if (inputPanel == null)
            inputPanel = InputPanelManager.Instance?.GetInputPanel();

        if (inputPanel == null)
        {
            Debug.LogError("InputPanel non trovato! Assicurati che ci sia un GameObject con il componente InputPanel nella scena o assegnalo nell'Inspector.");
            return;
        }

        if (answerFiles == null || answerFiles.Length == 0)
        {
            Debug.LogError("Manca il file delle risposte! Assegna almeno un TextAsset nell'Inspector.");
            return;
        }

        InitializeQuestionTexts();
        ToggleButtons(false);
    }

    public void StartAnswerCheck()
    {
        if (!LoadAnswerFile(currentQuestionIndex))
            return;
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        if (inputPanel == null)
            yield break;

        inputPanel.Show();
        List<string> remainingAnswers = new List<string>(rightAnswers);

        while (remainingAnswers.Count > 0)
        {
            while (!inputPanel.hasNewInput)
                yield return null;

            string userInput = inputPanel.lastInput.Trim().ToLower();

            if (remainingAnswers.Contains(userInput))
            {
                remainingAnswers.Remove(userInput);
                int answerIndex = answerIndexMap[userInput];
                MarkAnswerAsCorrect(answerIndex);
                Debug.Log($"Corretto! Risposte rimaste: {remainingAnswers.Count}");
                inputPanel.HideRetryText();
                if (remainingAnswers.Count > 0)
                    inputPanel.ResetForNextAttempt();
            }
            else
            {
                inputPanel.ShowRetryText();
                inputPanel.ResetForNextAttempt();
                Debug.Log($"Risposta errata: '{userInput}'. Riprova!");
            }
        }

        inputPanel.AcceptAndHide();

        bool startedNextDialogue = TryStartNextDialogueFile(currentQuestionIndex);
        currentQuestionIndex++;
        if (startedNextDialogue)
            yield break;

        DialogueSystem.instance?.OnInputComplete();
        yield return StartCoroutine(TypeTextCoroutine("Sia fatta la vostra volontà"));
        ToggleButtons(true);
    }

    private bool LoadAnswerFile(int index)
    {
        rightAnswers.Clear();
        answerIndexMap.Clear();

        if (answerFiles == null || answerFiles.Length == 0)
        {
            Debug.LogError("Manca il file delle risposte! Assegna almeno un TextAsset nell'Inspector.");
            return false;
        }

        if (index < 0 || index >= answerFiles.Length)
        {
            Debug.LogError($"Nessun file di risposte disponibile per l'indice {index}. Assicurati che answerFiles abbia abbastanza elementi.");
            return false;
        }

        TextAsset file = answerFiles[index];
        if (file == null)
        {
            Debug.LogError($"answerFiles[{index}] è null.");
            return false;
        }

        string[] lines = file.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string cleanLine = lines[i].Trim().ToLower();
            if (!string.IsNullOrEmpty(cleanLine))
            {
                rightAnswers.Add(cleanLine);
                answerIndexMap[cleanLine] = i;
            }
        }

        if (rightAnswers.Count == 0)
        {
            Debug.LogWarning($"File di risposte '{file.name}' è vuoto o contiene solo righe vuote.");
        }

        InitializeQuestionTexts();
        return true;
    }

    private bool TryStartNextDialogueFile(int index)
    {
        if (followupDialogueFiles == null || followupDialogueFiles.Length == 0 || index >= followupDialogueFiles.Length)
            return false;

        TextAsset nextFile = followupDialogueFiles[index];
        if (nextFile == null)
        {
            Debug.LogWarning("Follow-up dialogue file is missing or null.");
            return false;
        }

        List<string> lines = FileManager.ReadTextAsset(nextFile, false);
        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning($"Follow-up dialogue file '{nextFile.name}' è vuoto o non può essere letto.");
            return false;
        }

        DialogueSystem.instance?.Say(lines);
        return true;
    }

    IEnumerator TypeTextCoroutine(string frase)
    {
        if (DialogueSystem.instance?.dialogueContainer?.dialogueText == null)
            yield break;

        TMP_Text textDisplay = DialogueSystem.instance.dialogueContainer.dialogueText;
        textDisplay.text = "";

        foreach (char lettera in frase)
        {
            textDisplay.text += lettera;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void ToggleButtons(bool state)
    {
        if (buttonsToShow == null)
            return;

        foreach (GameObject btn in buttonsToShow)
        {
            if (btn != null)
                btn.SetActive(state);
        }
    }

    private void InitializeQuestionTexts()
    {
        if (questionTexts == null)
            return;

        foreach (TMP_Text txt in questionTexts)
        {
            if (txt != null)
                txt.color = new Color(1f, 1f, 1f, 1f); //  bianco
        }
    }

    private void MarkAnswerAsCorrect(int index)
    {
        if (questionTexts != null && index >= 0 && index < questionTexts.Length && questionTexts[index] != null)
        {
            questionTexts[index].color = Color.green;
        }
    }
}