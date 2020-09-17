﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotDestroy : MonoBehaviour
{
    public GameObject PickupEffect;
    // Start is called before the first frame update
    public void Pickup(GameObject player)
    {
        gameObject.SetActive(false);

        GameObject effect = Instantiate(PickupEffect, transform.position, new Quaternion(0, 0, 0, 0));  //create pickup effect
        Destroy(effect, 3f);
    }
}
