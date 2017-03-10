using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameObjectPool : Pool<GameObject> {
    private class PoolWorker : MonoBehaviour {
        private GameObjectPool m_gameObjectPool;

        public void Init(GameObjectPool pool) {
            m_gameObjectPool = pool;
            name = "PoolWorker";
        }

        public void InitFree(GameObject go, float afterSeconds = 0.0f, bool waitForParticleDeath = true) {
            StartCoroutine(AwaitFree(go, afterSeconds, waitForParticleDeath));
        }

        private IEnumerator AwaitFree(GameObject go, float afterSeconds, bool waitForParticleDeath) {
            if (afterSeconds > 0.0f)
                yield return new WaitForSeconds(afterSeconds);

            if (waitForParticleDeath) {
                var systems = go.GetComponentsInChildren<ParticleSystem>();

                if (systems != null) {
                    yield return new WaitForEndOfFrame();

                    float maxParticleLifetime = 0.0f;
                    foreach (var system in systems)
                        maxParticleLifetime = Mathf.Max(system.main.startLifetime.constant, maxParticleLifetime);

                    // Turn off emission
                    foreach (var system in systems) {
                        var emission = system.emission;
                        emission.enabled = false;
                    }

                    // Wait for any remaining particles to expire
                    yield return new WaitForSeconds(maxParticleLifetime);

                    // Turn on emission again because this gameobject will be reused
                    foreach (var system in systems) {
                        var emission = system.emission;
                        emission.enabled = true;
                    }
                }
            }

            m_gameObjectPool.FinalizeFree(go);
        }
    }

    private GameObject m_worker = new GameObject();

    public GameObjectPool(GameObject sample, int initialSize = 100)
        : base(sample, initialSize) {
        m_worker.AddComponent<PoolWorker>();
        m_worker.GetComponent<PoolWorker>().Init(this);
        m_worker.transform.parent = m_parent.transform;
    }

    new public void Free(GameObject obj) {
        Free(obj, 0.0f);
    }

    public void Free(GameObject obj, float afterSeconds, bool waitForParticleDeath = true) {
        Debug.Assert(m_indices.ContainsKey(obj), "Trying to free an object that is not in the pool.");
        Debug.Assert(m_indices[obj] < m_numActive, "Trying to free a freed object.");

        m_worker.GetComponent<PoolWorker>().InitFree(obj, afterSeconds, waitForParticleDeath);
    }

    protected void FinalizeFree(GameObject obj) {
        Swap(m_indices[obj], m_numActive - 1);
        --m_numActive;
        obj.SetActive(false);
    }
}

public class Pool<T> where T : class {
    public delegate T CreateFunc();
    public delegate void DestroyFunc(T obj);

    public delegate void ActivateFunc(T obj);
    public delegate void DeactivateFunc(T obj);

    protected CreateFunc m_create;
    protected DestroyFunc m_destroy;

    protected ActivateFunc m_activate;
    protected DeactivateFunc m_deactivate;

    protected List<T> m_pool = new List<T>();

    // Save indices of the pooled objects.
    // Used to get amortized cost of O(1) to free objects.
    protected Dictionary<T, int> m_indices = new Dictionary<T, int>();
    protected T m_sample;
    protected int m_numActive = 0;

    protected GameObject m_parent;
    protected int m_growAmount = 10;

    public int growAmount {
        set { m_growAmount = value; }
        get { return m_growAmount; }
    }

    public GameObject gameObject {
        get { return m_parent; }
    }

    public int numActive {
        get { return m_numActive; }
    }

    public List<T> elements {
        get { return m_pool; }
    }

    public Pool(T sample, CreateFunc createFunc, DestroyFunc destroyFunc, ActivateFunc activateFunc, DeactivateFunc deactivateFunc, int initialSize = 100) {
        m_sample = sample;
        m_create = createFunc;
        m_destroy = destroyFunc;
        m_activate = activateFunc;
        m_deactivate = deactivateFunc;
        var go = sample as GameObject;
        if (go)
            m_parent = new GameObject(go.name + "-Pool");

        Grow(initialSize);
    }

    public Pool(T sample, CreateFunc createFunc, DestroyFunc destroyFunc, int initialSize = 100)
        : this(sample, createFunc, destroyFunc, (obj) => { }, (obj) => { }, initialSize) {
    }

    public Pool(T sample, CreateFunc createFunc, int initialSize = 100)
    : this(sample, createFunc, (obj) => { }, (obj) => { }, (obj) => { }, initialSize) {
    }

    public Pool(T sample, int initialSize = 100)
        : this(sample,
              () => GameObject.Instantiate(sample as GameObject) as T,
              (obj) => GameObject.Destroy(obj as GameObject),
              (obj) => (obj as GameObject).SetActive(true),
              (obj) => (obj as GameObject).SetActive(false),
              initialSize) {
        Debug.Assert(sample is GameObject, "This constructor requires the type to be a GameObject. The type "
            + sample.GetType() + " doesn't meet the requirement.");
    }

    public void Free(T obj) {
        Debug.Assert(m_indices.ContainsKey(obj), "Trying to free an object that is not in the pool.");

        int idx = m_indices[obj];
        Debug.Assert(idx < m_numActive, "Trying to free a freed object.");

        // Swap with the last active
        Swap(idx, m_numActive - 1);
        --m_numActive;

        m_deactivate(obj);
    }

    protected void Swap(int idx1, int idx2) {
        // Swap
        var idx1Obj = m_pool[idx1];
        m_pool[idx1] = m_pool[idx2];
        m_pool[idx2] = idx1Obj;

        // Fix pool indices
        m_indices[m_pool[idx1]] = idx1;
        m_indices[m_pool[idx2]] = idx2;
    }

    public T Create() {
        if (m_numActive == m_pool.Count) {
            if (m_growAmount == 0)
                return null;

            Grow(m_growAmount);
        }

        T obj = m_pool[m_numActive];
        m_activate(obj);
        ++m_numActive;

        return obj;
    }

    public void Grow(int num) {
        for (int i = 0; i < num; ++i) {
            T obj = m_create();
            m_pool.Add(obj);
            m_indices.Add(obj, m_pool.Count - 1);

            if (m_parent) {
                var go = obj as GameObject;
                go.transform.parent = m_parent.transform;
            }

            m_deactivate(obj);
        }
    }

    public void ShrinkBy(int num) {
        int shrinkNum = Math.Min(num, m_pool.Count);

        for (int i = 0; i < shrinkNum; ++i) {
            m_destroy(m_pool[m_pool.Count - 1]);
            m_pool.RemoveAt(m_pool.Count - 1);
        }
    }
}
