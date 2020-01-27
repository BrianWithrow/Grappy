using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{    
    public float moveSpeed;
    public float jumpSpeed;
    public float grappleForce;
    public float handSpeed;

    public float groundCheckRadius;
    public LayerMask whatIsGround;

    public bool grounded;
    private bool facingRight;
    public bool dead;
    private bool victory;

    public Transform sprite;
    public Transform feet;
    public Transform handF;
    public Transform handB;

    private Transform grappable;

    private HingeJoint grappableHingeJoint;

    private Vector3 respawnPoint;
    private Vector3 handFInitPos;
    private Vector3 handBInitPos;

    private Rigidbody rb;
    private Animator anim;
    private SpriteRenderer sr;
    private SpriteRenderer srHandF;
    private SpriteRenderer srHandB;
    private AudioManager am;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        srHandF = handF.GetComponent<SpriteRenderer>();
        srHandB = handB.GetComponent<SpriteRenderer>();
        am = FindObjectOfType<AudioManager>();

        handFInitPos = handF.localPosition;
        handBInitPos = handB.localPosition;

        facingRight = true;
        dead = false;
        victory = false;
    }
    
    private void LateUpdate()
    {
        if (rb.velocity.x > 0.0f)
        {
            facingRight = true;
        }
        else if (rb.velocity.x < 0.0f)
        {
            facingRight = false;
        }

        if (facingRight)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        anim.SetFloat("VelX", Math.Abs(rb.velocity.x));
        anim.SetBool("Grounded", grounded);
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(feet.transform.position, groundCheckRadius, whatIsGround);

        sr.enabled = !dead && !victory;
        srHandF.enabled = !dead && !victory;
        srHandB.enabled = !dead && !victory;
       
        if (grappable == null)
        {
            if (!dead)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    GrappableHit();
                }

                Movement();
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                handF.parent = null;
                
                handF.Translate((grappable.position - handF.position) * handSpeed * Time.deltaTime);

                if (Vector3.Distance(handF.position, grappable.position) < 0.5f)
                {
                    handF.position = grappable.position;

                    Vector3 dir = grappable.transform.position - transform.position;
                    rb.AddForce(dir.normalized * grappleForce);
                }
            }
            else if (Input.GetMouseButton(1))
            {
                handB.parent = null;

                handB.Translate((grappable.position - handB.position) * handSpeed * Time.deltaTime);

                if (Vector3.Distance(handB.position, grappable.position) < 0.5f)
                {
                    if (grappableHingeJoint == null)
                    {
                        grappableHingeJoint = grappable.gameObject.AddComponent<HingeJoint>();

                        grappableHingeJoint.connectedBody = rb;
                        grappableHingeJoint.anchor = Vector3.zero;
                        grappableHingeJoint.axis = Vector3.forward;
                        grappableHingeJoint.autoConfigureConnectedAnchor = true;
                        grappableHingeJoint.connectedMassScale = 3.0f;

                        rb.constraints = RigidbodyConstraints.FreezePositionZ;
                    }
                }
            }
            else
            {
                Reset();
            }
        }
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public void Reset()
    {
        if (grappableHingeJoint != null)
        {
            RemoveHingeJoint();
        }

        ResetHand(handF, handFInitPos);
        ResetHand(handB, handBInitPos);

        grappable = null;
    }

    private void RemoveHingeJoint()
    {
        grappableHingeJoint = null;
        Destroy(grappable.gameObject.GetComponent<HingeJoint>());

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void ResetHand(Transform hand, Vector3 initPos)
    {
        hand.parent = transform;
        hand.localPosition = initPos;
        hand.rotation = Quaternion.identity;
        hand.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    private void GrappableHit()
    {
        RaycastHit hitInfo = new RaycastHit();

        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hit)
        {
            if (hitInfo.transform.gameObject.tag.Contains("Grappleable"))
            {
                grappable = hitInfo.transform;
            }
        }
    }

    private void Movement()
    {
        if (Input.GetAxisRaw("Horizontal") > 0.0f)
        {
            if (grounded)
            {
                rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
            }
            else
            {
                if (rb.velocity.x <= 0.1f)
                {
                    rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
                }
                else
                {
                    rb.AddForce(Vector3.right * 2.0f);
                }
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < 0.0f)
        {
            if (grounded)
            {
                rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
            }
            else
            {
                if (rb.velocity.x >= -0.1f)
                {
                    rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
                }
                else
                {
                    rb.AddForce(Vector3.right * -2.0f);
                }
            }
        }
        else
        {
            if (grounded)
            {
                rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
            }
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            rb.AddForce(Vector3.down * 10.0f);
        }
    }

    public IEnumerator DeadRoutine()
    {
        dead = true;

        rb.isKinematic = true;

        am.Pause("Theme");

        am.Play("Death");

        rb.velocity = Vector3.zero;
        
        yield return new WaitForSeconds(am.GetSound("Victory").source.clip.length);

        am.Play("Theme");

        transform.position = respawnPoint;

        Camera.main.GetComponent<CameraManager>().Reset(respawnPoint);

        rb.isKinematic = false;

        dead = false;
    }

    public IEnumerator VictoryLap(int sceneIndex)
    {
        victory = true;

        rb.isKinematic = true;

        am.Pause("Theme");

        am.Play("Victory");
        
        yield return new WaitForSeconds(am.GetSound("Victory").source.clip.length);

        SceneManager.LoadScene(sceneIndex);

        if (sceneIndex == 4)
        {
            am.Stop("Theme");
            am.Play("Title");
        }
        else
        {
            am.Play("Theme");
        }
        

        rb.isKinematic = false;

        victory = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            respawnPoint = transform.position;

            other.GetComponent<CheckPointFlag>().Rise();

            Debug.Log("Checkpoint");
        }
        else if (other.tag.Contains("Spikes") && !dead)
        {
            Reset();

            StartCoroutine(DeadRoutine());
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Exit")
        {
            if (Input.GetAxis("Vertical") > 0.0f && !victory)
            {
                StartCoroutine(VictoryLap(other.GetComponent<ExitPassage>().nextScene));
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Spikes") && !dead)
        {
            Reset();

            StartCoroutine(DeadRoutine());
        }
    }
}
