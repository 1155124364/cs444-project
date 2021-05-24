using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItem : MonoBehaviour {
    
    protected Vector3 defaultPosition;
    protected Quaternion defaultRotation;
    protected Transform defaultParent;
    
    public enum CountableType: int {Cupcake, Croissant, Doughnut};
    [Header("Countable Type")]
    public CountableType countableType;
    
    protected bool grabbedFlying;
    protected HandController handController;
    protected int flyingFrame;

    protected Rigidbody rigidbody;
    protected Collider collider;
    
    protected Container container;

    public bool hasContainer() {
        return (container != null);
    }
    
    void Start() {
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
        container = null;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    public void remoteGrabbed(HandController handController) {
        collider.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        if (container != null) {
            this.transform.SetParent(defaultParent);
            container = null;
        }
        grabbedFlying = true;
        flyingFrame = 15;
        this.handController = handController;
    }

    public void released(HandController handController, Vector3 velocity) {
        if (this.handController != handController) return;
        this.handController = null;
        this.transform.SetParent(defaultParent);
        collider.enabled = true;
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.velocity = velocity;
    }

    public void contained(Container container) {
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        this.transform.SetParent(container.transform);
    }

    void Update() {
        if (grabbedFlying) {
            if (flyingFrame == 0) {
                grabbedFlying = false;
                this.transform.SetParent(handController.transform);
                Debug.LogWarningFormat("{0} being grabbed!", this.name);
            }
            else {
                Vector3 flyingDirection = handController.transform.position - this.transform.position;
                flyingDirection /= (float)(flyingFrame);
                this.transform.position += flyingDirection;
                flyingFrame--;
            }
        }
    }

}