using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public static GameObject selectedObject; // ������ ���

    public Material highlightMaterial;
    private Material[] originalMaterials;

    void Update()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // ���� ������Ʈ ����
                if (selectedObject != null && originalMaterials != null)
                {
                    Renderer prevRenderer = selectedObject.GetComponent<Renderer>();
                    if (prevRenderer != null)
                        prevRenderer.materials = originalMaterials;
                }

                // ���ο� ����
                selectedObject = hit.collider.gameObject;
                Debug.Log("!!���õ�: " + selectedObject.name);

                // ��Ƽ���� ���� + �߰� (�������)
                Renderer rend = selectedObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    originalMaterials = rend.materials;

                    // ���� ��Ƽ���� + ���̶���Ʈ ��Ƽ���� �迭 �����
                    Material[] newMaterials = new Material[originalMaterials.Length + 1];
                    originalMaterials.CopyTo(newMaterials, 0);
                    newMaterials[newMaterials.Length - 1] = highlightMaterial;

                    rend.materials = newMaterials;
                }
            }
        }
    }
}