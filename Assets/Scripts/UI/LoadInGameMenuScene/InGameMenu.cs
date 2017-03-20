using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Dieses Script gehört in die IngameMenuScene

public class InGameMenu : MonoBehaviour {

    [SerializeField]
    private RectTransform PauseMenu;
    [SerializeField]
    private Slider peanutCooldown;
    [SerializeField]
    private Text peanutAmount;
    [SerializeField]
    private Slider stealthDuration;
    [SerializeField]
    private Slider wantedLevel;

    private float[] wantedLimits = new[] { 0f, 0.38f, 0.72f, 1f, 1f };


    private bool m_paused;
    private PlayerActor m_playerActor;
    private WantedLevel m_wantedLevel;

    void Start() {
        m_playerActor = GameObject.FindObjectOfType<PlayerActor>();
        m_wantedLevel = GameObject.FindObjectOfType<WantedLevel>();
        SetPaused(false);

        if (GameObject.FindObjectsOfType<InGameMenu>().Length > 1) {
            Debug.LogError("More than 1 InGameMenu is not allowed.");
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (Input.GetButtonDown("Cancel")) {
            SetPaused(!m_paused);
        }

        if (m_wantedLevel == null) return; // hotfix

        wantedLevel.value = Mathf.Lerp(wantedLimits[m_wantedLevel.currentWantedStage],
                    wantedLimits[m_wantedLevel.currentWantedStage + 1],
            m_wantedLevel.currentTierPercent);

        peanutCooldown.value = m_playerActor.shootPeanuts.cooldown.progress;
        peanutAmount.text = "x" + m_playerActor.shootPeanuts.ammo;

        stealthDuration.value = m_playerActor.camouflageController.PercentTimeLeft;
    }

    public void clickedButtonBackToGame() {
        SetPaused(false);
    }

    public void clickedButtonRestartLevel() {
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void clickedButtonExit() {
        SetPaused(false);
    }

    private void SetPaused(bool paused) {
        m_paused = paused;
        Time.timeScale = paused ? 0.0f : 1.0f;
        PauseMenu.gameObject.SetActive(paused);
    }
}
