/*
    PanelSelectionButton.cs
    Description: Deal with the selection of player. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSelectionButton : WallButton
{

    // Store the item prefab corresponding to this button.
    [Header("Selected Panel Prefab")]
    public GameObject panelPrefab;


    // Inherited method from WallButton.
    public override void selected() {
        panelSelected();
    }

    // Call the setWallPanel method of wallPanelController to set the wall panel.
    public void panelSelected() {
        wallPanelController.setWallPanel(panelPrefab);
    }

}
