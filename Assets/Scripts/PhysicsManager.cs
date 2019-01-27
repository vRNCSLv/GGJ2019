using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float g; // gravity constant

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, -g, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
