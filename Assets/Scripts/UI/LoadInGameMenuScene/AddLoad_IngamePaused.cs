using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Das InGameMenu wird als additive Scene mit Escape geladen
 "bool pause" speichert den Pause-Status. Wird die Scene durch
 Escape geladen, pausiert die andere Scene und pause wird auf
 true gesetzt. Beim erneuten betätigen der Escape-Taste schließt
 die InGameMenu-Scene wieder und die andere Scene wird fort-
 gesetzt.
 
 Dieses Script gehört in jede spielbare Levelscene!!!*/
public class AddLoad_IngamePaused : MonoBehaviour {

    private static bool pause = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                pausedOn();
            }
            else
            {
                pausedOff();
            }
        }
	}

    public static void setPause(bool p)
    {
        pause = p;
    }

    public void pausedOn()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        pause = true;
    }

    public void pausedOff()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(2);
        pause = false;
    }
}
