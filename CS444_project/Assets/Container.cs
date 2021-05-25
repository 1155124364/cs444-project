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

    public bool onBike = false;

    protected EBike eBike;

    protected Rigidbody rigidbody;

    public void cap() {
        capped = true;
    }

    public void uncap() {
        capped = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
    }

    public void remoteGrabbed(HandController handController) {

        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        if (this.onBike) {
            eBike = null;
            this.transform.SetParent(defaultParent);
            onBike = false;
        }

        grabbedFlying = true;
        flyingFrame = 15;
        this.handController = handController;
    }

    public void released(HandController handController, Vector3 velocity) {
        if (this.handController != handController) return;
        this.handController = null;
        this.transform.SetParent(defaultParent);
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.velocity = velocity;
    }

    public void setOnBike(EBike eBike) {
        if (onBike) return;
        if (handController != null) return;

        onBike = true;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        this.transform.position = eBike.transform.position;
        this.transform.rotation = eBike.transform.rotation;
        Vector3 delta = new Vector3(0f, 0.7f, -0.7f);
        this.transform.position += eBike.transform.rotation * delta;
        this.transform.SetParent(eBike.transform);
        this.eBike = eBike;
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
