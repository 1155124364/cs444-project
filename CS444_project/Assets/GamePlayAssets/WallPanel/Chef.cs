using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{

    Vector3 defaultRefreshPosition;

    // Start is called before the first frame update
    void Start()
    {
        defaultRefreshPosition = new Vector3(-3.3f, 1.5f, 15.5f);
    }

    public void make(GameObject item) {
        if (item == null) return;
        GameObject newItem = GameObject.Instantiate(item, this.transform);
        newItem.transform.position = defaultRefreshPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
