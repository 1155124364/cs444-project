/*
    WallButton.cs
    Description: class for buttons on the wall panel page. One button for one item. When selected, it will show corresponding interface.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WallButton : MonoBehaviour
{
    protected WallPanelController wallPanelController;
    
    public abstract void selected();

    // Initialize the wall control panel.
    public void initialize(WallPanelController wallPanelController) {
        this.wallPanelController = wallPanelController;
    }

}
