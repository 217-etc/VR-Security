using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material[] originalMaterials;

    public Material highlightMaterial; // ������ ��Ƽ����

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterials = meshRenderer.materials; // ���� ��Ƽ���� ����
        }
    }

    // Ư�� ��Ƽ����� ���� (�ܺο��� ȣ��)
    public void ApplyHighlight(int materialIndex)
    {
        if (meshRenderer != null && highlightMaterial != null && originalMaterials != null)
        {
            if (materialIndex >= 0 && materialIndex < originalMaterials.Length)
            {
                Material[] newMaterials = (Material[])originalMaterials.Clone();
                newMaterials[materialIndex] = highlightMaterial; // ������ �ε����� ��Ƽ���� ����
                meshRenderer.materials = newMaterials;
            }
            else
            {
                Debug.LogWarning($"[ChangeMaterial] �߸��� materialIndex: {materialIndex}, ������� ����.");
            }
        }
    }

    // ���� ��Ƽ����� ���� (�ܺο��� ȣ��)
    public void RemoveHighlight(int materialIndex)
    {
        if (meshRenderer != null && originalMaterials != null)
        {
            if (materialIndex >= 0 && materialIndex < originalMaterials.Length)
            {
                Material[] newMaterials = (Material[])meshRenderer.materials.Clone();
                newMaterials[materialIndex] = originalMaterials[materialIndex]; // ���� ��Ƽ���� ����
                meshRenderer.materials = newMaterials;
            }
            else
            {
                Debug.LogWarning($"[ChangeMaterial] �߸��� materialIndex: {materialIndex}, �������� ����.");
            }
        }
    }
}