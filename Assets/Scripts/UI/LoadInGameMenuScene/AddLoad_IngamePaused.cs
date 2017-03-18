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

    public string UISceneName;


	void Start () {
	    SceneManager.LoadScene(UISceneName, LoadSceneMode.Additive);
	}

}
