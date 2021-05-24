using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPanelController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Default Panel Prefab")]
    public GameObject defaultPanelPrefab;
    
    protected GameObject wallPanel;
    protected WallButton[] wallButtons;
    public Chef chef;
    public OrderController orderController;

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
    
    void Start()
    {
        chef = GameObject.FindObjectOfType<Chef>();
        orderController = GameObject.FindObjectOfType<OrderController>();
        setWallPanel(defaultPanelPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
