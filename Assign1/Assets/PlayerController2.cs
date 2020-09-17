using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour
{
    public float speed;
    public Text scoreText;
    public GameObject PickupEffect;
    private Rigidbody rb;
    private Vector3 movement;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody>();
        updateScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0f, moveVertical);
        scoreText.text = "Score: " + Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        Vector3 targetDirection = Camera.main.GetComponent<CameraController>().groundPos.TransformDirection(movement);  //localizes movement with camera
        rb.AddForce(targetDirection * speed);   //moves the ball
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void updateScore(int num)
    {
        score += num;
        scoreText.text = "Score: " + score.ToString();
    }
}
