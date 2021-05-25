/*
    BoxTrigger.cs
    Description: Control .
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    protected EBike eBike;

    // 
    void OnTriggerStay(Collider other)
    {
        Container container = other.GetComponent<Container>();
        if (container != null)
        {
            container.setOnBike(eBike);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        eBike = gameObject.GetComponentInParent<EBike>();
    }
}
