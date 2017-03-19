using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TopDownCamera), typeof(DeathCamera), typeof(SpectatorCamera))]
public class CameraController : MonoBehaviour  {
	
	private TopDownCamera m_topDown;
	private DeathCamera m_death;
	private SpectatorCamera m_spectator;

	private void Start() {
		m_topDown = GetComponent<TopDownCamera> ();
		m_death = GetComponent<DeathCamera> ();
		m_spectator = GetComponent<SpectatorCamera> ();
	}

	public void EnableNormalCam() {
		m_topDown.enabled = true;
		m_death.enabled = false;
		m_spectator.enabled = false;
	}
	public void EnableDeathCam() {
		m_topDown.enabled = false;
		m_death.enabled = true;
		m_spectator.enabled = false;
	}

	public void EnableSpectatorCam(Transform target, bool rotate=true) {
		m_topDown.enabled = false;
		m_death.enabled = false;
		m_spectator.SetTarget (target);
		m_spectator.SetRotate (rotate);
		m_spectator.enabled = true;
	}

}
