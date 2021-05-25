using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSelectionButton : WallButton
{

    [Header("Selected Panel Prefab")]
    public GameObject panelPrefab;
    
    public override void selected() {
        panelSelected();
    }

    public void panelSelected() {
        wallPanelController.setWallPanel(panelPrefab);
    }

}
