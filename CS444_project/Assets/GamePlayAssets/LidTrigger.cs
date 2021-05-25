/*
    LidTrigger.cs
    Description: The script for the trigger above the box, to detect whether the lid is inside the trigger, and to set the lid attach to the box.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidTrigger : MonoBehaviour {
    // Reference of the box.
    protected Container container;
    
    // Start is called before the first frame update
    // When start, get the reference of the box.
    void Start() {
        container = gameObject.GetComponentInParent<Container>();
    }

    // If the lid is in the trigger collider, call the setOnBox method of lid to ask the lid to attach to the box.
    void OnTriggerStay(Collider other) {
        Lid lid = other.GetComponent<Lid>();
        if (lid != null) {
            lid.setOnBox(container);
        }
    }
}
