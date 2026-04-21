using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputPanelTesting : MonoBehaviour
{
    public InputPanel inputPanel;
    private string rightAnswer= "Pippo";
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
            
            string characterName = inputPanel.lastInput;
            
            if (characterName.ToLower() == rightAnswer.ToLower())
            {
                correctAnswer = true;
                Debug.Log($"Risposta corretta! Nice to meet you, {characterName}");
                inputPanel.AcceptAndHide(); // Nasconde il pannello quando la risposta è corretta
            }
            else
            {
                Debug.Log($"Risposta sbagliata: '{characterName}'. Riprova!");
                // Resetta per il prossimo tentativo
                inputPanel.ResetForNextAttempt();
            }
        }
    }
}
