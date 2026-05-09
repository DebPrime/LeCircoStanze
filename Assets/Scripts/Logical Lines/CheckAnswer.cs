using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DIALOGUE;

public class CheckAnswer : MonoBehaviour
{
    public InputPanel inputPanel;
    [SerializeField] private TextAsset answerFile;
    public GameObject[] buttonsToShow;
    public TMP_Text[] questionTexts;

    private List<string> rightAnswers = new List<string>();
    private Dictionary<string, int> answerIndexMap = new Dictionary<string, int>();

    void Start()
    {
        if (inputPanel == null)
            inputPanel = InputPanelManager.Instance?.GetInputPanel();

        if (inputPanel == null)
        {
            Debug.LogError("InputPanel non trovato! Assicurati che ci sia un GameObject con il componente InputPanel nella scena o assegnalo nell'Inspector.");
            return;
        }

        if (answerFile != null)
        {
            string[] lines = answerFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string cleanLine = lines[i].Trim().ToLower();
                if (!string.IsNullOrEmpty(cleanLine))
                {
                    rightAnswers.Add(cleanLine);
                    answerIndexMap[cleanLine] = i;
                }
            }
        }
        else
        {
            Debug.LogError("Manca il file delle risposte! Assegna un TextAsset nell'Inspector.");
            return;
        }

        InitializeQuestionTexts();
        ToggleButtons(false);
    }

    public void StartAnswerCheck() => StartCoroutine(Running());

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
        DialogueSystem.instance?.OnInputComplete();
        yield return StartCoroutine(TypeTextCoroutine("Sia fatta la vostra volontà"));
        ToggleButtons(true);
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