using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderButton : WallButton
{

    [Header("Order No")]
    public int orderNo;

    public override void selected() {
        orderSelected();
    }

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    
    void Update() {
        
        if (wallPanelController == null) return;
        if (wallPanelController.orderController == null) return;
        //Debug.LogWarningFormat("Order List: {0}", wallPanelController.orderController.orderList);
        Order order = wallPanelController.orderController.orderList[orderNo];
        if (order == null) {
            gameObject.GetComponentInChildren<TMP_Text>().text = "Please Wait";
        } else {
            gameObject.GetComponentInChildren<TMP_Text>().text = string.Format("Destination: {0}\n Item: {1}", destinationName[order.destination], itemName[order.item]);
        }
    }

    public void orderSelected() {
        wallPanelController.orderController.processOrder(orderNo);
    }
    
}
