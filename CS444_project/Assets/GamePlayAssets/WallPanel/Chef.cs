/*
    Chef.cs
    Description: Generate instances of items. Just like a real chef!
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour {

    // Protected Vector3 member to store the item refresh position.
    protected Vector3 defaultRefreshPosition;

    // Start is called before the first frame update
    // When start, set the default refresh position.
    void Start() {
        defaultRefreshPosition = new Vector3(-3.3f, 1.5f, 15.5f);
    }

    // Public method to generate an item instance, i.e. make a cake.
    public void make(GameObject item) {
        if (item == null) return;
        GameObject newItem = GameObject.Instantiate(item, this.transform);
        newItem.transform.position = defaultRefreshPosition;
    }

}
