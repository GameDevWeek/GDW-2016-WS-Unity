using UnityEngine;

//Dieses Script gehört in die IngameMenuScene

public class InGameMenu : MonoBehaviour {

    [SerializeField] private RectTransform PauseMenu;

    private bool paused;

    // Use this for initialization
    void Start()
    {
        //Input.GetButtonDown();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
        }
        if (!paused) {
            PauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
        else {
            PauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void clickedButtonBackToGame()
    {
        paused = !paused;
    }

    public void clickedButtonRestartLevel()
    {
        paused = !paused;
    }

    public void clickedButtonExit()
    {
        Application.Quit();
    }
}
