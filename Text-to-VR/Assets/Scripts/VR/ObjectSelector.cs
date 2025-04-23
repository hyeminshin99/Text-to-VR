using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public static GameObject selectedObject; // 저장할 대상

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
                // 이전 오브젝트 복원
                if (selectedObject != null && originalMaterials != null)
                {
                    Renderer prevRenderer = selectedObject.GetComponent<Renderer>();
                    if (prevRenderer != null)
                        prevRenderer.materials = originalMaterials;
                }

                // 새로운 선택
                selectedObject = hit.collider.gameObject;
                Debug.Log("!!선택됨: " + selectedObject.name);

                // 머티리얼 복제 + 추가 (덧씌우기)
                Renderer rend = selectedObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    originalMaterials = rend.materials;

                    // 기존 머티리얼 + 하이라이트 머티리얼 배열 만들기
                    Material[] newMaterials = new Material[originalMaterials.Length + 1];
                    originalMaterials.CopyTo(newMaterials, 0);
                    newMaterials[newMaterials.Length - 1] = highlightMaterial;

                    rend.materials = newMaterials;
                }
            }
        }
    }
}