using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public TMP_InputField promptField;

    public void OnGenerateButtonClick()
    {
        string userPrompt = promptField.text;
        Debug.Log("�Էµ� ������Ʈ: " + userPrompt);

        // ���� �ܰ�: Flask ������ POST ��û
        FindObjectOfType<AssetRequester>().RequestAsset(userPrompt);
    }
}