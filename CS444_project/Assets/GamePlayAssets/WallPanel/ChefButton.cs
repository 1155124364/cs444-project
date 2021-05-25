using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefButton : WallButton
{
    
    [Header("Selected Item Prefab")]
    public GameObject itemPrefab;

    public override void selected() {
        itemSelected();
    }

    public void itemSelected() {
        wallPanelController.chef.make(itemPrefab);
    }

}
