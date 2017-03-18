using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControls : MonoBehaviour {
    public GameObject menuButtons;
    public GameObject achievementsButtons;

    public void clickedAchievements()
    {
        menuButtons.SetActive(false);
        achievementsButtons.SetActive(true);
    }

    public void clickedBack()
    {
        achievementsButtons.SetActive(false);
        menuButtons.SetActive(true);
    }
}
