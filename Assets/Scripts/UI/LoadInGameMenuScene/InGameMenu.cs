using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Dieses Script gehört in die IngameMenuScene

public class InGameMenu : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //Input.GetButtonDown();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void clickedButtonBackToGame()
    {
        SceneManager.UnloadSceneAsync(2);       //Die Scene IngameMenu schließen
        AddLoad_IngamePaused.setPause(false);   //In der Klasse AddLoad_IngamePaused pause-Variable auf false setzen, da sich das Spiel nun nicht mehr im Pausemodus befindet
        Time.timeScale = 1;                     //pausierte Scene fortsetzen
    }

    public void clickedButtonRestartLevel()
    {
        SceneManager.UnloadSceneAsync(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        AddLoad_IngamePaused.setPause(false);
        Time.timeScale = 1;
    }

    public void clickedButtonExit()
    {
        Application.Quit();
    }
}
