using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoom : MonoBehaviour {

	[SerializeField]
	private float m_width = 1;

	[SerializeField]
	private float m_height = 1;

	[SerializeField]
	private float m_border = 1;

	public float Width {
		get {return m_width;}
	}
	public float Height {
		get {return m_height;}
	}

	public float Top {
		get {return m_height / 2f + transform.position.z;}
	}
	public float Bottom {
		get {return -m_height / 2f + transform.position.z;}
	}
	public float Left {
		get {return -m_width / 2f + transform.position.x;}
	}
	public float Right {
		get {return m_width / 2f + transform.position.x;}
	}

	public void OnDrawGizmosSelected() {
		Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		Gizmos.DrawWireCube(transform.position, new Vector3(m_width, 4, m_height));
	}

	public bool IsInside(Vector3 p) {
		return p.x >= Left - m_border && p.x <= Right + m_border && p.y >= Bottom - m_border && p.y <= Top - m_border;
	}

}
