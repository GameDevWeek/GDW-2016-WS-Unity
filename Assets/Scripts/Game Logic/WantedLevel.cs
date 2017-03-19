using UnityEngine;

public class WantedLevel : MonoBehaviour {



    public int currentWantedStage{ get { return currentTier;  }}
    public float currentTierPercent {
        get { return timeInCurrentTier / timeInTier[currentTier]; }
    }


    [SerializeField] private float [] timeInTier = new []{ 2.5f, 2f, 1f };
    [SerializeField] private int currentTier = 0;

    [SerializeField] private float timeBeforeReduction = 0.3f;
    private float currentTimeBeforeReduction = 0f;

    [Tooltip("Number of Guards that, after alerted hard-trigger stage 1")]
    [SerializeField] private int alertedGuardsLimit = 3;

    private float timeInCurrentTier = 0f;
    private int alertedGuards = 0;



    private void Update() {

        if (currentTier == 0) {
            if (alertedGuards >= alertedGuardsLimit) {
                timeInCurrentTier = 0f;
                currentTier = 1;
            }else{
                timeInCurrentTier = Mathf.Max(timeInTier[0] * alertedGuards / alertedGuardsLimit, timeInCurrentTier);
            }
        }

        if (currentTimeBeforeReduction >= 0.3f) {
            timeInCurrentTier = Mathf.Max(timeInCurrentTier - Time.deltaTime, 0);
        }
        currentTimeBeforeReduction += Time.deltaTime;
    }

    public void RaiseWantedLevel() {
        if (!this.enabled)
            return;
        timeInCurrentTier += Time.deltaTime;
        currentTimeBeforeReduction = 0f;

        if (currentTierPercent >= 1) {
            if (currentTier == 2) {
                this.enabled = false;
                return;
            }
            currentTier++;
            timeInCurrentTier = 0f;
        }
    }

    public void GuardIsNowVigilant() {
        alertedGuards++;
    }

    public void TriggerLaserAlert() {
        if(currentTier >= 2) return;
        timeInCurrentTier = 0f;
        currentTier = 2;
    }

}
