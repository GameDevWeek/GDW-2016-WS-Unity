using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCamera : MonoBehaviour {

	public GameObject m_Parent;

	void LateUpdate () 
	{
		transform.position = m_Parent.transform.position;
		transform.rotation = m_Parent.transform.rotation;
		transform.localScale = m_Parent.transform.localScale;

		GetComponent<Camera> ().rect = m_Parent.GetComponent<Camera> ().rect;
		GetComponent<Camera> ().aspect = m_Parent.GetComponent<Camera> ().aspect;
		GetComponent<Camera> ().orthographicSize = m_Parent.GetComponent<Camera> ().orthographicSize;
	}
}
