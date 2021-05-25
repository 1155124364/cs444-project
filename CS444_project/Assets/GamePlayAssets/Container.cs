/*
    Container.cs
    Description: Control the behavior of box.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    // References of other required objects.
    protected HandController handController;
    protected EBike eBike;
    protected Rigidbody rigidbody;
    protected Transform defaultParent;
    
    public bool capped = false;
    public bool onBike = false;

    // Protected member variables for inner status.
    protected int flyingFrame;
    protected bool grabbedFlying;

    public void cap() {
        capped = true;
    }

    public void uncap() {
        capped = false;
    }

    // Public method remoteGrabbed
    // Called by a hand controller when the player use the hand controller to grab the box remotely.
    // After this method is called, the box will start to fly to the controller position.
    public void remoteGrabbed(HandController handController) {

        // Disable the physics of rigid body.
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // If the box is already on the e-bike, it should move off from the e-bike first.
        if (this.onBike) {
            eBike = null;
            this.transform.SetParent(defaultParent);
            onBike = false;
        }

        // Start flying
        grabbedFlying = true;
        flyingFrame = 15;

        // Store the reference of the hand controller
        this.handController = handController;
    }

    // Public method called by the hand controller, when the player releases the box from hand.
    public void released(HandController handController, Vector3 velocity) {

        // Check whether the call is from the correct hand controller
        if (this.handController != handController) return;

        // Clear the reference to the hand controller, and resume the transform parent
        this.handController = null;
        this.transform.SetParent(defaultParent);

        // Resume the physics of rigid body.
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.velocity = velocity;
    }

    // Public method to set the box on the e-bike.
    // Called by the e-bike, and the box should move to the correct position relative to the e-bike.
    public void setOnBike(EBike eBike) {
        // If the box is already on the e-bike, or the box is grabbed on the hand, it should not attach to the e-bike.
        if (onBike) return;
        if (handController != null) return;

        // Set the inner status variable.
        onBike = true;

        // Disable the physics of rigid body.
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // Set the position and transform parent.
        this.transform.position = eBike.transform.position;
        this.transform.rotation = eBike.transform.rotation;
        Vector3 delta = new Vector3(0f, 0.7f, -0.7f);
        this.transform.position += eBike.transform.rotation * delta;
        this.transform.SetParent(eBike.transform);
        this.eBike = eBike;
    }

    // When start, get the references of other objects, and initialize inner status variables.
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbedFlying) {
            // If the box is flying towards the player's hand controller, continue flying, or stop at the controller's position.
            if (flyingFrame == 0) {
                // If the box has arrived at the controller's position, it should stop flying, and set the transform parent to the controller's transform.
                grabbedFlying = false;
                this.transform.SetParent(handController.transform);
                Debug.LogWarningFormat("{0} being grabbed!", this.name);
            } else {
                // Otherwise, keep flying.
                Vector3 flyingDirection = handController.transform.position - this.transform.position;
                flyingDirection /= (float)(flyingFrame);
                this.transform.position += flyingDirection;
                flyingFrame--;
            }
        }
    }
}
