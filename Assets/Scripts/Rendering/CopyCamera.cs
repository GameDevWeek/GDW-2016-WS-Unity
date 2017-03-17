using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCamera : MonoBehaviour {

	[SerializeField]
	private GameObject m_parent;

	[SerializeField]
	[Range(1, 8)]
	private int m_downSample = 1;

	private Camera m_parentCamera;
	private Camera m_camera;
	private RenderTexture m_texture;

	void Awake() {
		m_parentCamera = m_parent.GetComponent<Camera> ();
		m_camera = GetComponent<Camera> ();
	}

	void OnDestroy() {
		if (m_texture != null) {
			m_texture.Release();
		}
	}

	void LateUpdate () {
		if (m_texture==null
			|| m_parentCamera.pixelWidth/m_downSample != m_texture.width
			|| m_parentCamera.pixelHeight/m_downSample != m_texture.height) {

			m_texture = new RenderTexture (m_parentCamera.pixelWidth/m_downSample,
			                               m_parentCamera.pixelHeight/m_downSample,
			                               0, RenderTextureFormat.R8);
			m_texture.Create ();
			m_camera.targetTexture = m_texture;
		}

		transform.position = m_parent.transform.position;
		transform.rotation = m_parent.transform.rotation;
		transform.localScale = m_parent.transform.localScale;

		m_camera.rect = m_parentCamera.rect;
		m_camera.aspect = m_parentCamera.aspect;
		m_camera.orthographicSize = m_parentCamera.orthographicSize;

	}
}
