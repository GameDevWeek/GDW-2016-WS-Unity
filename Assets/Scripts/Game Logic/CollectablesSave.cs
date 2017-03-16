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
    public int m_bronzeElephant;
    public int m_silverElephant;
    public int m_goldElephant;
    public int m_score;
    public int m_destroyedObjects;
    public int m_highestAlarm;

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
       
    }

    public void changePeanuts(int p)
    {
        m_peanuts += p;
       
        
    }

    public void changebronzeElephant(int e)
    {
        m_bronzeElephant += e;
       
    }

    public void changeSilverElephant(int e)
    {
        m_silverElephant += e;

    }

    public void changeGoldElephant(int e)
    {
        m_goldElephant += e;

    }

    public void changeScore(int s)
    {
        m_score += s;

    }

    public void changedestroyedObjects(int d)
    {
        m_destroyedObjects += d;

    }

    public void changeHighestAlarm(int a)
    {
        m_highestAlarm += a;

    }

}
