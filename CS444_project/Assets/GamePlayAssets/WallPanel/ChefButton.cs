/*
    ChefButton.cs
    Description: Class for buttons on the Chef panel page. One button for one item. When selected, it will ask the chef to make a corresponding item.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefButton : WallButton {
    
    // Store the item prefab corresponding to this button.
    [Header("Selected Item Prefab")]
    public GameObject itemPrefab;

    // Call the make method of chef to make an item.
    protected void itemSelected() {
        wallPanelController.chef.make(itemPrefab);
    }

    // Inherited method from WallButton.
    public override void selected() {
        itemSelected();
    }

}
