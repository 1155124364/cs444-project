using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order {

    public int destination;
    public int item;

    public void setOrder(int destination, int item) {
        this.destination = destination;
        this.item = item;
    }

}