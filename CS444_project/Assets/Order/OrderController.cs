using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderController : MonoBehaviour
{

    protected int orderGenerationRate = 10;
    public Order[] orderList;
    public int orderNum;
    public int orderProcessing = -1;

    protected System.Random random;

    protected int finishedOrderCount = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        Debug.LogWarningFormat("randnum = {0}", randnum);
        //int randnum = 1;
        if (randnum == 0) {
            int destination = random.Next(0, 5);
            int item = random.Next(0, 2);
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
        orderNum--;
        finishedOrderCount++;
        return true;
    }

    public void processOrder(int orderNo) {
        if ((orderNo < 0) || (orderNo >= orderList.Length) || (orderList[orderNo] == null)) return;
        orderProcessing = orderNo;
    }

    // Update is called once per frame
    void Update()
    {
        if (orderNum < 3) {
            generateOrder();
        }
    }
}