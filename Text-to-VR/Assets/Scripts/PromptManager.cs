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
        Debug.Log("�Էµ� ������Ʈ: " + userPrompt);

        // ������Ʈ ���� �̸� ����
        objectSaver.promptName = userPrompt;

        // ���� ��û (���� FindObjectOfType�� ����)
        FindObjectOfType<AssetRequester>().RequestAsset(userPrompt);
    }
}