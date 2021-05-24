using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    [Header("Destination No. ")]
    public int destination_no;
    protected OrderController orderController;

    // Start is called before the first frame update
    void Start() {
        orderController = GameObject.FindObjectOfType<OrderController>();
    }

    CountableItem countableItem;

    protected void destroyReceivedItem() {
        Destroy(countableItem.gameObject);
        countableItem = null;
    }

    void OnTriggerEnter(Collider other) {
        countableItem = other.GetComponent<CountableItem>();
        if (countableItem != null) {
            if (countableItem.hasContainer()) return;
            int item = (int)countableItem.countableType;
            Debug.LogWarningFormat("Destination {0} triggered by CountableItem {1}!", destination_no, item);
            bool receiveSuccessful = orderController.received(destination_no, item);
            if (receiveSuccessful) {
                Invoke("destroyReceivedItem", 3);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
