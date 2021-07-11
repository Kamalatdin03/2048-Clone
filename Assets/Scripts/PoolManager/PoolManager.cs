using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private List<Pool> _manulPools;

    private Dictionary<string, List<GameObject>> _pools;

    private void Awake()
    {
        _pools = new Dictionary<string, List<GameObject>>();

        Instance = this;

        Init();
    }

    private void Init()
    {
        foreach (var pool in _manulPools)
        {
            var objectPool = new List<GameObject>();

            for (int i = 0; i < pool.amount; i++)
            {
                var obj = Instantiate(pool.prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            _pools.Add(pool.id, objectPool);
        }
    }

    public GameObject SpawnPool(string id, Vector3 pos, Quaternion rotation)
    {
        if (!_pools.ContainsKey(id))
        {
            Debug.Log($"This id {id} not contain from pool {_pools.ContainsKey(id)}");
            return null;
        }

        var list = _pools[id];

        GameObject objectToSpawn = list[list.Count - 1];

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rotation;

        if (objectToSpawn.TryGetComponent<IPoolableObject>(out var poolable))
        {
            poolable.OnSpawn();
        }

        list.RemoveAt(list.Count - 1);

        return objectToSpawn;
    }

    public GameObject SpawnPool(string id, Transform parent)
    {
        GameObject obj = SpawnPool(id, Vector3.zero, Quaternion.identity);

        if (obj == null)
        {
            new System.NullReferenceException("Wtf");
        }

        obj.transform.SetParent(parent);
        return obj;
    }

    public T SpawnPool<T>(string id, Transform parent)
    {
        GameObject obj = SpawnPool(id, parent);
        return obj.GetComponent<T>();
    }

    public void DeSpawnPool(string id, GameObject obj)
    {
        if (_pools.TryGetValue(id, out var list))
        {
            list.Add(obj);
        }

        if (obj.TryGetComponent<IPoolableObject>(out var poolable))
        {
            poolable.OnDeSpawn();
        }

        obj.transform.SetParent(transform);
        obj.SetActive(false);
    }

    [Serializable]
    private sealed class Pool
    {
        public string id;
        public GameObject prefab;
        public int amount;
    }
}
