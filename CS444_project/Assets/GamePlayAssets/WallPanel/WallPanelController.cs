/*
    WallPanelController.cs
    Description: Control the function of wall panel.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPanelController : MonoBehaviour
{
    // Store the item prefab corresponding to this button.
    [Header("Default Panel Prefab")]
    public GameObject defaultPanelPrefab;
    
    protected GameObject wallPanel;
    protected WallButton[] wallButtons;
    public Chef chef;
    public OrderController orderController;

    // Set initial state of wall panel
    public void setWallPanel(GameObject panelPrefab) {
        if (panelPrefab == null) return;
        if (wallPanel != null) {
            Destroy(wallPanel);
            wallPanel = null;
        }
        wallPanel = GameObject.Instantiate(panelPrefab, this.transform);
        wallButtons = wallPanel.GetComponentsInChildren<WallButton>();
        for (int i = 0; i < wallButtons.Length; i++) {
            wallButtons[i].initialize(this);
        }
    }

    // Assign each button its corresponding function.
    void Start()
    {
        chef = GameObject.FindObjectOfType<Chef>();
        orderController = GameObject.FindObjectOfType<OrderController>();
        setWallPanel(defaultPanelPrefab);
    }

}
