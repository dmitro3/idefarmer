using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    // Object pool parameters
    [Header("Pooling Parameters")]
    public GameObject prefabProduct;          // Prefab của đối tượng lông cừu
    public Transform productParents;          // Parent chứa các đối tượng lông cừu
    public int poolSize = 10;                 // Số lượng đối tượng trong pool

    private Queue<GameObject> poolObjects;    // Hàng đợi chứa các đối tượng

    private void Awake()
    {
        poolObjects = new Queue<GameObject>();

        // Initialize pool with inactive objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabProduct, productParents);
            obj.SetActive(false);
            poolObjects.Enqueue(obj);
        }
    }

    // Function to get an object from the pool
    public GameObject GetPooledObject()
    {
        //Debug.Log("GetPooledObject ");
        if (poolObjects.Count > 0)
        {
          //  Debug.Log("GetPooledObject Count "+ poolObjects.Count);
            GameObject pooledObj = poolObjects.Dequeue();
            pooledObj.SetActive(true);
            return pooledObj;
        }
        else
        {
            // Optionally, instantiate more if needed (if pool size is not fixed)
            GameObject newObject = Instantiate(prefabProduct, productParents);
            return newObject;
        }
    }

    // Function to return an object to the pool
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(productParents.transform);
        poolObjects.Enqueue(obj);
    }
}
