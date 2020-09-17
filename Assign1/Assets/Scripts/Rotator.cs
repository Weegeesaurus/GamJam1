using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public AnimationCurve myCurve;
    private float startY;

    // Update is called once per frame
    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, startY+ myCurve.Evaluate(Time.time % (myCurve.length+1))-.6f, transform.position.z);
    }
}
