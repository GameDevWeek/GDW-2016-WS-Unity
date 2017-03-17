using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuAchievementButton : MonoBehaviour {
    public GameObject menuButtons;


    public void clickedAchievements()
    {
        menuButtons.SetActive(false);
    }
}
