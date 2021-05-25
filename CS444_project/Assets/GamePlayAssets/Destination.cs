/*
    Destination.cs
    Description: The script for the trigger when player arrive at the destinations.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

    [Header("Destination No. ")]
    public int destination_no;

    // Reference of the orderController.
    protected OrderController orderController;
    // Reference of the messageController.
    protected MessageController messageController;

    CountableItem countableItem;

    // When the cake is delivered at the correct place, delete it.
    protected void destroyReceivedItem() {
        Destroy(countableItem.gameObject);
        countableItem = null;
    }

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    // If the cake is in the trigger collider, execute the following steps. 
    void OnTriggerEnter(Collider other) {
        countableItem = other.GetComponent<CountableItem>();

        // if cake arrived at the corresponding place.
        if (countableItem != null) {

            // if cake in the box, skip this function
            if (countableItem.hasContainer()) return;

            // if not
            int item = (int)countableItem.countableType;
            bool receiveSuccessful = orderController.received(destination_no, item);
            // And the order is correct, show order is completed message and delete the delivered cake
            if (receiveSuccessful) {
                messageController.popMessage("Order Completed!");
                Invoke("destroyReceivedItem", 3);
            }
        }

        MainPlayerController mainPlayerController = other.GetComponent<MainPlayerController>();
        // if player arrived at the corresponding place
        if (mainPlayerController != null) {
            bool isDestination = false;
            if (orderController.orderProcessing >= 0 && orderController.orderProcessing < orderController.orderList.Length) {
                Order order = orderController.orderList[orderController.orderProcessing];
                if (order != null) {
                    if (order.destination == destination_no) isDestination = true;
                }
            }

            // if player don't arrive at the correct destination, show this message.
            if (!isDestination) {
                messageController.popMessage(string.Format("Welcome to {0}!", destinationName[destination_no]));
            } else {
                // otherwise, show this message as reminder.
                messageController.popMessage(string.Format("You've arrived at {0}!\nYou can put down the item here!", destinationName[destination_no]));
            }
        }
    }

    // When start, get the reference of the order controller and message controller.
    void Start()
    {
        orderController = GameObject.FindObjectOfType<OrderController>();
        messageController = GameObject.FindObjectOfType<MessageController>();
    }
}
