using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Used if frequent creation/destruction of game objects is required. Example: particle effects </para> 
/// Note: You have to reset the state of the object when it is destroyed/created yourself. Often you don't need to do it at all.
/// </summary>
public class Spawner : MonoBehaviour {
    [System.Serializable]
    public class SpawnSample {
        [Tooltip("The prefab that is to be spawned.")]
        public GameObject prefab;
        public int preloadSize = 100;
        [Tooltip("Is the pool allowed to exceed the preload size?")]
        public bool allowPoolGrowth = true;
    }

    private static Dictionary<string, GameObjectPool> m_goPools = new Dictionary<string, GameObjectPool>();
    private static Dictionary<GameObject, GameObjectPool> m_goPoolsByObject = new Dictionary<GameObject, GameObjectPool>();

    [SerializeField]
    private List<SpawnSample> m_samples = new List<SpawnSample>();
    [SerializeField]
    private static int m_defaultPreloadSize = 100;

    public List<SpawnSample> samples {
        get { return m_samples; }
    }

    // For editor purposes: all pools are children of "Pools"
    private static GameObject m_poolsGameObject;

    private void Awake() {
        m_poolsGameObject = new GameObject("Pools");

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
    }

    private static GameObject SpawnInternal(string name) {
        GameObjectPool pool = null;
        if (m_goPools.TryGetValue(name, out pool)) {
            var go = pool.Create();
            m_goPoolsByObject.Add(go, pool);
            return go;
        }

        return null;
    }

    /// <summary>
    /// <para>Make sure to call DeSpawn if the object isn't needed anymore. </para> 
    /// </summary>
    public static GameObject Spawn(GameObject prefab) {
        GameObject go = SpawnInternal(prefab.name);
        if (go == null) {
            // Create a pool for the prefab
            Debug.Log("Consider preloading the pool of objects for " + prefab.name + ".");
            var pool = new GameObjectPool(prefab, m_defaultPreloadSize);

            if (m_poolsGameObject) {
                pool.gameObject.transform.parent = m_poolsGameObject.transform;
            }

            m_goPools.Add(prefab.name, pool);

            go = pool.Create();
            m_goPoolsByObject.Add(go, pool);
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
        if (m_goPools.TryGetValue(name, out pool)) {
            var go = pool.Create();
            go.transform.position = position;
            go.transform.rotation = rotation;
            m_goPoolsByObject.Add(go, pool);
            return go;
        }

        Debug.Assert(pool != null, "There is no pool for " + name + ". Make sure to create one first.");
        return null;
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, float lifetimeInSeconds, bool waitForParticleDeath = true) {
        return Spawn(name, lifetimeInSeconds, Vector3.zero, Quaternion.identity, waitForParticleDeath);
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, float lifetimeInSeconds, Vector3 position, bool waitForParticleDeath = true) {
        return Spawn(name, lifetimeInSeconds, position, Quaternion.identity, waitForParticleDeath);
    }

    /// <summary>
    /// <para>The object will be despawned automatically after the specified lifetime and after particle death (if true). </para> 
    /// </summary>
    public static GameObject Spawn(string name, float lifetimeInSeconds, Vector3 position, Quaternion rotation, bool waitForParticleDeath = true) {
        GameObjectPool pool = null;
        if (m_goPools.TryGetValue(name, out pool)) {
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
        if (m_goPoolsByObject.TryGetValue(go, out pool)) {
            pool.Free(go, afterSeconds, waitForParticleDeath);
            m_goPoolsByObject.Remove(go);
        }

        Debug.Assert(pool != null, "Could not despawn game object " + go.name);
    }
}
