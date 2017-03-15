using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CollectablesSave : Singleton<CollectablesSave>
{

    public int m_peanuts;
    public int m_miniJadeElephant;
  
    struct Collectables
    {
        int peanuts, miniJadeElephant;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            changePeanuts(-3);
            changeMiniJadeElephant(-1);
            Debug.Log("Peanuts: "+ m_peanuts + " MiniElephant: "+ m_miniJadeElephant);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Peanuts: " + m_peanuts + " MiniElephant: " + m_miniJadeElephant);
        }

    }

    public void changePeanuts(int p)
    {
        m_peanuts += p;
       
        
    }

    public void changeMiniJadeElephant(int j)
    {
        m_miniJadeElephant += j;
       
    }

    
}
