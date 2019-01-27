using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryShot : MonoBehaviour
{
    public float zoomSpeed;

    private GameObject target = null;
    private float targetDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Transform t = target.transform;
            if ((t.position - transform.position).sqrMagnitude > targetDistance) 
            {
                transform.position = Vector3.MoveTowards(transform.position, t.position, zoomSpeed);
            }            
            transform.LookAt(t);
        }
    }

    public void Shoot(GameObject winner)
    {
        target = winner;
        targetDistance = (target.transform.position - transform.position).sqrMagnitude / 2;
    }
}
