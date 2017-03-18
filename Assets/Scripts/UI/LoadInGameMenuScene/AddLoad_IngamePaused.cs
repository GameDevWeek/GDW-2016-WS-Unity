using UnityEngine;
using UnityEngine.SceneManagement;

/*Dieses Script gehört in jede spielbare Levelscene!!!*/
public class AddLoad_IngamePaused : MonoBehaviour {

    public string UISceneName;


	void Start () {
        SceneManager.LoadScene(UISceneName, LoadSceneMode.Additive);
	}

}
