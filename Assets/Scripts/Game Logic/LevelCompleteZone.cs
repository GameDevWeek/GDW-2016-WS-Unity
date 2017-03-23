using UnityEngine;

[RequireComponent(typeof(LevelCompletion))]
public class LevelCompleteZone : MonoBehaviour {
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(GameTag.Player)) {
            GetComponent<LevelCompletion>().CompleteLevel();
        }
    }
}
