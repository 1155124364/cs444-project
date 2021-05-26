/*
    ContainerController.cs
    Description: The script for the trigger about the box, to detect whether the cake is inside the trigger, and to set the cake attach to the box.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour {

    // Reference of the box.
    protected Container container;

    // If the cake in the box is in the trigger collider, call the contained method of countableItem to leave the cake in the box.
    void OnTriggerEnter(Collider other)
    {
        CountableItem countableItem = other.GetComponent<CountableItem>();
        if (countableItem != null)
        {
            countableItem.contained(container);
        }
    }

    // When start, get the reference of the box.
    void Start()
    {
        container = gameObject.GetComponentInParent<Container>();
    }
}
