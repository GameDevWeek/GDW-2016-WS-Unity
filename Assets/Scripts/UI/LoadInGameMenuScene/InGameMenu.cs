using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Dieses Script gehört in die IngameMenuScene

public class InGameMenu : MonoBehaviour {

    [SerializeField] private RectTransform PauseMenu;
    [SerializeField] private Slider peanutCooldown;
    [SerializeField] private Text peanutAmount;
    [SerializeField] private Slider stealthDuration;
    [SerializeField] private Slider wantedLevel;

    private float[] wantedLimits = new[] {0f, 0.38f, 0.72f, 1f, 1f};


    private bool paused;
    private PlayerActor m_playerActor;
    private WantedLevel m_wantedLevel;

    void Start() {
        m_playerActor = GameObject.FindObjectOfType<PlayerActor>();
        m_wantedLevel = GameObject.FindObjectOfType<WantedLevel>();
    
    }
    // Update is called once per frame
    void LateUpdate()
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

	    if(m_wantedLevel == null) return; // hotfix

        wantedLevel.value =  Mathf.Lerp(wantedLimits[m_wantedLevel.currentWantedStage],
                    wantedLimits[m_wantedLevel.currentWantedStage + 1],
            m_wantedLevel.currentTierPercent);

        peanutCooldown.value = m_playerActor.shootPeanuts.cooldown.progress;
        peanutAmount.text = "x" + m_playerActor.shootPeanuts.ammo;

        stealthDuration.value = m_playerActor.camouflageController.PercentTimeLeft;
    }

    public void clickedButtonBackToGame()
    {
        paused = !paused;
    }

    public void clickedButtonRestartLevel()
	{
        paused = !paused;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void clickedButtonExit()
    {
        paused = false;
    }
}
