using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float dashPower=800;
    private Rigidbody rb;
    public TrailRenderer trail;
    public GameObject startingDashEffect;
    // Start is called before the first frame update
    void Start()
    {
        trail.emitting = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))  //press tab or esc to bring up the menu
        {
            GameObject effect = Instantiate(startingDashEffect, transform.position, new Quaternion(0, 0, 0, 0));  //create dash effect
            Destroy(effect, 1f);    //destroy after 1 second
            rb.isKinematic = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))  //press tab or esc to bring up the menu
        {
            rb.isKinematic = false;         //make sure this is disabled first
            trail.emitting = true;
            Vector3 targetDirection = Camera.main.GetComponent<CameraController>().groundPos.forward; //localizes movement with camera
            rb.AddForce(targetDirection * dashPower);   //moves the ball
            Invoke("StopTrail", 3f);
        }
    }

    private void StopTrail()
    {
        trail.emitting = false;
    }

}
