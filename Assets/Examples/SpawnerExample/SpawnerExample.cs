using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerExample : MonoBehaviour {
    public struct ShootEventData {
        public GameObject shooter;

        public ShootEventData(GameObject shooter) {
            this.shooter = shooter;
        }
    }

    public delegate void ShootEvent(ShootEventData data);
    public static event ShootEvent OnShoot;

    [SerializeField]
    private Cooldown m_cooldown = new Cooldown(0.5f);
    [SerializeField]
    private GameObject m_bulletPrefab;

    void Update() {
        m_cooldown.Update(Time.deltaTime);

        if (Input.GetMouseButton(0) && m_cooldown.IsOver()) {
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var rayHit = Physics.Raycast(ray, out hitInfo);

            if (rayHit) {
                var bullet = global::Spawner.Spawn(m_bulletPrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                Debug.Assert(bullet.GetComponent<SpawnerExampleBullet>(), "Bullet should have a bullet component.");
                bullet.GetComponent<SpawnerExampleBullet>().Shoot(ray.direction * 10.0f);
                if (OnShoot != null) {
                    OnShoot.Invoke(new ShootEventData(gameObject));
                }

                m_cooldown.Start();
            }
        }
    }
}

