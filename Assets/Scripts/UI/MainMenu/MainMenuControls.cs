using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControls : MonoBehaviour {
    public GameObject menuText;
    public GameObject menuButtons;
    public GameObject achievementsButtons;
    public GameObject credits;
    public GameObject elephant2d;
    public GameObject elephant3d;
    private bool view2d = false;
    private bool view3d = true;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (view3d)
            {
                view3d = false;
                view2d = true;
                elephant3d.SetActive(false);
                elephant2d.SetActive(true);
            }

            else
            {
                view3d = true;
                view2d = false;
                elephant3d.SetActive(true);
                elephant2d.SetActive(false);
            }
        }
    }

    public void clickedAchievements()
    {
        menuButtons.SetActive(false);
        achievementsButtons.SetActive(true);
    }

    public void clickedBackAchievements()
    {
        achievementsButtons.SetActive(false);
        menuButtons.SetActive(true);
    }

    public void clickedBackCredits()
    {
        credits.SetActive(false);
        menuButtons.SetActive(true);
        menuText.SetActive(true);
    }

    public void clickedCredits()
    {
        menuButtons.SetActive(false);
        menuText.SetActive(false);
        credits.SetActive(true);
    }
}
