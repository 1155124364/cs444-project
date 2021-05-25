using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lid : MonoBehaviour
{
    protected bool grabbedFlying;
    protected HandController handController;
    protected int flyingFrame;

    protected Transform defaultParent;
    protected Container container;

    protected Rigidbody rigidbody;

    protected bool onBox = false;

    public void setOnBox(Container container) {
        if (onBox) return;
        if (handController != null) return;
        onBox = true;

        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        this.transform.position = container.transform.position;
        this.transform.rotation = container.transform.rotation;
        Vector3 y = new Vector3(0f, 0.36f, 0f);
        this.transform.position += container.transform.rotation * y;
        this.transform.SetParent(container.transform);
        this.container = container;
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

        if (this.onBox) {
            container.uncap();
            container = null;
            this.transform.SetParent(defaultParent);
            onBox = false;
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
        else if (onBox) {
            this.transform.position = container.transform.position;
            this.transform.rotation = container.transform.rotation;
            Vector3 y = new Vector3(0f, 0.36f, 0f);
            this.transform.position += container.transform.rotation * y;
            this.transform.SetParent(container.transform);
            this.container = container;
        }
        if (this.transform.position.y < -10f) {
            Vector3 reset = this.transform.position;
            reset.y = 10f;
            this.transform.position = reset;
        }
        Debug.LogWarningFormat("lid position {0}", this.transform.position);
    }
}
