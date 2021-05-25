using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderController : MonoBehaviour
{

    protected int orderGenerationRate = 1000;
    public Order[] orderList;
    public int orderNum;
    public int orderProcessing = -1;

    protected MessageController messageController = null;

    protected System.Random random;

    public int finishedOrderCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        messageController = GameObject.FindObjectOfType<MessageController>();
        orderList = new Order[3];
        for (int i = 0; i < 3; i++) orderList[i] = null;
        orderNum = 0;
        random = new System.Random();
    }

    protected void generateOrder() {
        int firstEmpty = 0;
        for (int i = 0; i < 3; i++) {
            if (orderList[i] == null) {
                firstEmpty = i;
                break;
            }
        }
        int randnum = random.Next(0, orderGenerationRate);
        //Debug.LogWarningFormat("randnum = {0}", randnum);
        //int randnum = 1;
        if (randnum == 0) {
            int destination = random.Next(0, 6);
            int item = random.Next(0, 3);
            orderList[firstEmpty] = new Order();
            Debug.LogWarningFormat("new order {0}", orderList[firstEmpty]);
            orderList[firstEmpty].setOrder(destination, item);
            orderNum++;
        }
    }

    public bool received(int destination, int item) {
        if ((orderProcessing < 0) || (orderProcessing >= orderList.Length) || (orderList[orderProcessing] == null)) return false;
        if ((orderList[orderProcessing].destination != destination) || (orderList[orderProcessing].item != item)) return false;
        orderList[orderProcessing] = null;
        orderProcessing = -1;
        orderNum--;
        finishedOrderCount++;
        return true;
    }

    public void processOrder(int orderNo) {
        if ((orderNo < 0) || (orderNo >= orderList.Length) || (orderList[orderNo] == null)) return;
        orderProcessing = orderNo;
    }

    protected string[] destinationName = new string[6] {"Post Office", "Bank", "Commercial Center", "Police Station", "Office Building", "Apartment"};
    protected string[] itemName = new string[3] {"Cupcake", "Croissant", "Doughnut"};

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

    public void closeMessage() {
        messageController.destroyNewMessage();
    }

    // Update is called once per frame
    void Update()
    {
        if (orderNum < 3) {
            generateOrder();
        }
    }
}