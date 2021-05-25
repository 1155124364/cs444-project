/*
    Order.cs
    Description: the class for orders, contains order destination and item information.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order {

    // Member variables, which can be publicly accessed.
    public int destination;
    public int item;

    // Interface for setting the order parameters.
    public void setOrder(int destination, int item) {
        this.destination = destination;
        this.item = item;
    }

}