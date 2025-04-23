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
            Debug.Log("서버 응답 JSON: " + jsonResponse);

            AssetResponse result = JsonUtility.FromJson<AssetResponse>(jsonResponse);
            Debug.Log("서버 응답 URL: " + result.url);

            // 다음 단계: 모델 다운로드 및 로딩
            StartCoroutine(LoadModelFromURL(result.url));
        }
        else
        {
            Debug.LogError("에러: " + request.error);
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

            // 저장할 임시 경로
            string path = Application.persistentDataPath + "/model.glb";
            System.IO.File.WriteAllBytes(path, glbData);

            GameObject model = Siccity.GLTFUtility.Importer.LoadFromFile(path);
            model.transform.position = Vector3.zero;
            model.transform.localScale = new Vector3(10f, 10f, 10f);

            model.AddComponent<BoxCollider>(); //선택가능하도록 collider추가
            model.name = "GeneratedModel"; //선택 추적 가능하도록 태그 또는 이름 설정 가능

        }
        else
        {
            Debug.LogError("모델 다운로드 실패: " + request.error);
        }
    }
}