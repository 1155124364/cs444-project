/*
    Lid.cs
    Description: Control the behavior of box lid.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lid : MonoBehaviour {

    // References of other required objects.
    protected HandController handController;
    protected Transform defaultParent;
    protected Container container;
    protected Rigidbody rigidbody;

    // Protected member variables for inner status.
    protected bool grabbedFlying;
    protected bool onBox = false;
    protected int flyingFrame;
    
    // Public method to set the lid on the box.
    // Called by the container (box), and the lid should move to the correct position relative to the box.
    public void setOnBox(Container container) {
        // If the lid is already on the box, or the lid is grabbed on the hand, it should not attach to the box.
        if (onBox) return;
        if (handController != null) return;

        // Set the inner status variable.
        onBox = true;
        this.container = container;

        // Disable the physics of rigid body.
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // Set the position and transform parent.
        this.transform.position = container.transform.position;
        this.transform.rotation = container.transform.rotation;
        Vector3 y = new Vector3(0f, 0.36f, 0f);
        this.transform.position += container.transform.rotation * y;
        this.transform.SetParent(container.transform);
    }
    
    // Public method remoteGrabbed
    // Called by a hand controller when the player use the hand controller to grab the lid remotely.
    // After this method is called, the lid will start to fly to the controller position.
    public void remoteGrabbed(HandController handController) {
        // Disable the physics of rigid body.
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        // If the lid is already on the box, it should move off from the box first.
        if (this.onBox) {
            container.uncap();
            container = null;
            this.transform.SetParent(defaultParent);
            onBox = false;
        }

        // Start flying
        grabbedFlying = true;
        flyingFrame = 15;

        // Store the reference of the hand controller
        this.handController = handController;
    }

    // Public method called by the hand controller, when the player releases the lid from hand.
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

    // Start is called before the first frame update
    // When start, get the references of other objects, and initialize inner status variables.
    void Start() {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        defaultParent = this.transform.parent;
        grabbedFlying = false;
        handController = null;
    }

    // Update is called once per frame
    // 3 different situations are dealt with during update
    void Update() {
        if (grabbedFlying) {
            // If the lid is flying towards the player's hand controller, continue flying, or stop at the controller's position.
            if (flyingFrame == 0) {
                // If the lid has arrived at the controller's position, it should stop flying, and set the transform parent to the controller's transform.
                grabbedFlying = false;
                this.transform.SetParent(handController.transform);
            } else {
                // Otherwise, keep flying.
                Vector3 flyingDirection = handController.transform.position - this.transform.position;
                flyingDirection /= (float)(flyingFrame);
                this.transform.position += flyingDirection;
                flyingFrame--;
            }
        }
        else if (onBox) {
            // If the lid is on the box, it should keep attaching to the box.
            // This part is to avoid uncontrollable collisions between the box and the lid in some very rare situations.
            this.transform.position = container.transform.position;
            this.transform.rotation = container.transform.rotation;
            Vector3 y = new Vector3(0f, 0.36f, 0f);
            this.transform.position += container.transform.rotation * y;
            this.transform.SetParent(container.transform);
            this.container = container;
        }
        if (this.transform.position.y < -10f) {
            // In some very rare situations, the lid may fall under the ground, we may reset the lid position when this happens.
            Vector3 reset = this.transform.position;
            reset.y = 10f;
            this.transform.position = reset;
        }
    }
    
}
