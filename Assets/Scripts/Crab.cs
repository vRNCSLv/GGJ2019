using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Crab : MonoBehaviour
{
    private static List<Crab> survivors = new List<Crab>();

    public const string ShellTag = "Shell";
    public const string CrabTag = "Player";

    [Header("Health")]
    public float harmfulCollisionThreshold;
    public int hitPoints;
    private int HitPoints
    {
        get
        {
            return hitPoints;
        }

        set
        {
            hitPoints = value;
            RefreshIcons();
        }
    }
    public int ShellHitPoints
    {
        get
        {
            Transform shell = Shell;
            if (shell)
            {
                return shell.GetComponent<Shell>().hitPoints;
            }
            else
            {
                return 0;
            }
        }
    }

    [Header("Movement")]
    public float rotationSpeed;
    public float translationSpeed;
    public float speedFactorWhenArmored; // <=1
    public float sprintImpulse;
    public float sprintCoolDownThreshold;
    public float tiltLimit;

    [Header("Keyboard controls")]
    public KeyCode cwRotationKey;
    public KeyCode ccwRotationKey;
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode sprintKey;
    public KeyCode dropKey;

    [Header("Joypad controls")]
    public string horizontalVirtualAxis;
    public string verticalVirtualAxis;
    public string sprintVirtualAxis;
    public string dropVirtualAxis;

    private Animator anim;
    private Rigidbody rb;

    private float Tilt
    {
        get
        {
            Vector3 euler = transform.rotation.eulerAngles;
            float x = euler.x;
            float z = euler.z;
            if (euler.x > 180)
            {
                x -= 360;
            }
            if (euler.z > 180)
            {
                z -= 360;
            }
            return Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
        }
    }

    private float ModifiedTranslationSpeed
    {
        get
        {
            return (state == CrabState.Armored ? speedFactorWhenArmored : 1f) * translationSpeed;
        }
    }

    public enum CrabState
    {
        Naked, Armoring, Armored, Dead
    };
    private CrabState state;

    public bool CanHurt
    {
        get
        {
            return (state == CrabState.Armored);
        }
    }

    private bool lookToCamera = false;

    public bool IsDead
    {
        get
        {
            return (state == CrabState.Dead);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
		anim = GetComponentInChildren<Animator>();
        survivors.Add(this);
        this.rb = GetComponent<Rigidbody>();
        this.state = CrabState.Naked;
        RefreshIcons();
    }

    private Vector3 euler;

    float epsilon = 0f;

    // Update is called once per frame
    void Update()
    {
        bool walking = false;
        if (rb.velocity.magnitude <= sprintCoolDownThreshold)
        {
            anim.SetBool("sprinting", false);            
        }
        if (state == CrabState.Armoring)
        {            
            Shell.GetComponent<Shell>().beingArmored = true;
            Vector3 targetPosition = ShellHandle.position;
            targetPosition.y = Mathf.Max(transform.position.y, targetPosition.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3 * translationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ShellHandle.rotation, rotationSpeed * Time.deltaTime);
            epsilon += 0.01f;
            if ((transform.position - ShellHandle.position).magnitude < epsilon &&
                Quaternion.Angle(transform.rotation, ShellHandle.rotation) < epsilon)
            {
                To(CrabState.Armored);
                Shell.SetParent(this.transform, true);
                Shell.GetComponent<Shell>().beingArmored = false;
                epsilon = 0f;
            }
            
        }
        else if (state == CrabState.Dead)
        {
            /*
            Quaternion death = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 180);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, death, rotationSpeed * Time.deltaTime);
            */
        }
        else if (lookToCamera)
        {
            Vector3 targetDir = Camera.main.transform.position - transform.position;
            Vector3 glance = Vector3.RotateTowards(transform.forward, targetDir, 0.05f, 0.05f);
            transform.rotation = Quaternion.LookRotation(glance);
        }
        else
        {
            this.euler = transform.rotation.eulerAngles;
            if (Tilt > tiltLimit)
            {
                Stabilize();
            }
            else
            {
                if (!JoypadMovement(out walking))
                {
                    KeyboardMovement(out walking);
                }
            }
        }
        anim.SetBool("walking", walking);
        if (walking)
        {
            Orchestra.Play(Orchestra.Sfx.Sand, Orchestra.Sparsely);
        }
    }

    private void To(CrabState targetState)
    {
        this.state = targetState;
        if (targetState == CrabState.Armoring)
        {
            anim.SetBool("ducking", true);
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            anim.SetBool("ducking", false);
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
        if (targetState == CrabState.Dead)
        {
            anim.SetBool("dying", true);
        }
        if (targetState == CrabState.Armored)
        {
            anim.SetBool("hasshell", true);
        }
        else
        {
            anim.SetBool("hasshell", false);
        }
        RefreshIcons();
    }

    private void Stabilize()
    {
        if (state != CrabState.Dead && state != CrabState.Armoring && rb.velocity.magnitude < 0.1f)
        {
            transform.position += 0.5f * Vector3.up;
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            transform.rotation = Quaternion.Euler(euler);
            rb.velocity = Vector3.zero;
        }

    }

    private bool JoypadMovement(out bool walking)
    {
        bool inUse = false;
        walking = false;
        Vector3 stick;
        stick.x = Input.GetAxis(horizontalVirtualAxis);
        stick.y = 0;
        stick.z = Input.GetAxis(verticalVirtualAxis);
        if (stick.magnitude > 0.1f)
        {
            inUse = true;
            transform.rotation = Quaternion.LookRotation(stick, Vector3.up);
            transform.Translate(Vector3.forward * ModifiedTranslationSpeed * stick.sqrMagnitude * Time.deltaTime, Space.Self);
            walking = true;
        }

        if (Input.GetAxis(sprintVirtualAxis) > 0)
        {
            inUse = true;
            Sprint();
            walking = true;
        }

        if (Input.GetButtonDown(dropVirtualAxis))
        {
            inUse = true;
            DropShell();
        }
        return inUse;
    }

    private void KeyboardMovement(out bool walking)
    {
        walking = false;

        // Spinning
        int way = 0;
        if (Input.GetKey(cwRotationKey))
        {
            way = +1;
        }
        else if (Input.GetKey(ccwRotationKey))
        {
            way = -1;
        }
        if (way != 0)
        {
            walking = true;
            transform.Rotate(Vector3.up, way * rotationSpeed * Time.deltaTime);
        }

        // Shifting
        way = 0;
        if (Input.GetKey(forwardKey))
        {
            way = +1;
        }
        else if (Input.GetKey(backwardKey))
        {
            way = -1;
        }
        if (way != 0)
        {
            walking = true;
            transform.Translate(way * Vector3.forward * ModifiedTranslationSpeed * Time.deltaTime, Space.Self);
        }

        if (Input.GetKeyDown(sprintKey))
        {
            walking = true;
            Sprint();
        }

        if (Input.GetKeyDown(dropKey))
        {
            DropShell();
        }
    }

    private Transform Shell
    {
        get; set;
    }

    private Transform ShellHandle
    {
        get
        {
            return Shell.GetComponent<Shell>().Handle;
        }
    }

    private void DropShell()
    {
        if (state == CrabState.Armored && rb.velocity.magnitude <= sprintCoolDownThreshold)
        {
            Transform shell = Shell;
            if (shell != null)
            {
                shell.GetComponent<Collider>().enabled = true;
                shell.GetComponent<Collider>().isTrigger = true;
                shell.SetParent(this.transform.parent); // i.e. the beach
                To(CrabState.Naked);
            }
            else
            {
                Debug.LogWarning("Shell should be dropped but it was not found");
            }
        }

    }

    private void Sprint()
    {
        if (rb.velocity.magnitude <= sprintCoolDownThreshold)
        {
            rb.AddRelativeForce(Vector3.forward * sprintImpulse, ForceMode.Impulse);
            anim.SetBool("sprinting", true);
            Orchestra.Play(Orchestra.Sfx.Sprint);
        }
    }

    private bool BeHitBy(GameObject collidingGameObject, float collisionSpeed)
    {
        if (state != CrabState.Dead && collisionSpeed >= harmfulCollisionThreshold)
        {
            Hit hit = collidingGameObject.gameObject.GetComponent<Hit>();
            if (hit)
            {                
                TakeHit(hit.PointsPerHit);
                Orchestra.Play(Orchestra.Sfx.Crash);
                return true;
            }
        }
        else
        {
            Orchestra.Play(Orchestra.Sfx.Knock, Orchestra.VerySparsely);
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == CrabState.Naked && collision.gameObject.CompareTag(ShellTag))
        {
            if (!collision.gameObject.GetComponent<Shell>().falling)
            {
                rb.velocity = Vector3.zero;
                Transform crab = this.transform;
                Shell = collision.gameObject.transform;
                Shell.GetComponent<Collider>().enabled = false;
                Shell.GetComponent<Rigidbody>().isKinematic = true;
                Shell.GetComponent<Shell>().Crab = this.gameObject;
                To(CrabState.Armoring);
            }
        }
        else
        {
            if (collision.gameObject.CompareTag(CrabTag))
            {
                anim.SetBool("clashing", true);
                Orchestra.Play(Orchestra.Sfx.Scream);
            }
            BeHitBy(collision.gameObject, collision.relativeVelocity.sqrMagnitude);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(CrabTag))
        {
            anim.SetBool("clashing", false);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        BeHitBy(collider.gameObject, (collider.attachedRigidbody.velocity - rb.velocity).sqrMagnitude);
    }

    public bool TakeHit(int points)
    {
        if (state == CrabState.Armored)
        {
            int pointsAvoided;
            if (!Shell.gameObject.GetComponent<Shell>().TakeHit(points, out pointsAvoided))
            {
                To(CrabState.Naked);
            }
            points -= pointsAvoided;
        }
        HitPoints -= points;
        if (HitPoints <= 0)
        {
            Die();
            return false;
        }
        return true;
    }

    private void Die()
    {
        To(CrabState.Dead);
        survivors.Remove(this);
        if (survivors.Count == 1)
        {
            foreach (Crab survivor in survivors) {
                survivor.Win();
                return;
            }
        }
        else if (survivors.Count == 0)
        {
            StartCoroutine(WaitAndRestart());
        }
    }

    private void Win()
    {
        Camera.main.GetComponent<VictoryShot>().Shoot(this.gameObject);
        lookToCamera = true;
        StartCoroutine(WaitAndRestart());
        Orchestra.Play(Orchestra.Sfx.Laughter);
    }

    IEnumerator WaitAndRestart()
    {
        Debug.Log("Game over");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RefreshIcons()
    {
        GetComponent<IconsManager>().SetIcons(HitPoints, ShellHitPoints);
    }
}
