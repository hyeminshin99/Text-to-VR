using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;


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
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("���� ���� JSON: " + jsonResponse);

            AssetResponse result = JsonUtility.FromJson<AssetResponse>(jsonResponse);
            Debug.Log("���� ���� URL: " + result.url);

            // ���� �ܰ�: �� �ٿ�ε� �� �ε�
            StartCoroutine(LoadModelFromURL(result.url));
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

    [System.Serializable]
    public class AssetResponse
    {
        public string url;
    }

    IEnumerator LoadModelFromURL(string modelUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get(modelUrl);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] glbData = request.downloadHandler.data;

            // ������ �ӽ� ���
            string path = Application.persistentDataPath + "/model.glb";
            System.IO.File.WriteAllBytes(path, glbData);

            GameObject model = Siccity.GLTFUtility.Importer.LoadFromFile(path);
            model.transform.position = Vector3.zero;
            model.transform.localScale = new Vector3(10f, 10f, 10f);

            model.AddComponent<BoxCollider>(); //���ð����ϵ��� collider�߰�
            model.name = "GeneratedModel"; //���� ���� �����ϵ��� �±� �Ǵ� �̸� ���� ����

        }
        else
        {
            Debug.LogError("�� �ٿ�ε� ����: " + request.error);
        }
    }
}