using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WallButton : MonoBehaviour
{
    protected WallPanelController wallPanelController;
    
    public abstract void selected();

    public void initialize(WallPanelController wallPanelController) {
        this.wallPanelController = wallPanelController;
    }

}
