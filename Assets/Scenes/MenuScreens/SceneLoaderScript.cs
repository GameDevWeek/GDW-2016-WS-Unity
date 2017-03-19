using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderScript : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadSceneAdditive(string sc)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sc, UnityEngine.SceneManagement.LoadSceneMode.Additive);
	}

	public void LoadSceneSingle(string sc)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sc, UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	public void Quit()
	{
		#if UNITY_EDITOR
        	 // Application.Quit() does not work in the editor so
        	 // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
         	UnityEditor.EditorApplication.isPlaying = false;
    	 #else
         	Application.Quit();
     	#endif
	}
}
