using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidTrigger : MonoBehaviour
{
    protected Container container;
    // Start is called before the first frame update
    void Start()
    {
        container = gameObject.GetComponentInParent<Container>();
    }

    void OnTriggerStay(Collider other) {
        Lid lid = other.GetComponent<Lid>();
        if (lid != null) {
            //Debug.LogWarningFormat("Container {0} triggered by Lid {1}!", this.name, lid.name);
            lid.setOnBox(container);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
