using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerData
{
    public string username;
    public int circoStanza;
}

public class DataCollector : MonoBehaviour
{
    // 1. Esempio GET: Recupera dati dal server
    public IEnumerator GetData(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Converte il testo JSON in un oggetto C#
                PlayerData data = JsonUtility.FromJson<PlayerData>(webRequest.downloadHandler.text);
                Debug.Log("Giocatore: " + data.username + " - Score: " + data.circoStanza);
            }
            else
            {
                Debug.LogError("Errore: " + webRequest.error);
            }
        }
    }

    // 2. Esempio POST: Invia dati al server
    public IEnumerator PostData(string url, PlayerData data)
    {
        string json = JsonUtility.ToJson(data); // Converte oggetto in stringa JSON
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dati inviati con successo!");
            }
        }
    }
}