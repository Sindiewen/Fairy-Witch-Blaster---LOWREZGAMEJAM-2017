using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Creates object pool for projectiles
/// </summary>
public class PoolManager : MonoBehaviour
{
    // Static values
    static PoolManager _instance;

    // accessor
    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                // find instance of the pool manager script in the scene
                _instance = FindObjectOfType<PoolManager>();
            }
            return _instance;
        }
    }


    // Private variables

    // Creates a dictionary
    //  Used for object pooling
    Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();


    

    // Public class methods
    /// <summary>
    /// Creates object pool
    /// 
    /// Store each pool as a queue, take oldest object and add it back into the queue
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreatePool(GameObject prefab, int poolSize)
    {
        // Stores the prefab identity - unique int for every game object
        int poolKey = prefab.GetInstanceID();

        // Ensures pool key isnt in dictionary
        // If key doesn't exsist, add it into the dictionary
        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            // Loops through the pool size, instantiatting each object and storing them into the object pool
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiates prefab
                GameObject newObject = Instantiate(prefab) as GameObject;

                // Ensures the object is no active when spawned
                newObject.SetActive(false);

                // Add it to the pool
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
    }

    /// <summary>
    /// Method for reusing objects in the pool
    /// 
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void ReuseObject(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        // Gets the prefab's instance id
        int poolKey = prefab.GetInstanceID();

        // Ensures prefab is in the dictionary already
        if (poolDictionary.ContainsKey(poolKey))
        {
            // Gets next object in pool
            GameObject objectToReuse = poolDictionary[poolKey].Dequeue();

            // Adds object back to end of queue
            poolDictionary[poolKey].Enqueue(objectToReuse);

            // Sets the values for the object to reuse
            objectToReuse.SetActive(true);
            objectToReuse.transform.position = position;
            objectToReuse.transform.rotation = rotation;
        }
    }
}

