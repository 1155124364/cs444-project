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
		//Debug.LogWarningFormat("move to {0}", moveToPosition);
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
		//Debug.LogWarningFormat("{0} controller collider(s) found!", controllerColliders.Length);
		for (int i = 0; i < controllerColliders.Length; i++) {
			controllerColliders[i].enabled = true;
		}
		eBike.getOff(this);
		Vector3 mainPosition = this.transform.position;
		mainPosition.y = 121.0f;
		Vector3 delta = mainPosition - this.transform.position;
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		// Debug.LogWarningFormat("Controller detect: {0}, Controller Collider: {1}", Controller.detectCollisions, Controller.GetComponent<Collider>().enabled);
		unlockDefaultMovement();
		myMoveTo(mainPosition);
		this.transform.position = mainPosition;
		//PlayerController.Teleported = true;
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

		//rideBike();
	}

	void Update() {
		//Debug.LogWarningFormat("Controller position: {0}", Controller.transform.position);
		//Debug.LogWarningFormat("Main position: {0}", this.transform.position);
		//Debug.LogWarningFormat("isGrounded: {0}", Controller.isGrounded);
		// Debug.LogWarningFormat("Main position: {0}, rotation: {1}", this.transform.position, this.transform.rotation);
	}

}

