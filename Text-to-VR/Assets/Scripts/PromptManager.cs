using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public TMP_InputField promptField;
    public ObjectSaver objectSaver;

    public void OnGenerateButtonClick()
    {
        string userPrompt = promptField.text;
        Debug.Log("입력된 프롬프트: " + userPrompt);

        // 프롬프트 저장 이름 전달
        objectSaver.promptName = userPrompt;

        // 서버 요청 (직접 FindObjectOfType로 실행)
        FindObjectOfType<AssetRequester>().RequestAsset(userPrompt);
    }
}