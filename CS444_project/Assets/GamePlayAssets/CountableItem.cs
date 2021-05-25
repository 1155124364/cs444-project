/*
    CountableItem.cs
    Description: Control the behavior of countableItem(cake).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItem : MonoBehaviour {

    // References of other required objects.
    protected Vector3 defaultPosition;
    protected Quaternion defaultRotation;
    protected Transform defaultParent;
    protected HandController handController;
    protected Rigidbody rigidbody;
    protected Collider collider;
    protected Container container;

    // Protected member variables for inner status.
    public enum CountableType: int {Cupcake, Croissant, Doughnut};
    [Header("Countable Type")]
    public CountableType countableType;   
    protected bool grabbedFlying;
    protected int flyingFrame;

    public bool hasContainer() {
        return (container != null);
    }

    // Public method remoteGrabbed
    // Called by a hand controller when the player use the hand controller to grab the cake remotely.
    // After this method is called, the cake will start to fly to the controller position.
    public void remoteGrabbed(HandController handController) {
        // Disable the physics of rigid body.
        collider.enabled = false;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // If the cake is already on the box, it should move off from the box first.
        if (container != null) {
            this.transform.SetParent(defaultParent);
            container = null;
        }

        // Start flying
        grabbedFlying = true;
        flyingFrame = 15;

        // Store the reference of the hand controller
        this.handController = handController;
    }

    // Public method called by the hand controller, when the player releases the cake from hand.
    public void released(HandController handController, Vector3 velocity) {
        // Check whether the call is from the correct hand controller

        if (this.handController != handController) return;
        // Clear the reference to the hand controller, and resume the transform parent
        this.handController = null;
        this.transform.SetParent(defaultParent);

        // Resume the physics of rigid body.
        collider.enabled = true;
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.velocity = velocity;
    }

    // Called by the container (box), and the cake should be fixed in the box.
    public void contained(Container container) {
        this.container = container;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        this.transform.SetParent(container.transform);
    }

    // When start, get the references of other objects, and initialize inner status variables.
    void Start()
    {
        defaultPosition = this.transform.position;
        defaultRotation = this.transform.rotation;
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
        container = null;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() {
        if (grabbedFlying) {
            // If the cake is flying towards the player's hand controller, continue flying, or stop at the controller's position.
            if (flyingFrame == 0) {
                // If the cake has arrived at the controller's position, it should stop flying, and set the transform parent to the controller's transform.
                grabbedFlying = false;
                this.transform.SetParent(handController.transform);
            }
            else {
                // Otherwise, keep flying.
                Vector3 flyingDirection = handController.transform.position - this.transform.position;
                flyingDirection /= (float)(flyingFrame);
                this.transform.position += flyingDirection;
                flyingFrame--;
            }
        }
    }

}