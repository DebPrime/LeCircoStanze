using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro

public class DataCollector : NetworkBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TMP_Text[] groupStatusTexts; // Drag your 6 Text objects here in the Inspector

    private Dictionary<ulong, int> clientGroupIndex = new Dictionary<ulong, int>();
    private const int MAX_CLIENTS = 6;

    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_CLIENTS)
        {
            response.Approved = false;
            response.Reason = "Server Full";
        }
        else
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        // Assign the first available index (0 to 5)
        for (int i = 0; i < MAX_CLIENTS; i++)
        {
            if (!clientGroupIndex.ContainsValue(i))
            {
                clientGroupIndex.Add(clientId, i);
                UpdateUIText(i, "Connected - Main Scene");
                break;
            }
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer && clientGroupIndex.TryGetValue(clientId, out int index))
        {
            UpdateUIText(index, "Disconnected");
            clientGroupIndex.Remove(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestChangeServerRpc(string sceneName, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        // 1. Update the UI on the Server/Host
        if (clientGroupIndex.TryGetValue(clientId, out int index))
        {
            UpdateUIText(index, $"In Scene: {sceneName}");
        }

        // 2. Tell the specific client to load the scene
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
        };
        LoadLocalSceneClientRpc(sceneName, clientRpcParams);
    }

    private void UpdateUIText(int groupIndex, string message)
    {
        // Only updates if the reference is set in the Inspector
        if (groupIndex >= 0 && groupIndex < groupStatusTexts.Length && groupStatusTexts[groupIndex] != null)
        {
            groupStatusTexts[groupIndex].text = $"Gruppo {groupIndex + 1}: {message}";
        }
    }

    [ClientRpc]
    private void LoadLocalSceneClientRpc(string sceneName, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(SwitchSceneRoutine(sceneName));
    }

    private IEnumerator SwitchSceneRoutine(string newSceneName)
    {
        Scene oldScene = SceneManager.GetActiveScene();
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        yield return loadOp;

        Scene newScene = SceneManager.GetSceneByName(newSceneName);
        if (newScene.IsValid()) SceneManager.SetActiveScene(newScene);

        if (oldScene.name != "MainScene")
            SceneManager.UnloadSceneAsync(oldScene);
        else
        {
            foreach (GameObject obj in oldScene.GetRootGameObjects())
            {
                if (obj.GetComponent<NetworkManager>() == null && obj != this.gameObject)
                    obj.SetActive(false);
            }
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        base.OnDestroy();
    }
}
