using UnityEngine;
using UnityEngine.UI;

//Dieses Script gehört in die IngameMenuScene

public class InGameMenu : MonoBehaviour {

    [SerializeField] private RectTransform PauseMenu;
    [SerializeField] private Slider peanutCooldown;
    [SerializeField] private Text peanutAmount;
    [SerializeField] private Slider stealthDuration;
    [SerializeField] private Slider wantedLevel;

    [SerializeField] private float wantedLevel2 = 0.38f;
    [SerializeField] private float wantedLevel3 = 0.72f;



    private bool paused;

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

        if (WantedLevel.Instance.CurrentWantedStage == 0) {
            wantedLevel.value = 0;
        } else if (WantedLevel.Instance.CurrentWantedStage == 1) {
            wantedLevel.value = Mathf.Lerp(0, wantedLevel2, WantedLevel.Instance.PercentCurrentWantedLevel);
        }
        else if (WantedLevel.Instance.CurrentWantedStage == 2) {
            wantedLevel.value = Mathf.Lerp(wantedLevel2, wantedLevel3, WantedLevel.Instance.PercentCurrentWantedLevel);
        }
        else if (WantedLevel.Instance.CurrentWantedStage == 3) {
            wantedLevel.value = 1;
        }


        peanutCooldown.value = PlayerActor.Instance.shootPeanuts.cooldown.progress;
        peanutAmount.text = "x" + PlayerActor.Instance.shootPeanuts.ammo;

        stealthDuration.value = PlayerActor.Instance.camouflageController.PercentTimeLeft;

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
