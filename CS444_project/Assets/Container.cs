using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    
    protected bool grabbedFlying;
    protected HandController handController;
    protected int flyingFrame;

    protected Transform defaultParent;
    
    public bool capped = false;

    public void cap() {
        capped = true;
    }

    public void uncap() {
        capped = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
    }

    public void remoteGrabbed(HandController handController) {
        grabbedFlying = true;
        flyingFrame = 15;
        this.handController = handController;
    }

    public void released(HandController handController) {
        if (this.handController != handController) return;
        this.handController = null;
        this.transform.SetParent(defaultParent);
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedFlying) {
            if (flyingFrame == 0) {
                grabbedFlying = false;
                this.transform.SetParent(handController.transform);
                Debug.LogWarningFormat("{0} being grabbed!", this.name);
            } else {
                Vector3 flyingDirection = handController.transform.position - this.transform.position;
                flyingDirection /= (float)(flyingFrame);
                this.transform.position += flyingDirection;
                flyingFrame--;
            }
        }
    }
}
