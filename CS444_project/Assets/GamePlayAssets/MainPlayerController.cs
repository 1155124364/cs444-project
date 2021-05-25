/*
    MainPlayController.cs
    Description: Control the movement of the game character in two situations(on e-bike or not) and how to get on and off the e-bike.
*/

using UnityEngine;

public class MainPlayerController : MonoBehaviour {
	
	// References of other GameObjects.
	protected OVRPlayerController PlayerController = null;
	protected CharacterController Controller = null;
	protected EBike eBike = null;
	protected Collider[] controllerColliders;
	
	// Store whether the player is on the bike.
	protected bool onBike = false;
	
	// Public method to get whether the player is on the bike.
	public bool isOnBike() {
		return onBike;
	}

	// Method to lock the default movement by hand controller.
	// Called when the player gets on the bike.
	public void lockDefaultMovement() {
		PlayerController.EnableLinearMovement = false;
		PlayerController.EnableRotation = false;
	}

	// Method to unlock the default movement. Called when the player gets off the bike.
	public void unlockDefaultMovement() {
		PlayerController.EnableLinearMovement = true;
		PlayerController.EnableRotation = true;
	}

	// Move to a position by modifying the player position directly.
	// Called when teleportation / motion ignoring the terrian is required.
	public void myMoveTo(Vector3 moveToPosition) {
		Controller.transform.position = moveToPosition;
	}

	// Public method for the player to get on the bike.
	// When the player is on the bike, the collider of the player is temporarily disabled, to avoid some bugs of uncontrollable collision with the bike.
	public void rideBike() {
		// Set the variables for status.
		onBike = true;
		lockDefaultMovement();

		// Disable the colliders.
		Controller.detectCollisions = false;
		if (controllerColliders == null) {
			controllerColliders = Controller.GetComponents<Collider>();
		}
		for (int i = 0; i < controllerColliders.Length; i++) {
			controllerColliders[i].enabled = false;
		}

		// Call the ride on method of e-bike.
		eBike.rideOn(this);

	}

	// Public method for the player to get off the bike.
	// Resume the collider of the player during this process.
	public void getOffBike() {
		// Set the variables for status.
		onBike = false;

		// Enable the colliders.
		Controller.detectCollisions = true;
		for (int i = 0; i < controllerColliders.Length; i++) {
			controllerColliders[i].enabled = true;
		}

		// Get off the bike
		eBike.getOff(this);

		// After getting off the bike, the player should be lifted to a relatively high position, and then fall to the ground, so that to avoid uncontrollable collisions.
		Vector3 mainPosition = this.transform.position;
		mainPosition.y = 10.0f;
		Vector3 delta = mainPosition - this.transform.position;
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		unlockDefaultMovement();
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		Controller.Move(delta);
	}

	// When start, get references of some other objects.
	void Start() {
		if (PlayerController == null) {
			PlayerController = gameObject.GetComponent<OVRPlayerController>();
		}
		if (Controller == null) {
			Controller = gameObject.GetComponent<CharacterController>();
			controllerColliders = Controller.GetComponents<Collider>();
		}
		if (eBike == null) {
			eBike = GameObject.FindObjectOfType<EBike>();
		}
	}
}