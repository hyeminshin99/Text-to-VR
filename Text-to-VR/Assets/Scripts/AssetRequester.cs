using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetRequester : MonoBehaviour
{
    private string serverURL = "http://localhost:5005/get_asset";

    public void RequestAsset(string prompt)
    {
        StartCoroutine(SendPromptToServer(prompt));
    }

    IEnumerator SendPromptToServer(string prompt)
    {
        string json = JsonUtility.ToJson(new PromptData(prompt));
        UnityWebRequest request = UnityWebRequest.Put(serverURL, json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            Debug.Log("���� ����: " + result);
            // ���� �ܰ�: ��� URL�� �Ľ��ϰ� GLB �� �ε�
        }
        else
        {
            Debug.LogError("����: " + request.error);
        }
    }

    [System.Serializable]
    public class PromptData
    {
        public string prompt;
        public PromptData(string p) { prompt = p; }
    }
}