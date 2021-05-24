using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour {
    protected Container container;

    // Start is called before the first frame update
    void Start()
    {
        container = gameObject.GetComponentInParent<Container>();
    }

    void OnTriggerEnter(Collider other) {
        CountableItem countableItem = other.GetComponent<CountableItem>();
        if (countableItem != null) {
            Debug.LogWarningFormat("Container {0} triggered by CountableItem {1}!", this.name, countableItem.name);
            countableItem.contained(container);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
