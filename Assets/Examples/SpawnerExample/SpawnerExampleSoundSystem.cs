using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>Just used to show how events work.</summary>
 */
public class SpawnerExampleSoundSystem : MonoBehaviour {
	void Start () {
        // Subscribe/Listen to shoot event
        SpawnerExample.OnShoot += OnShoot;
	}

    private void OnDestroy() {
        // Unsubscribe from shoot event
        SpawnerExample.OnShoot -= OnShoot;
    }

    private void OnShoot(SpawnerExample.ShootEventData data) {
        var audioSource = Camera.main.GetComponent<AudioSource>();
        if (audioSource) {
            audioSource.Play();
        }
    }
}
