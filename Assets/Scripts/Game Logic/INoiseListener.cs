
using UnityEngine;

public struct NoiseSourceData {
    public GameObject noiseCauser;
    public Vector3 initialPosition;

    public NoiseSourceData(GameObject noiseCauser, Vector3 initialPosition) {
        this.noiseCauser = noiseCauser;
        this.initialPosition = initialPosition;
    }
}

public interface INoiseListener {
    void Inform(NoiseSourceData data);
}
