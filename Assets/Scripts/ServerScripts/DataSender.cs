using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class DataSender : MonoBehaviour
{
    // Local server address (Node.js default is usually 3000)
    private string serverUrl = "http://localhost:3000/data";

    public static DataSender Instance { get; private set; }

    private string savedAuthToken = "";

    [System.Serializable]
    public class LoginCredentials
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string status;
        public string token; // This will hold the long wristband string from the server
    }

    [System.Serializable]
    public class GameData
    {
        public string playerName;
        public string score;
    }

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void Login(string username, string password)
    {
        LoginCredentials credentials = new LoginCredentials { username = username, password = password };
        string json = JsonUtility.ToJson(credentials);
        StartCoroutine(LoginRequest(serverUrl + "/login", json));
    }

    private IEnumerator LoginRequest(string url, string json)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Read the server's response and extract the token string
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            // Save the token into our variable
            savedAuthToken = response.token;
            Debug.Log("Login Successful! Token saved.");
        }
        else
        {
            Debug.LogError("Login Failed: " + request.error);
        }
    }


    public void SendDataToServer(string name, string score)
    {
        GameData data = new GameData { playerName = name, score = score };
        string json = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest(serverUrl, json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (!string.IsNullOrEmpty(savedAuthToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + savedAuthToken);
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Network Error: " + request.error);
        }
        else
        {
            Debug.Log("Server Response: " + request.downloadHandler.text);
        }
    }
}
