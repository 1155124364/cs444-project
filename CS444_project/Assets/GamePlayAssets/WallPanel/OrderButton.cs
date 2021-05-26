/*
    OrderButton.cs
    Description: class for buttons on the Order panel page. One button for one item. When selected, it will be taken by player, and player can according to the order to deliver the cake.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderButton : WallButton
{
    // Store the item number corresponding to this button.
    [Header("Order No")]
    public int orderNo;

    public override void selected() {
        orderSelected();
    }

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    // Update the order list when the taken order is completed. Add new order and delete the finished one.
    void Update() {
        
        if (wallPanelController == null) return;
        if (wallPanelController.orderController == null) return;
        Order order = wallPanelController.orderController.orderList[orderNo];
        if (order == null) {
            gameObject.GetComponentInChildren<TMP_Text>().text = "Please Wait";
        } else {
            gameObject.GetComponentInChildren<TMP_Text>().text = string.Format("Destination: {0}\n Item: {1}", destinationName[order.destination], itemName[order.item]);
        }
    }

    // Call the processOrder method of orderController to process the order.
    public void orderSelected() {
        wallPanelController.orderController.processOrder(orderNo);
    }
    
}
