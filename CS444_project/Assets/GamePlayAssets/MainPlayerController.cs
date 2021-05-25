/*
    MainPlayController.cs
    Description: Control the movement of the game character in two situations(on e-bike or not) and how to get on and off the e-bike.
*/

using UnityEngine;

public class MainPlayerController : MonoBehaviour {
	
	protected OVRPlayerController PlayerController = null;
	protected CharacterController Controller = null;
	protected EBike eBike = null;
	protected bool onBike = false;

	protected Collider[] controllerColliders;

	public bool isOnBike() {
		return onBike;
	}

	public void lockDefaultMovement() {
		PlayerController.EnableLinearMovement = false;
		PlayerController.EnableRotation = false;
	}

	public void unlockDefaultMovement() {
		PlayerController.EnableLinearMovement = true;
		PlayerController.EnableRotation = true;
	}

	public void myMoveTo(Vector3 moveToPosition) {
		Controller.transform.position = moveToPosition;
	}

	public void rideBike() {
		onBike = true;
		lockDefaultMovement();
		Controller.detectCollisions = false;
		if (controllerColliders == null) {
			controllerColliders = Controller.GetComponents<Collider>();
		}
		for (int i = 0; i < controllerColliders.Length; i++) {
			controllerColliders[i].enabled = false;
		}
		eBike.rideOn(this);
	}

	public void getOffBike() {
		onBike = false;
		Controller.detectCollisions = true;
		for (int i = 0; i < controllerColliders.Length; i++) {
			controllerColliders[i].enabled = true;
		}
		eBike.getOff(this);
		Vector3 mainPosition = this.transform.position;
		mainPosition.y = 121.0f;
		Vector3 delta = mainPosition - this.transform.position;
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		unlockDefaultMovement();
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		Controller.Move(delta);
	}

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

