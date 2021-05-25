/*
    StatusText.cs
    Description: show the information of the order which taken by player and the number of the finished order(s) by player.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusText : MonoBehaviour
{

    protected OrderController orderController;
    
    // Start is called before the first frame update
    void Start()
    {
        orderController = GameObject.FindObjectOfType<OrderController>();
    }

    // Setting of cake types and destination
    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    // every time player finish an order, the taken order information shown in the status will become none, and Finished order count will +1.
    void Update()
    {
        if (orderController == null) {
            orderController = GameObject.FindObjectOfType<OrderController>();
        }
        int orderProcessing = orderController.orderProcessing;
        string orderString = "";
        if ((orderProcessing < 0) || (orderProcessing >= orderController.orderNum) || (orderController.orderList[orderProcessing]) == null) {
            orderString = "None";
        } else {
            Order order = orderController.orderList[orderProcessing];
            orderString = string.Format("{0} send to {1}", itemName[order.item], destinationName[order.destination]);
        }
        string statusString = string.Format("Current Order:\n{0}\n\nFinished order count: {1}", orderString, orderController.finishedOrderCount);
        gameObject.GetComponent<TMP_Text>().text = statusString;
    }
}
