using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneElephant : MonoBehaviour {
    [SerializeField]
    private Material m_elephantMaterial;
    [SerializeField]
    private Material m_elephantStoneMaterial;
    [SerializeField]
    private Renderer m_elephantRenderer;
    [SerializeField]
    private Renderer m_trunkRenderer;

    private Coroutine m_toStoneRoutine;
    [SerializeField]
    private float m_toStoneDelay = 1.0f;

    void Start () {
        CamouflageController.OnElephantEntersCamouflageMode += OnElephantEntersCamouflageMode;
        CamouflageController.OnElephantExitsCamouflageMode += OnElephantExitsCamouflageMode;
	}

    private void OnDestroy() {
        CamouflageController.OnElephantEntersCamouflageMode -= OnElephantEntersCamouflageMode;
        CamouflageController.OnElephantExitsCamouflageMode -= OnElephantExitsCamouflageMode;
    }

    private void OnElephantExitsCamouflageMode() {
        if (m_toStoneRoutine != null) {
            StopCoroutine(m_toStoneRoutine);
            m_toStoneRoutine = null;
        }

        SetMaterial(m_elephantRenderer, m_elephantMaterial);
        SetMaterial(m_trunkRenderer, m_elephantMaterial);
    }

    private void OnElephantEntersCamouflageMode() {
        if (m_toStoneRoutine != null) {
            return;
        }

        m_toStoneRoutine = StartCoroutine(ToStoneRoutine());
    }

    IEnumerator ToStoneRoutine() {
        yield return new WaitForSeconds(m_toStoneDelay);

        SetMaterial(m_elephantRenderer, m_elephantStoneMaterial);
        SetMaterial(m_trunkRenderer, m_elephantStoneMaterial);

        m_toStoneRoutine = null;
    }

    private void SetMaterial(Renderer renderer, Material material) {
        if (renderer && material) {
            renderer.material = material;
        }
    }
}
