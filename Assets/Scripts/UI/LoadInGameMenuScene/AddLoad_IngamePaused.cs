using UnityEngine;
using UnityEngine.SceneManagement;

/*Dieses Script gehört in jede spielbare Levelscene!!!*/
public class AddLoad_IngamePaused : MonoBehaviour {

    public string UISceneName = "3";


	void Awake () {
        SceneManager.LoadScene(UISceneName, LoadSceneMode.Additive);
	}

}
