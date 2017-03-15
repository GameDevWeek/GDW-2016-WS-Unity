using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteZone : MonoBehaviour
{
    [SerializeField]
    private float m_time=1.0F;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            
            Invoke("loadNextLevel", m_time);
            
        }
    }

    private void loadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
