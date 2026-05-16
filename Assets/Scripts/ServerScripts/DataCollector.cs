using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataCollector : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton; // Kept login button only

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            Debug.Log($"Attempting login for: {username}");
            DataSender.Instance.Login(username, password);
        }
        else
        {
            Debug.LogWarning("Fields cannot be blank!");
        }
    }
}