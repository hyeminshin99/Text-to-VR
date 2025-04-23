#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ObjectSaver : MonoBehaviour
{
    public string promptName = "GeneratedModel";

    public void SaveSelectedObject()
    {
        GameObject target = ObjectSelector.selectedObject;

        if (target == null)
        {
            Debug.LogWarning("저장할 오브젝트가 선택되지 않았습니다.");
            return;
        }

        string safeName = MakeSafeFilename(promptName);
        string folder = "Assets/SavedPrefabs/" + safeName + "/";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string prefabPath = folder + safeName + ".prefab";
        int index = 1;
        while (File.Exists(prefabPath))
        {
            prefabPath = folder + safeName + "_" + index + ".prefab";
            index++;
        }

        // 모든 MeshFilter의 mesh 저장
        foreach (MeshFilter mf in target.GetComponentsInChildren<MeshFilter>())
        {
            if (mf.sharedMesh != null)
            {
                string meshPath = folder + mf.name + "_Mesh.asset";
                Mesh meshCopy = Object.Instantiate(mf.sharedMesh);
                AssetDatabase.CreateAsset(meshCopy, meshPath);
                mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            }
        }

        // 모든 MeshRenderer의 Material 저장
        foreach (MeshRenderer mr in target.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] newMaterials = new Material[mr.sharedMaterials.Length];

            for (int i = 0; i < mr.sharedMaterials.Length; i++)
            {
                Material mat = mr.sharedMaterials[i];

                if (mat == null) continue;

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mat)))
                {
                    // 이미 저장된 에셋이면 그대로 사용
                    newMaterials[i] = mat;
                }
                else
                {
                    // 복제 + 속성 수동 복사
                    Shader shader = Shader.Find("GLTFUtility/URP/Standard (Metallic)");
                    if (shader == null)
                    {
                        shader = Shader.Find("Universal Render Pipeline/Lit");
                        Debug.LogWarning("GLTFUtility 셰이더를 찾지 못했습니다. URP/Lit로 대체합니다.");
                    }

                    Material matCopy = new Material(shader);
                    matCopy.CopyPropertiesFromMaterial(mat); // 기본 속성

                    // 중요한 surface inputs 복사 수동 추가
                    TryCopySafeTexture(mat, matCopy, "_BaseMap");
                    TryCopySafeTexture(mat, matCopy, "_MetallicGlossMap");
                    TryCopySafeTexture(mat, matCopy, "_BumpMap");
                    TryCopySafeColor(mat, matCopy, "_BaseColor");
                    TryCopySafeFloat(mat, matCopy, "_Smoothness");
                    TryCopySafeFloat(mat, matCopy, "_Metallic");

                    string matPath = $"{folder}{mr.name}_Mat_{i}.mat";
                    AssetDatabase.CreateAsset(matCopy, matPath);
                    newMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
            }

            mr.sharedMaterials = newMaterials;
        }

        // Prefab 저장
        PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
        Debug.Log("완전 프리팹 저장 완료: " + prefabPath);
    }

    private static void TryCopySafeTexture(Material from, Material to, string key)
    {
        if (from.HasProperty(key))
            to.SetTexture(key, from.GetTexture(key));
    }

    private static void TryCopySafeColor(Material from, Material to, string key)
    {
        if (from.HasProperty(key))
            to.SetColor(key, from.GetColor(key));
    }

    private static void TryCopySafeFloat(Material from, Material to, string key)
    {
        if (from.HasProperty(key))
            to.SetFloat(key, from.GetFloat(key));
    }

    private string MakeSafeFilename(string input)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            input = input.Replace(c, '_');
        return input.Replace(" ", "");
    }
}
#endif
