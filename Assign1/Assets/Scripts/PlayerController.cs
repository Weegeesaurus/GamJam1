﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //movement
    public float speed;
    public float jumpPower = 10;
    private Vector3 movement;       //movement direction from input
    private float distToGround;     //default distance from ground
    private bool canJump = true;
    private float jumpCooldown = .1f;   //prevents the jump function being called many times in a second

    //gameplay
    public int checkpoint = 0;      //current checkpoint
    public int score=0;
    [SerializeField]
    private bool dead = false;          //is player dead?
    public float respawnTime = 3;
    private float fade = 0f;    //timer for dissolving animation

    //UI
    public Text scoreText;
    public Text speedText;      //hooks up UI

    //Systems
    private Rigidbody rb;
    private MeshRenderer render;
    private Material mat;           //colors and texture





    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        UpdateScore(0);                     //sets up the UI incase its messed up in the editor
        rb = GetComponent<Rigidbody>();
        render = GetComponent<MeshRenderer>();
        mat = render.material;              //get the material

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
                Invoke("JumpRestore", jumpCooldown);    //restores the jump after a certain cooldown
            }
        }

        if (dead)
        {
            fade += Time.deltaTime / respawnTime;   //progress dissolve animation over time
            mat.SetFloat("_Fade", fade);            //dissolve the player model
        }

        speedText.text = Mathf.RoundToInt(rb.velocity.magnitude * 5).ToString();  //sets the UI speed
        //^^left this in FixedUpdate for performance as speed is determined every FixedUpdate
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pick Up"))  //when picking up a "coin"
        {
            other.gameObject.GetComponent<CoinPickUp>().Pickup(gameObject);
        }

        if (other.gameObject.CompareTag("Destructable"))  //when rolling over a destructable
        {
            other.gameObject.GetComponent<CarrotDestroy>().Pickup(gameObject);  //this is hard coded to the carrot
        }

        if (other.gameObject.CompareTag("Water") && !dead)  //when touching the water killbox
        {
            rb.useGravity = false;  //turns off gravity
            dead = true;
            rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);    //bounces the player upward, "float back up"
            Invoke("Respawn", respawnTime);                     //start respawn
        }

        if (other.gameObject.CompareTag("Checkpoint"))  //when touching checkpoint
        {
            if (other.gameObject.GetComponent<Checkpoint>().Activate(checkpoint))   //if the checkpoint activates successfully
            {
                checkpoint = other.gameObject.GetComponent<Checkpoint>().checkpointNum;    //set new checkpoint

                GameObject[] checks = GameObject.FindGameObjectsWithTag("Checkpoint");  //get all other checkpoints
                foreach (GameObject obj in checks)                                      //sort through all other ones
                {
                    obj.GetComponent<Checkpoint>().SelfCheck(checkpoint);           //deletes old checkpoints

                }
            }
        }
    }
    public void UpdateScore(int num)    //updates and adds to the score
    {
        score += num;
        scoreText.text = "Score: " + score.ToString();
    }

    private void JumpRestore()    //refreshes the jump
    {
        canJump = true;
    }

    public void Respawn()
    {
        rb.useGravity = true;       //reactivate gravity
        rb.velocity = Vector3.zero; //reset player velocity

        GameObject[] checks = GameObject.FindGameObjectsWithTag("Checkpoint");  //find the checkpoint we are at
        foreach (GameObject obj in checks)
        {
            if (obj.GetComponent<Checkpoint>().checkpointNum == checkpoint) //looking for the right one
            {
                transform.position = obj.transform.position;    //teleport to the checkpoint
            }

        }

        dead = false;
        fade = 0;           //reset the dissolve progress for later
        mat.SetFloat("_Fade", 0);   //undissolve the player model
    }

    private bool IsGrounded()  //checks if the player is on the ground
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

    void OnTriggerStay(Collider other)  //attach the player to the platform
    {

        if (other.gameObject.tag == "Moving Platform")
        {
            transform.parent = other.transform;

        }
    }

    void OnTriggerExit(Collider other)  //detach the player from the platform
    {
        if (other.gameObject.tag == "Moving Platform")
        {
            transform.parent = null;    //not the cleanest method but it works for small scale projects

        }
    }
}
