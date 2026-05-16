using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoginStuff : MonoBehaviour
{
    public InputPanel inputPanelId;
    public InputPanel inputPanelPw;
    [SerializeField] private TextAsset answerFile;
    public GameObject[] buttonsToShow;

    public string profId;
    public string profPw;
    public string[] studNames;

    void Start()
    {
        if (inputPanelId == null)
            inputPanelId = InputPanelManager.Instance?.GetInputPanel();

        if (inputPanelId == null)
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
            }
            studNames = new string[6]; //DA METTERE A POSTO
            studNames.Append(lines[0]);
            studNames.Append(lines[1]);
            studNames.Append(lines[2]);
            studNames.Append(lines[3]);
            studNames.Append(lines[4]);
            studNames.Append(lines[5]);
            //--------------------------------------------------------------------------------------------------------------------------------------------------
        }
        else
        {
            Debug.LogError("Manca il file delle risposte! Assegna un TextAsset nell'Inspector.");
            return;
        }

        ToggleButtons(false);
    }

    public void StartAnswerCheck() => StartCoroutine(Running());

    IEnumerator Running()
    {
        if (inputPanelId == null)
            yield break;

        inputPanelId.Show();
        inputPanelPw.Show();
        while (!inputPanelId.hasNewInput)
            yield return null;

        string userInput = inputPanelId.lastInput.Trim().ToLower();
        if (profId == userInput)
        {
            string pwInput = inputPanelId.lastInput.Trim().ToLower();
            if (profPw == pwInput)
            {
                //prof login
            }
            else if (studNames.Contains(userInput))
            {
                //students login
                Debug.Log($"Corretto! Risposte rimaste: si");
            }
            else
            {
                Debug.Log($"tentato accesso con nome utrnte: {userInput}");
            }
        }

        inputPanelId.AcceptAndHide();
        ToggleButtons(true);
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
}