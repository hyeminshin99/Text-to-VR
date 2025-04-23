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
            Debug.LogWarning("������ ������Ʈ�� ���õ��� �ʾҽ��ϴ�.");
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

        // ��� MeshFilter�� mesh ����
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

        // ��� MeshRenderer�� Material ����
        foreach (MeshRenderer mr in target.GetComponentsInChildren<MeshRenderer>())
        {
            Material[] newMaterials = new Material[mr.sharedMaterials.Length];

            for (int i = 0; i < mr.sharedMaterials.Length; i++)
            {
                Material mat = mr.sharedMaterials[i];

                if (mat == null) continue;

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mat)))
                {
                    // �̹� ����� �����̸� �״�� ���
                    newMaterials[i] = mat;
                }
                else
                {
                    // ���� + �Ӽ� ���� ����
                    Shader shader = Shader.Find("GLTFUtility/URP/Standard (Metallic)");
                    if (shader == null)
                    {
                        shader = Shader.Find("Universal Render Pipeline/Lit");
                        Debug.LogWarning("GLTFUtility ���̴��� ã�� ���߽��ϴ�. URP/Lit�� ��ü�մϴ�.");
                    }

                    Material matCopy = new Material(shader);
                    matCopy.CopyPropertiesFromMaterial(mat); // �⺻ �Ӽ�

                    // �߿��� surface inputs ���� ���� �߰�
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

        // Prefab ����
        PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
        Debug.Log("���� ������ ���� �Ϸ�: " + prefabPath);
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
