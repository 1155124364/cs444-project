using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    protected EBike eBike;
    // Start is called before the first frame update
    void Start()
    {
        eBike = gameObject.GetComponentInParent<EBike>();
    }

    void OnTriggerStay(Collider other) {
        //Debug.LogWarningFormat("EBike {0} triggered by GameObject {1}!", this.name, other.gameObject.name);
        Container container = other.GetComponent<Container>();
        if (container != null) {
            //Debug.LogWarningFormat("EBike {0} triggered by Container {1}!", this.name, container.name);
            container.setOnBike(eBike);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
