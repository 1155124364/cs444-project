using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    [Header("Destination No. ")]
    public int destination_no;
    protected OrderController orderController;
    protected MessageController messageController;

    // Start is called before the first frame update
    void Start() {
        orderController = GameObject.FindObjectOfType<OrderController>();
        messageController = GameObject.FindObjectOfType<MessageController>();
    }

    CountableItem countableItem;

    protected void destroyReceivedItem() {
        Destroy(countableItem.gameObject);
        countableItem = null;
    }

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    void OnTriggerEnter(Collider other) {
        countableItem = other.GetComponent<CountableItem>();
        if (countableItem != null) {
            if (countableItem.hasContainer()) return;
            int item = (int)countableItem.countableType;
            Debug.LogWarningFormat("Destination {0} triggered by CountableItem {1}!", destination_no, item);
            bool receiveSuccessful = orderController.received(destination_no, item);
            if (receiveSuccessful) {
                messageController.popMessage("Order Completed!");
                Invoke("destroyReceivedItem", 3);
            }
        }
        MainPlayerController mainPlayerController = other.GetComponent<MainPlayerController>();
        if (mainPlayerController != null) {
            bool isDestination = false;
            if (orderController.orderProcessing >= 0 && orderController.orderProcessing < orderController.orderList.Length) {
                Order order = orderController.orderList[orderController.orderProcessing];
                if (order != null) {
                    if (order.destination == destination_no) isDestination = true;
                }
            }
            if (!isDestination) {
                messageController.popMessage(string.Format("Welcome to {0}!", destinationName[destination_no]));
            } else {
                messageController.popMessage(string.Format("You've arrived at {0}!\nYou can put down the item here!", destinationName[destination_no]));
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
