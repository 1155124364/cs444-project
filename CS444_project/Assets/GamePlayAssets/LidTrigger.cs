/*
    LidTrigger.cs
    Description: Control the pop-up message in front of the player.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidTrigger : MonoBehaviour
{
    protected Container container;
    // Start is called before the first frame update
    void Start()
    {
        container = gameObject.GetComponentInParent<Container>();
    }

    void OnTriggerStay(Collider other) {
        Lid lid = other.GetComponent<Lid>();
        if (lid != null) {
            lid.setOnBox(container);
        }
    }
}
