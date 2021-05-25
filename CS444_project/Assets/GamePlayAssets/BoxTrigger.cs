/*
    BoxTrigger.cs
    Description: The script for the trigger above the e-bike, to detect whether the box is inside the trigger, and to set the box attach to the e-bike.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    // Reference of the e-bike.
    protected EBike eBike;

    // If the box is in the trigger collider, call the setOnBike method of container to ask the box to attach to the e-bike. 
    void OnTriggerStay(Collider other)
    {
        Container container = other.GetComponent<Container>();
        if (container != null)
        {
            container.setOnBike(eBike);
        }
    }

    // When start, get the reference of the e-bike.
    void Start()
    {
        eBike = gameObject.GetComponentInParent<EBike>();
    }
}
