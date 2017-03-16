
using UnityEngine;

public struct NoiseSourceData {
    public GameObject noiseCauser;
    public Vector3 initialPosition;
    public int priority;

    public NoiseSourceData(GameObject noiseCauser, Vector3 initialPosition, int priority) {
        this.noiseCauser = noiseCauser;
        this.initialPosition = initialPosition;
        this.priority = priority;
    }
}

public interface INoiseListener {
    void Inform(NoiseSourceData data);
}
