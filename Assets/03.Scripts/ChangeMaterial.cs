using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material[] originalMaterials;

    public Material highlightMaterial; // 강조할 머티리얼

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterials = meshRenderer.materials; // 원래 머티리얼 저장
        }
    }

    // 특정 머티리얼로 변경 (외부에서 호출)
    public void ApplyHighlight(int materialIndex)
    {
        if (meshRenderer != null && highlightMaterial != null && originalMaterials != null)
        {
            if (materialIndex >= 0 && materialIndex < originalMaterials.Length)
            {
                Material[] newMaterials = (Material[])originalMaterials.Clone();
                newMaterials[materialIndex] = highlightMaterial; // 지정된 인덱스의 머티리얼 변경
                meshRenderer.materials = newMaterials;
            }
            else
            {
                Debug.LogWarning($"[ChangeMaterial] 잘못된 materialIndex: {materialIndex}, 적용되지 않음.");
            }
        }
    }

    // 원래 머티리얼로 복구 (외부에서 호출)
    public void RemoveHighlight(int materialIndex)
    {
        if (meshRenderer != null && originalMaterials != null)
        {
            if (materialIndex >= 0 && materialIndex < originalMaterials.Length)
            {
                Material[] newMaterials = (Material[])meshRenderer.materials.Clone();
                newMaterials[materialIndex] = originalMaterials[materialIndex]; // 원래 머티리얼 복구
                meshRenderer.materials = newMaterials;
            }
            else
            {
                Debug.LogWarning($"[ChangeMaterial] 잘못된 materialIndex: {materialIndex}, 복구되지 않음.");
            }
        }
    }
}