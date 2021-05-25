/*
	HandController.cs
	Description: Control the behavior of the e-bike.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBike : MonoBehaviour {

    // References to other objects.
    protected MainPlayerController mainPlayerController = null;
    protected Collider[] subColliders;

    // Method rideOn. Called when the player ride on the bike.
    public void rideOn(MainPlayerController mainPlayerController) {
        // Store the player controller;
        this.mainPlayerController = mainPlayerController;

        // Temporarily disable the colliders of the bike, to avoid uncontrollable collisions between the bike and the player.
        if ((subColliders == null) || (subColliders.Length == 0)) {
            subColliders = gameObject.GetComponentsInChildren<Collider>();
        }
        for (int i = 0; i < subColliders.Length; i++) {
            subColliders[i].enabled = false;
        }
        
        // Set the position of the bike to the player's position.
        Vector3 mainPosition = mainPlayerController.transform.position;
        mainPosition.y = 0.0f;
        mainPlayerController.myMoveTo(mainPosition);
    }

    // Method getOff. Called when the player get off the bike.
    public void getOff(MainPlayerController mainPlayerController) {
        // If incorrect mainPlayerController, return directly.
        if (this.mainPlayerController != mainPlayerController) return;

        // Clear the stored player controller.
        this.mainPlayerController = null;

        // Resume the colliders.
        for (int i = 0; i < subColliders.Length; i++) {
            subColliders[i].enabled = true;
        }
    }

    // Method to set the forward direction of the bike.
    public void setRotation(Quaternion rotation) {
        Vector3 euler = rotation.eulerAngles;
        // The bike should only rotate alone Y-axis.
        euler.x = 0;
        euler.z = 0;
        this.transform.rotation = Quaternion.Euler(euler);
    }

    // Start is called before the first frame update
    // When start, get the colliders of the bike.
    void Start() {
        subColliders = gameObject.GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update() {
        if (mainPlayerController != null) {
            // If the player is on the bike, the bike should follow the player's position.
            Vector3 mainPosition = mainPlayerController.transform.position;
            Vector3 mainRotation = mainPlayerController.transform.rotation.eulerAngles;
            // The bike should only rotate alone Y-axis, and should be fixed on the ground.
            mainRotation.x = 0;
            mainRotation.z = 0;
            mainPosition.y = 0;
            // The player should also be in the position. In some rare cases, the player's position may be incorrect due to uncontrollable collisions.
            mainPlayerController.myMoveTo(mainPosition);
            this.transform.position = mainPosition;
        }
    }

}
