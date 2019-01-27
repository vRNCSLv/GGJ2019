using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public const string GroundTag = "Ground";
    public const string HandleTag = "Handle";
    
    public float rotationSpeed;    
    public bool falling;
    public int hitPoints;
    public bool beingArmored;    

    private Transform handle;
    public Transform Handle
    {
        get
        {
            return handle;
        }
    }

    internal GameObject Crab
    {
        private get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.handle = transform.Find(HandleTag);
    }

    // Update is called once per frame
    void Update()
    {
        if (beingArmored)
        {            
            float deltaY = Crab.transform.position.y - Handle.transform.position.y;
            if (deltaY > 0.1f)
            {
                deltaY = 0.1f;
            }
            if (deltaY < -0.1f)
            {
                deltaY = -0.1f;
            }
            transform.Translate(Vector3.up * deltaY, Space.World);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Inverse(Handle.localRotation), rotationSpeed * Time.deltaTime);
        }
    }

    

    private void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == Crab)
        {
            GetComponent<Collider>().isTrigger = false;
            GetComponent<Rigidbody>().isKinematic = false;
            Crab = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GroundTag))
        {
            if (falling)
            {
                Orchestra.Play(Orchestra.Sfx.Knock, Orchestra.Sparsely);
            }
            falling = false;
        }
    }

    internal bool TakeHit(int points, out int taken)
    {
        if (points >= this.hitPoints)
        {
            taken = hitPoints;
            this.hitPoints = 0;
            Die();
            return false;
        }        
        else
        {
            taken = points;
            this.hitPoints -= points;
            return true;
        }
    }
}
