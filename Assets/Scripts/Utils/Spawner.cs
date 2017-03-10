using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Used if frequent creation/destruction of game objects is required. Example: particle effects </para> 
/// <para>Note: You have to reset the state of the object when it is destroyed/created yourself. Often you don't need to do it at all.</para>
/// <para>Note: Make sure the Spawner is called first in the script execution order.</para>
/// </summary>
public sealed class Spawner : MonoBehaviour {
    [System.Serializable]
    public class SpawnSample {
        [Tooltip("The prefab that is to be spawned.")]
        public GameObject prefab;
        public int preloadSize = 100;
        [Tooltip("Is the pool allowed to exceed the preload size?")]
        public bool allowPoolGrowth = true;
    }

    private Dictionary<string, GameObjectPool> m_goPools = new Dictionary<string, GameObjectPool>();
    private Dictionary<GameObject, GameObjectPool> m_goPoolsByObject = new Dictionary<GameObject, GameObjectPool>();

    [SerializeField]
    private List<SpawnSample> m_samples = new List<SpawnSample>();
    [SerializeField]
    private int m_defaultPreloadSize = 100;

    private static Spawner m_instance = null;

    public List<SpawnSample> samples {
        get { return m_samples; }
    }

    private static Dictionary<string, GameObjectPool> goPools {
        get {
            return m_instance.m_goPools;
        }
    }

    private static Dictionary<GameObject, GameObjectPool> goPoolsByObject {
        get {
            return m_instance.m_goPoolsByObject;
        }
    }

    // For editor purposes: all pools are children of "Pools"
    private GameObject m_poolsGameObject;

    public static Spawner instance {
        get {
            return m_instance;
        }
    }

    private void OnDestroy() {
        m_instance = null;
    }

    private void Awake() {
        m_poolsGameObject = new GameObject("Pools");
        m_poolsGameObject.transform.parent = transform;

        if (FindObjectsOfType<Spawner>().Length > 1) {
            Debug.LogError("Do not create more than one Spawner instance.");
        }

        m_goPools.Clear();
        m_goPoolsByObject.Clear();

        foreach (var sample in m_samples) {
            Debug.Assert(sample.prefab != null, "There are uninitialized pool samples.");
            if (sample.prefab == null) {
                continue;
            }

            var pool = new GameObjectPool(sample.prefab, sample.preloadSize);
            pool.gameObject.transform.parent = m_poolsGameObject.transform;

            if (!sample.allowPoolGrowth) {
                pool.growAmount = 0;
            }
            m_goPools.Add(sample.prefab.name, pool);
        }

        m_instance = this;
    }

    private static GameObject SpawnInternal(string name) {
        GameObjectPool pool = null;
        if (m_instance.m_goPools.TryGetValue(name, out pool)) {
            var go = pool.Create();
            if (go != null) {
                goPoolsByObject.Add(go, pool);
            }
            return go;
        }

        return null;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab) {
        Debug.Assert(prefab, "The requested prefab is null.");
        GameObject go = SpawnInternal(prefab.name);
        if (go == null) {
            // Create a pool for the prefab
            Debug.Log("Consider preloading the pool of objects for " + prefab.name + ".");
            var pool = new GameObjectPool(prefab, m_instance.m_defaultPreloadSize);

            if (m_instance.m_poolsGameObject) {
                pool.gameObject.transform.parent = m_instance.m_poolsGameObject.transform;
            }

            goPools.Add(prefab.name, pool);

            go = pool.Create();
            goPoolsByObject.Add(go, pool);
        }

        return go;
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        var go = Spawn(prefab);
        DeSpawn(go, lifetimeInSeconds, waitForParticleDeath);
        return go;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position) {
        var go = Spawn(prefab);
        go.transform.position = position;
        return go;
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        var go = Spawn(prefab);
        go.transform.position = position;
        DeSpawn(go, lifetimeInSeconds, waitForParticleDeath);
        return go;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
        var go = Spawn(prefab);
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        var go = Spawn(prefab);
        go.transform.position = position;
        go.transform.rotation = rotation;
        DeSpawn(go, lifetimeInSeconds, waitForParticleDeath);
        return go;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(string name) {
        GameObject go = SpawnInternal(name);
        Debug.Assert(go != null, "There is no pool for " + name + ". Make sure to create one first.");
        return go;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(string name, Vector3 position) {
        return Spawn(name, position, Quaternion.identity);
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(string name, Vector3 position, Quaternion rotation) {
        GameObjectPool pool = null;
        if (goPools.TryGetValue(name, out pool)) {
            var go = pool.Create();
            go.transform.position = position;
            go.transform.rotation = rotation;
            goPoolsByObject.Add(go, pool);
            return go;
        }

        Debug.Assert(pool != null, "There is no pool for " + name + ". Make sure to create one first.");
        return null;
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        return Spawn(name, Vector3.zero, Quaternion.identity, lifetimeInSeconds, waitForParticleDeath);
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, Vector3 position, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        return Spawn(name, position, Quaternion.identity, lifetimeInSeconds, waitForParticleDeath);
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, Vector3 position, Quaternion rotation, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        GameObjectPool pool = null;
        if (goPools.TryGetValue(name, out pool)) {
            var go = pool.Create();
            go.transform.position = position;
            go.transform.rotation = rotation;
            pool.Free(go, lifetimeInSeconds, waitForParticleDeath);
            return go;
        }

        Debug.Assert(pool != null, "There is no pool for " + name + ". Make sure to create one first.");
        return null;
    }

    public static void DeSpawn(GameObject go, float afterSeconds = 0.0f, bool waitForParticleDeath = true) {
        Debug.Assert(go != null, "GameObject should not be null.");

        GameObjectPool pool = null;
        if (goPoolsByObject.TryGetValue(go, out pool)) {
            pool.Free(go, afterSeconds, waitForParticleDeath);
            goPoolsByObject.Remove(go);
        }

        Debug.Assert(pool != null, "Could not despawn game object " + go.name);
    }
}
