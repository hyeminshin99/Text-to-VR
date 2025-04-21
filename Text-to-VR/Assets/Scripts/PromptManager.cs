using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public TMP_InputField promptField;

    public void OnGenerateButtonClick()
    {
        string userPrompt = promptField.text;
        Debug.Log("입력된 프롬프트: " + userPrompt);

        // 다음 단계: Flask 서버에 POST 요청
        FindObjectOfType<AssetRequester>().RequestAsset(userPrompt);
    }
}