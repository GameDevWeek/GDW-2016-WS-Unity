using System;
using UnityEngine;
using UnityEngine.UI;

public class EndgameSceneController : MonoBehaviour {

	[SerializeField]
	private GameObject m_firstFrame;

	[SerializeField]
	private GameObject m_secondFrame;

	[SerializeField]
	private GameObject m_arm;

	[SerializeField]
	private float m_firstFrameTime;

	[SerializeField]
	private float m_secondFrameAdditionalTime;

	[SerializeField]
	private float m_armTime;

	[SerializeField]
	private float m_armInitialAngle = 45;

	[SerializeField]
	private float m_armFinalAngle = 0;

	private float m_timeAcc;


	private void Start() {
		m_firstFrame.SetActive (true);
		m_secondFrame.SetActive (false);
		m_arm.SetActive (false);
	}

	private void Update() {
		m_timeAcc += Time.unscaledDeltaTime;

		if (m_timeAcc > m_firstFrameTime) {
			m_firstFrame.SetActive (false);
			m_secondFrame.SetActive (true);
			m_arm.SetActive (true);

			var alpha = Mathf.Min ((m_timeAcc-m_firstFrameTime) / m_armTime, 1f);
			var armAngle = Mathf.Lerp (m_armInitialAngle, m_armFinalAngle, Mathf.Sin(alpha * Mathf.PI/2f)*0.5f+0.5f);
			m_arm.GetComponent<RectTransform> ().localRotation = Quaternion.Euler (new Vector3 (0,0,armAngle));

			if (m_timeAcc > m_firstFrameTime+m_armTime+m_secondFrameAdditionalTime) {
				// TODO: change scene
			}
		}

	}

}
