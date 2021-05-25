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

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

    // Update is called once per frame
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
