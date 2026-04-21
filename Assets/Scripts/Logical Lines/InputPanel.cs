using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text riprovaText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;
    public TMP_InputField InputField => inputField;
    public string InputText
    {
        get => inputField.text;
        set => inputField.text = value;
    }
    public string lastInput{get;private set;}=string.Empty;
    public bool isWaitingOnUserInput{get;private set;}
    public bool correctAnswer=false;
    public bool hasNewInput {get; private set;} = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (canvasGroup == null || titleText == null || acceptButton == null || inputField == null)
        {
            Debug.LogError("InputPanel is missing required UI components! Please assign CanvasGroup, TMP_Text, Button, and TMP_InputField in the Inspector.");
            return;
        }
        
        canvasGroup.alpha=0;
        SetCanvasState(active: false);
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show()
    {
        inputField.text=string.Empty;
        canvasGroup.alpha=100;
        SetCanvasState(active: true);
        isWaitingOnUserInput=true;

    }

    public void ShowRetryText()
    {
        riprovaText.text="*Riprova";
    }

        public void HideRetryText()
    {
        riprovaText.text="";
    }
    public void OnAcceptInput()
    {
        if(inputField.text==string.Empty)
            return;
        lastInput=inputField.text;
        hasNewInput = true;
        // Signal that user has provided input, but don't hide the panel yet
    }

    public void AcceptAndHide()
    {
        Hide();
    }

    public void ResetForNextAttempt()
    {
        lastInput = string.Empty;
        inputField.text = string.Empty;
        acceptButton.gameObject.SetActive(false);
        hasNewInput = false;
        // isWaitingOnUserInput rimane true per permettere il prossimo tentativo
    }

    public void Hide()
    {
        canvasGroup.alpha=0;
        SetCanvasState(active: false);
        isWaitingOnUserInput=false;
    }

    private void SetCanvasState(bool active)
    {
        canvasGroup.interactable=active;
        canvasGroup.blocksRaycasts=active;
    }

    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }
    
    private bool HasValidText()
    {
        return inputField.text !=string.Empty;
    }
}
