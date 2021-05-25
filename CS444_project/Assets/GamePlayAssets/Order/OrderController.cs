/*
    OrderController.cs
    Description: Control the pop-up message in front of the player.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderController : MonoBehaviour {
    
    // Public members of order information.
    public Order[] orderList;
    public int orderNum;
    public int orderProcessing = -1;
    public int finishedOrderCount = 0;

    // Protected members for order generation.
    protected int orderGenerationRate = 1000;
    protected System.Random random;

    // Members for popping messages.
    protected MessageController messageController = null;
    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    // Protected method for generating order.
    protected void generateOrder() {
        // Check the first null entry in the order list.
        int firstEmpty = 0;
        for (int i = 0; i < 3; i++) {
            if (orderList[i] == null) {
                firstEmpty = i;
                break;
            }
        }

        // Use random number to control the speed of order generation.
        int randnum = random.Next(0, orderGenerationRate);
        if (randnum == 0) {
            // Order generated!
            int destination = random.Next(0, 6);
            int item = random.Next(0, 3);
            orderList[firstEmpty] = new Order();
            orderList[firstEmpty].setOrder(destination, item);
            orderNum++;
        }
    }

    // Public method dealing with receptions.
    // Called by a destination when it receives an item.
    // Return true if the reception is exactly the current order's requirement.
    public bool received(int destination, int item) {
        // If no order processing, or the destination or item do not match the current processing order, return false.
        if ((orderProcessing < 0) || (orderProcessing >= orderList.Length) || (orderList[orderProcessing] == null)) return false;
        if ((orderList[orderProcessing].destination != destination) || (orderList[orderProcessing].item != item)) return false;

        // Otherwise, update the order list and other info, return true.
        orderList[orderProcessing] = null;
        orderProcessing = -1;
        orderNum--;
        finishedOrderCount++;
        return true;
    }

    // Public method to set an order as current processing order.
    public void processOrder(int orderNo) {
        if ((orderNo < 0) || (orderNo >= orderList.Length) || (orderList[orderNo] == null)) return;
        orderProcessing = orderNo;
    }

    // Public method to pop a status message.
    public void popStatus() {
        string orderString = "";
        if ((orderProcessing < 0) || (orderProcessing >= orderNum) || (orderList[orderProcessing]) == null) {
            orderString = "None";
        } else {
            Order order = orderList[orderProcessing];
            orderString = string.Format("{0} send to {1}", itemName[order.item], destinationName[order.destination]);
        }
        string statusString = string.Format("Current Order:\n{0}\n\nFinished order count: {1}", orderString, finishedOrderCount);
        messageController.popMessage(statusString);
    }

    // Public method to close the message.
    public void closeMessage() {
        messageController.destroyNewMessage();
    }

    // Start is called before the first frame update
    // When start, get the required GameObjects and initialize the order information.
    void Start() {
        messageController = GameObject.FindObjectOfType<MessageController>();
        orderList = new Order[3];
        for (int i = 0; i < 3; i++) orderList[i] = null;
        orderNum = 0;
        random = new System.Random();
    }

    // Update is called once per frame
    // Each frame, use generateOrder() to possibly generate an order when the orderNum is smaller than 3.
    void Update() {
        if (orderNum < 3) {
            generateOrder();
        }
    }
}