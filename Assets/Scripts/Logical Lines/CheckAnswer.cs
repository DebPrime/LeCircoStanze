using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DIALOGUE;

public class CheckAnswer : MonoBehaviour
{
    public InputPanel inputPanel;
    [SerializeField] private TextAsset answerFile;
    public TMP_Text narratorText;
    public string question;
    public GameObject[] buttonsToShow;
    private string rightAnswer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Try to get InputPanel from assigned reference or manager
        if (inputPanel == null)
        {
            inputPanel = InputPanelManager.Instance.GetInputPanel();
        }

        if (inputPanel == null)
        {
            Debug.LogError("InputPanel not found! Make sure there's an InputPanel GameObject in the scene with the InputPanel script attached, or assign it in the Inspector.");
            return;
        }

        // Carica la risposta corretta dal file di testo
        if (answerFile != null)
        {
            rightAnswer = answerFile.text.Trim();
        }
        else
        {
            Debug.LogError("Answer file not assigned! Please assign a TextAsset in the Inspector.");
            return;
        }

        if (buttonsToShow != null)
        {
            foreach (GameObject buttonObject in buttonsToShow)
            {
                if (buttonObject != null)
                    buttonObject.SetActive(false);
            }
        }
    }

    public void StartAnswerCheck()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        bool correctAnswer = false;
        
        inputPanel.Show();
        
        while (!correctAnswer)
        {
            // Aspetta che l'utente fornisca un input
            while (!inputPanel.hasNewInput)
                yield return null;
            
            string characterName = inputPanel.lastInput.ToLower();
            string rightAnswerLower = rightAnswer.ToLower();
            
            if (characterName == rightAnswerLower)
            {
                correctAnswer = true;
                inputPanel.HideRetryText();
                Debug.Log($"Risposta corretta! Nice to meet you, {characterName}");
                inputPanel.AcceptAndHide(); // Nasconde il pannello quando la risposta è corretta
                DialogueSystem.instance?.OnInputComplete();
                DialogueSystem.instance.dialogueContainer.dialogueText.text = "Sia fatta la vostra volontà";

                if (buttonsToShow != null)
                {
                    foreach (GameObject buttonObject in buttonsToShow)
                    {
                        if (buttonObject != null)
                            buttonObject.SetActive(true);
                    }
                }
            }
            else
            {
                inputPanel.ShowRetryText();
                Debug.Log($"Risposta sbagliata: '{characterName}'. Riprova!");
                // Resetta per il prossimo tentativo
                inputPanel.ResetForNextAttempt();
            }
        }
    }
}
