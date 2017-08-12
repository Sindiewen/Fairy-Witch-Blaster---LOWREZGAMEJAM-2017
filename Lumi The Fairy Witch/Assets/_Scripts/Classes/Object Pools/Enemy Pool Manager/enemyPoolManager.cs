using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Creates object pool for projectiles
/// </summary>
public class enemyPoolManager : MonoBehaviour
{
    // Static values
    static enemyPoolManager _instance;

    // accessor
    public static enemyPoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                // find instance of the pool manager script in the scene
                _instance = FindObjectOfType<enemyPoolManager>();
            }
            return _instance;
        }
    }


    // Private variables

    // Creates a dictionary
    //  Used for object pooling
    Dictionary<int, Queue<Enemy_Projectile_Controller>> poolDictionary = new Dictionary<int, Queue<Enemy_Projectile_Controller>>();




    // Public class methods
    /// <summary>
    /// Creates object pool
    /// 
    /// Store each pool as a queue, take oldest object and add it back into the queue
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreatePool(Enemy_Projectile_Controller prefab, int poolSize)
    {
        // Stores the prefab identity - unique int for every game object
        int poolKey = prefab.GetInstanceID();

        // Ensures pool key isnt in dictionary
        // If key doesn't exsist, add it into the dictionary
        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Enemy_Projectile_Controller>());

            // Loops through the pool size, instantiatting each object and storing them into the object pool
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiates prefab
                Enemy_Projectile_Controller newObject = Instantiate(prefab) as Enemy_Projectile_Controller;

                // Ensures the object is no active when spawned
                newObject.gameObject.SetActive(false);

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
    public void ReuseObject(Enemy_Projectile_Controller prefab, Vector2 position, Quaternion rotation, bool facingRight, 
        float projectileDeaspawnTime, float projFireSpeed)
    {
        // Gets the prefab's instance id
        int poolKey = prefab.GetInstanceID();

        // Ensures prefab is in the dictionary already
        if (poolDictionary.ContainsKey(poolKey))
        {
            // Gets next object in pool
            Enemy_Projectile_Controller objectToReuse = poolDictionary[poolKey].Dequeue();

            // Adds object back to end of queue
            poolDictionary[poolKey].Enqueue(objectToReuse);

            // Sets the values for the object to reuse
            objectToReuse.gameObject.SetActive(true);
            objectToReuse.transform.position = position;
            objectToReuse.transform.rotation = rotation;

            // IF the player is facing not right, set the projectile fire speed to negative the whole number
            // so the projectile can fire left
            if (!facingRight)
            {
                projFireSpeed *= -1;
            }

            // Adds force to the projectile 
            objectToReuse.setProjectileFiringValues(projFireSpeed);

            // Disables projectile after a set time
            objectToReuse.Invoke("destroyProjectile", projectileDeaspawnTime);
        }
    }
}
