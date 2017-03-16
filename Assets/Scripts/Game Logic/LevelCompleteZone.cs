using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteZone : MonoBehaviour
{
    [SerializeField]
    private float m_time=1.0F;

    [SerializeField]
    private SceneAsset m_scene ;
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
        //todo: save data

        SceneManager.LoadScene(m_scene.name);
        
    }
}
