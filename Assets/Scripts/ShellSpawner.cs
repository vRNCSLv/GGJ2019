using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSpawner : MonoBehaviour
{
    public Crab[] crabsToFollow;
    public Shell[] prefabsToSpawn;
    public float minInterval;
    public float maxInterval;

    private float timeToNextSpawn;

    // Start is called before the first frame update
    void Start()
    {
        SetNextInterval();
    }

    private void SetNextInterval()
    {
        timeToNextSpawn = UnityEngine.Random.Range(minInterval, maxInterval);
    }

    // Update is called once per frame
    void Update()
    {
        timeToNextSpawn -= Time.deltaTime;
        if (timeToNextSpawn <= 0)
        {
            Spawn();
            SetNextInterval();
        }
    }

    private void Spawn()
    {
        int i = UnityEngine.Random.Range(0, crabsToFollow.Length);
        Crab weakest = crabsToFollow[i];
        int minTotalHitPoints = weakest.hitPoints + weakest.ShellHitPoints;
        foreach (Crab crab in crabsToFollow)
        {
            if (crab != weakest && crab.hitPoints + crab.ShellHitPoints < minTotalHitPoints) // strictly less than
            {
                weakest = crab;
            }
        }
        if (weakest.IsDead)
        {
            Destroy(this);
            return;
        }

        Vector3 position = this.transform.position;
        position.x = weakest.transform.position.x;
        position.z = weakest.transform.position.z;
        this.transform.position = position;        

        i = UnityEngine.Random.Range(0, prefabsToSpawn.Length);
        GameObject prefab = prefabsToSpawn[i].gameObject;
        GameObject newborn = Instantiate<GameObject>(prefab);
        newborn.tag = Crab.ShellTag; // just to play safe        
        newborn.transform.parent = this.transform.parent; // i.e. the beach
        newborn.transform.localScale = prefab.transform.localScale;
        newborn.transform.position = this.transform.position;
        newborn.GetComponent<Shell>().falling = true;
    }
}
