using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    public int pointsPerHit;
    public int PointsPerHit
    {
        get
        {
            Crab crab = GetComponent<Crab>();
            if (crab)
            {
                if (!crab.CanHurt)
                {
                    return 0;
                }
            }
            return pointsPerHit;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
