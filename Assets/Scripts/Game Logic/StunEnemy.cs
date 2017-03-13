using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEnemy : MonoBehaviour {

    public struct StunEventData
    {
        public GameObject stunner, stunned;

        public StunEventData(GameObject stunner, GameObject stunned)
        {
            this.stunner = stunner;
            this.stunned = stunned;
        }
    }

    public delegate void StunEvent(StunEventData data);
    public static StunEvent OnStun;

    [Range(0,10)]
    public float Distance = 1.0f;

    public float HighOffeset = 0.0f;

    public string TagToBeHitted = "Enemy";

    [SerializeField]
    private Cooldown m_cooldown = new Cooldown(0.5f);
    private Vector3 m_vectorOffset;
	// Use this for initialization
	void Start () {
        OnStun += ReactOnStun;
    }

    private void OnDestroy()
    {
        OnStun -= ReactOnStun;
    }

    private void OnValidate()
    {
        m_vectorOffset = new Vector3(0, HighOffeset, 0);
    }

    // Update is called once per frame
    void Update () {
        m_cooldown.Update(Time.deltaTime);
        if (Input.GetMouseButtonDown(1))
        {
            if (m_cooldown.IsOver() && StunEnemyInFront())
            {
                m_cooldown.Start();
            }
        }
	}

    private bool StunEnemyInFront()
    {
        RaycastHit info = new RaycastHit();
        Ray myRay = new Ray(transform.position + m_vectorOffset, transform.forward);
        Physics.SphereCast(myRay, 0.25f, out info, Distance);

        if (info.rigidbody != null && info.rigidbody.tag == TagToBeHitted)
        {
            if (Vector3.Dot(info.rigidbody.transform.forward, transform.forward) > 0) //Check if the Object in Front of this one, is directed with the back to this. 
            {
                if (OnStun != null)
                {
                    OnStun(new StunEventData(this.gameObject, info.rigidbody.gameObject));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void ReactOnStun(StunEventData data)
    {
        Debug.Log(data.stunner.name +" is stunning "+ data.stunned);
    }



    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + transform.forward * Distance + m_vectorOffset, "StunEnemy | CoolDown:" + ((int)(m_cooldown.timeLeftInSeconds*100))/100.0f);
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawLine(transform.position + m_vectorOffset, transform.position + transform.forward * Distance + m_vectorOffset);
    }


}
