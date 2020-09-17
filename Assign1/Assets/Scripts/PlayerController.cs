using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //movement
    public float speed;
    public float jumpPower = 10;
    private Vector3 movement;   //movement direction from input
    private float distToGround; //default distance from ground
    private bool canJump = true;
    private float jumpCooldown = .1f;   //prevents the jump function being called many times in a second

    //gameplay
    public int checkpoint = 0;
    public int score=0;
    private bool dead = false;          //is player dead?
    public float respawnTime = 3;
    private float fade = 0f;    //fade for death

    //UI
    public Text scoreText;
    public Text speedText;      //hooks up UI

    //Systems
    private Rigidbody rb;
    private MeshRenderer render;
    private Material mat;





    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        UpdateScore(0); //sets up the UI
        rb = GetComponent<Rigidbody>();
        render = GetComponent<MeshRenderer>();
        mat = render.material;      //get the material

        distToGround = (float)GetComponent<SphereCollider>().bounds.extents.y;  //calculates dist from ground
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");     //getting input

        movement = new Vector3(moveHorizontal, 0f, moveVertical);   //combining input
    }

    private void FixedUpdate()
    {
        if (!dead)  //movement
        {
            Vector3 targetDirection = Camera.main.GetComponent<CameraController>().groundPos.TransformDirection(movement);  //localizes movement with camera
            rb.AddForce(targetDirection * speed);   //moves the ball

            if (Input.GetButton("Jump") && IsGrounded() && canJump)   //jump
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                canJump = false;
                Invoke("JumpRestore", jumpCooldown);
            }
        }

        if (dead)
        {
            fade += Time.deltaTime / respawnTime;  //dissolve the player model
            mat.SetFloat("_Fade", fade);
        }

        speedText.text = Mathf.RoundToInt(rb.velocity.magnitude * 5).ToString();  //sets the UI speed
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pick Up"))  //when picking up a "coin"
        {
            other.gameObject.GetComponent<CoinPickUp>().Pickup(gameObject);
        }
        if (other.gameObject.CompareTag("Destructable"))  //when picking up a "coin"
        {
            other.gameObject.GetComponent<CarrotDestroy>().Pickup(gameObject);
        }

        if (other.gameObject.CompareTag("Water") && !dead)  //when picking up a "coin"
        {
            rb.useGravity = false;
            dead = true;
            rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);    //bounces the player upward, "float back up"
            Invoke("Respawn", respawnTime);                     //start respawn
        }

        if (other.gameObject.CompareTag("Checkpoint"))  //checkpoint
        {
            if (other.gameObject.GetComponent<Checkpoint>().Activate(checkpoint))   //try to activate the checkpoint
            {
                checkpoint = other.gameObject.GetComponent<Checkpoint>().checkpoint;    //set new checkpoint

                GameObject[] checks = GameObject.FindGameObjectsWithTag("Checkpoint");  //get all other checkpoints
                foreach (GameObject obj in checks)                                      //sort through all other ones
                {
                    obj.GetComponent<Checkpoint>().SelfCheck(checkpoint);           //deletes old checkpoints

                }
            }
        }
    }
    public void UpdateScore(int num)    //updates and addes to the score
    {
        score += num;
        scoreText.text = "Score: " + score.ToString();
    }

    private void JumpRestore()    //updates and addes to the score
    {
        canJump = true;
    }

    public void Respawn()
    {
        rb.useGravity = true;
        rb.velocity = Vector3.zero;

        GameObject[] checks = GameObject.FindGameObjectsWithTag("Checkpoint");  //find the checkpoint we are at
        foreach (GameObject obj in checks)
        {
            if (obj.GetComponent<Checkpoint>().checkpoint == checkpoint)
            {
                transform.position = obj.transform.position;    //teleport to the checkpoint
            }

        }

        dead = false;
        fade = 0;
        mat.SetFloat("_Fade", 0);   //undissolve the player model
    }

    private bool IsGrounded()  //checks if the player is on thr ground
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit ,distToGround + 0.1f)) //raycast to check for collision
        {
            if (hit.collider.gameObject.tag!="Pick Up" && hit.collider.gameObject.tag != "Checkpoint" && hit.collider.gameObject.tag != "Water")  //if we hit an object that isnt a pickup/checkpoint
            {
                return true;
            }
        }
    return false;
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Moving Platform")
        {
            transform.parent = other.transform;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Moving Platform")
        {
            transform.parent = null;

        }
    }
}
