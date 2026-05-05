using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanelManager : MonoBehaviour
{
    private static InputPanelManager instance;
    public static InputPanelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputPanelManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("InputPanelManager");
                    instance = go.AddComponent<InputPanelManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private InputPanel inputPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Try to find existing InputPanel in scene
        inputPanel = FindObjectOfType<InputPanel>();
        if (inputPanel == null)
        {
            Debug.LogWarning("No InputPanel found in scene! Please create an InputPanel GameObject with the InputPanel script attached.");
        }
    }

    public InputPanel GetInputPanel()
    {
        if (inputPanel == null)
        {
            inputPanel = FindObjectOfType<InputPanel>();
        }
        return inputPanel;
    }

    public bool HasInputPanel()
    {
        return GetInputPanel() != null;
    }
}