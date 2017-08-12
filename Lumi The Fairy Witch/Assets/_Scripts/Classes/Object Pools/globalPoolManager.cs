/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalPoolManager : MonoBehaviour
{

    // Static values
    static globalPoolManager _instance;

    // accessor
    public static globalPoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                // find instance of the pool manager script in the scene
                _instance = FindObjectOfType<globalPoolManager>();
            }
            return _instance;
        }
    }


    // Private variables

    // Creates a dictionary
    //  Used for object pooling
    //Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    Dictionary<int, Queue<Lumi_Projectile_Controller>> fairyPoolDictionary = new Dictionary<int, Queue<Lumi_Projectile_Controller>>();
    Dictionary<int, Queue<Lumi_Projectile_Controller>> witchPoolDictionary = new Dictionary<int, Queue<Lumi_Projectile_Controller>>();
    //Dictionary<int, Queue<Enemy_Projectile_Controller>> enemyPoolDictionary = new Dictionary<int, Queue<Enemy_Projectile_Controller>>();




    /// <summary>
    /// Creates object pool
    /// 
    /// Store each pool as a queue, take oldest object and add it back into the queue
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreateFairyPool(Lumi_Projectile_Controller prefab, int poolSize)
    {
        // Stores the prefab identity - unique int for every game object
        int poolKey = prefab.GetInstanceID();

        // Ensures pool key isnt in dictionary
        // If key doesn't exsist, add it into the dictionary
        if (!fairyPoolDictionary.ContainsKey(poolKey))
        {
            fairyPoolDictionary.Add(poolKey, new Queue<Lumi_Projectile_Controller>());

            // Loops through the pool size, instantiatting each object and storing them into the object pool
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiates prefab
                Lumi_Projectile_Controller newObject = Instantiate(prefab) as Lumi_Projectile_Controller;

                // Ensures the object is no active when spawned
                newObject.gameObject.SetActive(false);

                // Add it to the pool
                fairyPoolDictionary[poolKey].Enqueue(newObject);
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
    public void ReuseFairyObject(Lumi_Projectile_Controller prefab, Vector2 position, Quaternion rotation, bool facingRight,
        float projFireSpeed, float projectileDeaspawnTime, ref int numOfProjectilesOnScreen)
    {
        // Gets the prefab's instance id
        int poolKey = prefab.GetInstanceID();

        // Ensures prefab is in the dictionary already
        if (fairyPoolDictionary.ContainsKey(poolKey))
        {
            // Gets next object in pool
            Lumi_Projectile_Controller objectToReuse = fairyPoolDictionary[poolKey].Dequeue();

            // Adds object back to end of queue
            fairyPoolDictionary[poolKey].Enqueue(objectToReuse);

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




    /// <summary>
    /// Creates object pool
    /// 
    /// Store each pool as a queue, take oldest object and add it back into the queue
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreateWitchPool(Lumi_Projectile_Controller prefab, int poolSize)
    {
        // Stores the prefab identity - unique int for every game object
        int poolKey = prefab.GetInstanceID();

        // Ensures pool key isnt in dictionary
        // If key doesn't exsist, add it into the dictionary
        if (!witchPoolDictionary.ContainsKey(poolKey))
        {
            witchPoolDictionary.Add(poolKey, new Queue<Lumi_Projectile_Controller>());

            // Loops through the pool size, instantiatting each object and storing them into the object pool
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiates prefab
                Lumi_Projectile_Controller newObject = Instantiate(prefab) as Lumi_Projectile_Controller;

                // Ensures the object is no active when spawned
                newObject.gameObject.SetActive(false);

                // Add it to the pool
                witchPoolDictionary[poolKey].Enqueue(newObject);
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
    public void ReuseWitchObject(Lumi_Projectile_Controller prefab, Vector2 position, Quaternion rotation, bool facingRight,
        float projFireSpeed, float projectileDeaspawnTime, ref int numOfProjectilesOnScreen)
    {
        // Gets the prefab's instance id
        int poolKey = prefab.GetInstanceID();

        // Ensures prefab is in the dictionary already
        if (witchPoolDictionary.ContainsKey(poolKey))
        {
            // Gets next object in pool
            Lumi_Projectile_Controller objectToReuse = witchPoolDictionary[poolKey].Dequeue();

            // Adds object back to end of queue
            witchPoolDictionary[poolKey].Enqueue(objectToReuse);

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

    
    /// <summary>
    /// Creates object pool
    /// 
    /// Store each pool as a queue, take oldest object and add it back into the queue
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreateEnemyPool(Enemy_Projectile_Controller prefab, int poolSize)
    {
        // Stores the prefab identity - unique int for every game object
        int poolKey = prefab.GetInstanceID();

        // Ensures pool key isnt in dictionary
        // If key doesn't exsist, add it into the dictionary
        if (!enemyPoolDictionary.ContainsKey(poolKey))
        {
            enemyPoolDictionary.Add(poolKey, new Queue<Enemy_Projectile_Controller>());

            // Loops through the pool size, instantiatting each object and storing them into the object pool
            for (int i = 0; i < poolSize; i++)
            {
                // Instantiates prefab
                Enemy_Projectile_Controller newObject = Instantiate(prefab) as Enemy_Projectile_Controller;

                // Ensures the object is no active when spawned
                newObject.gameObject.SetActive(false);

                // Add it to the pool
                enemyPoolDictionary[poolKey].Enqueue(newObject);
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
    public void ReuseEnemyObject(Enemy_Projectile_Controller prefab, Vector2 position, Quaternion rotation, bool facingRight,
        float projectileDeaspawnTime, float projFireSpeed)
    {
        // Gets the prefab's instance id
        int poolKey = prefab.GetInstanceID();

        // Ensures prefab is in the dictionary already
        if (enemyPoolDictionary.ContainsKey(poolKey))
        {
            // Gets next object in pool
            Enemy_Projectile_Controller objectToReuse = enemyPoolDictionary[poolKey].Dequeue();

            // Adds object back to end of queue
            enemyPoolDictionary[poolKey].Enqueue(objectToReuse);

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
*/