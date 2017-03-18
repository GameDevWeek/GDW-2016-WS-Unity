using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAchievements : MonoBehaviour {
    List<Archievement> achievements;

    private void Start()
    {
        achievements = ArchievementTracker.Instance.getAchievment();
    }


}
