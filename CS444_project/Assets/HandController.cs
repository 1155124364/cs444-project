using UnityEngine;
using System;
using System.Collections.Generic;

public class HandController : MonoBehaviour {

	// Store the hand type to know which button should be pressed
	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;


	// Store the player controller to forward it to the object
	[Header( "Player Controller" )]
	public MainPlayerController playerController;



	// Store all gameobjects containing an Anchor
	// N.B. This list is static as it is the same list for all hands controller
	// thus there is no need to duplicate it for each instance
	static protected ObjectAnchor[] anchors_in_the_scene;

	static protected EBike eBike;

	void Start () {
		// Prevent multiple fetch
		if ( anchors_in_the_scene == null ) anchors_in_the_scene = GameObject.FindObjectsOfType<ObjectAnchor>();
		if (playerController == null) playerController = GameObject.FindObjectOfType<MainPlayerController>();
		if (eBike == null) eBike = GameObject.FindObjectOfType<EBike>();
		originalPointingDirection = new Vector3(0f, 1f, 1f);
		originalPointingDirection.Normalize();

		recentPosition = new Vector3[4]{this.transform.position, this.transform.position, this.transform.position, this.transform.position};
	}


	// This method checks that the hand is closed depending on the hand side
	protected bool is_hand_closed () {
		// Case of a left hand
		if ( handType == HandType.LeftHand ) return
			OVRInput.Get( OVRInput.Button.Three )                           // Check that the A button is pressed
			&& OVRInput.Get( OVRInput.Button.Four )                         // Check that the B button is pressed
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryHandTrigger ) > 0.5     // Check that the middle finger is pressing
			&& OVRInput.Get( OVRInput.Axis1D.PrimaryIndexTrigger ) > 0.5;   // Check that the index finger is pressing


		// Case of a right hand
		else return
			OVRInput.Get( OVRInput.Button.One )                             // Check that the A button is pressed
			&& OVRInput.Get( OVRInput.Button.Two )                          // Check that the B button is pressed
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryHandTrigger ) > 0.5   // Check that the middle finger is pressing
			&& OVRInput.Get( OVRInput.Axis1D.SecondaryIndexTrigger ) > 0.5; // Check that the index finger is pressing
	}


	// Automatically called at each frame
	void Update () {
		for (int i = 3; i > 0; i--) {
			recentPosition[i] = recentPosition[i - 1];
		}
		recentPosition[0] = this.transform.position;

		buttonBYBehavior();
		if (remoteGrab) {
			remoteGrabUI();
		}
		triggerBehavior();
		if (playerController.isOnBike()) {
			moveWithBike();
		}
		// handle_controller_behavior();
	}

	protected bool triggerPulledPreviousFrame = false;

	protected bool buttonBYPressedPreviousFrame = false;
	protected bool remoteGrab = false;


	[Header("Beamer")]
	public GameObject beamerPrefab;
	[Header("Marker")]
	public GameObject markerPrefab;

	protected GameObject beamer = null;
	protected GameObject marker = null;
	
	protected Vector3 originalPointingDirection;

	protected GameObject pointedObject = null;
	protected GameObject grabbedObject = null;
	protected Type grabbedObjectType;

	protected float maximumRemoteGrabDistance = 5.0f;

	protected bool getTargetPoint(Vector3 position, Vector3 direction, out Vector3 targetPoint, out Collider objectCollider) {
		targetPoint = new Vector3();
		objectCollider = null;
		RaycastHit raycastHit;
		if (!Physics.Raycast(position, direction, out raycastHit, Mathf.Infinity)) return false;
		if (raycastHit.distance > maximumRemoteGrabDistance) return false;
		targetPoint = raycastHit.point;
		objectCollider = raycastHit.collider;
		return true;
	}

	protected void remoteGrabUI() {
		if (grabbedObject != null) return;
		Vector3 pointingDirection = this.transform.rotation * originalPointingDirection;
		if (beamer == null) {
			beamer = GameObject.Instantiate(beamerPrefab, this.transform);
			beamer.transform.position = this.transform.position + pointingDirection * 0.1f;
			beamer.transform.SetParent(this.transform);
		}
		Vector3 targetPoint;
		Collider objectCollider;
		if (getTargetPoint(this.transform.position, pointingDirection, out targetPoint, out objectCollider)) {
			if (marker == null) {
				marker = GameObject.Instantiate(markerPrefab, this.transform);
			}
			marker.transform.position = targetPoint;
			pointedObject = objectCollider.gameObject;
		}
		else {
			if (marker != null) Destroy(marker);
		}
	}

	protected void destroyGrabUI() {
		if (marker != null) Destroy(marker);
		if (beamer != null) Destroy(beamer);
	}

	protected void buttonBYBehavior() {
		bool buttonBYPressed = false;
		if (handType == HandType.LeftHand) buttonBYPressed = (OVRInput.Get(OVRInput.Button.Four));
		else buttonBYPressed = (OVRInput.Get(OVRInput.Button.Two));
		if (buttonBYPressed == buttonBYPressedPreviousFrame) return;
		buttonBYPressedPreviousFrame = buttonBYPressed;

		if (buttonBYPressed) {
			if (playerController.isOnBike()) return;
			if (grabbedObject != null) return;
			remoteGrab = !remoteGrab;
			Debug.LogWarningFormat("remote Grab {0}!", remoteGrab);
			if (!remoteGrab) destroyGrabUI();
		}
	}

	protected bool releaseObject(GameObject grabbedObject) {
		if (grabbedObject == null) return false;
		Vector3 velocity = (recentPosition[0] - recentPosition[3]) * 20.0f;
		if (this.grabbedObjectType == typeof(CountableItem)) {
			this.grabbedObject = null;
			CountableItem countableItem = grabbedObject.GetComponent<CountableItem>();
			countableItem.released(this, velocity);
			grabbedObjectType = null;
			return true;
		}
		if (this.grabbedObjectType == typeof(Container)) {
			this.grabbedObject = null;
			Container container = grabbedObject.GetComponent<Container>();
			container.released(this);
			grabbedObjectType = null;
			return true;
		}
		if (this.grabbedObjectType == typeof(Lid)) {
			this.grabbedObject = null;
			Lid lid = grabbedObject.GetComponent<Lid>();
			lid.released(this, velocity);
			grabbedObjectType = null;
			return true;
		}
		return false;
	}

	protected bool grabObject(GameObject grabbedObject) {
		if (grabbedObject == null) return false;
		Debug.LogWarningFormat("try to grab GameObject name: {0}", grabbedObject.name);
		CountableItem countableItem = grabbedObject.GetComponent<CountableItem>();
		if (countableItem != null) {
			this.grabbedObject = grabbedObject;
			destroyGrabUI();
			countableItem.remoteGrabbed(this);
			grabbedObjectType = typeof(CountableItem);
			return true;
		}
		Container container = grabbedObject.GetComponent<Container>();
		if (container != null) {
			this.grabbedObject = grabbedObject;
			destroyGrabUI();
			container.remoteGrabbed(this);
			grabbedObjectType = typeof(Container);
			return true;
		}
		Lid lid = grabbedObject.GetComponent<Lid>();
		if (lid != null) {
			this.grabbedObject = grabbedObject;
			destroyGrabUI();
			lid.remoteGrabbed(this);
			grabbedObjectType = typeof(Lid);
			return true;
		}
		WallButton wallButton = grabbedObject.GetComponent<WallButton>();
		if (wallButton != null) {
			wallButton.selected();
			return false;
		}
		return false;
	}

	protected void triggerBehavior() {
		bool triggerPulled = false;
		if (handType == HandType.LeftHand) triggerPulled = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5);
		else triggerPulled = (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5);
		if (triggerPulled == triggerPulledPreviousFrame) return;
		triggerPulledPreviousFrame = triggerPulled;

		if (triggerPulled) {
			Debug.LogWarning("trigger pulled!");

			if (playerController.isOnBike()) {
				checkGetOffBike();
			} else {
				if (remoteGrab) {
					if (grabbedObject == null) {
						if (grabObject(pointedObject)) pointedObject = null;
					} else {
						releaseObject(grabbedObject);
						remoteGrab = false;
					}
				} else checkGetOnBike();
			}
		}
	}

	protected void checkGetOnBike() {
		float bikeDistance = Vector3.Distance(this.transform.position, eBike.transform.position);
		Debug.LogWarningFormat("bikeDistance = {0}", bikeDistance);
		if (bikeDistance < 1.5f) {
			remoteGrab = false;
			playerController.rideBike();
		}
	}

	protected void checkGetOffBike() {
		playerController.getOffBike();
	}

	protected void moveWithBike() {
		if (handType != HandType.RightHand) return;
		Vector3 handle = new Vector3(0.0f, 1f, 1f);
		Vector3 handleN = new Vector3(0.0f, -1f, 1f);
		handle.Normalize();
		handleN.Normalize();
		Quaternion rawRotation = this.transform.rotation;
		Vector3 handleDir = rawRotation * handle;
		Vector3 handleNDir = rawRotation * handleN;
		if ((handleDir.y > -0.2) && (handleDir.y < 0.2)) {
			Quaternion ort = playerController.transform.rotation;
			Vector3 ortEuler = ort.eulerAngles;
			ortEuler.z = ortEuler.x = 0f;
			ort = Quaternion.Euler(ortEuler);
			if (handleNDir.y < -0.1f) {
				if (handleNDir.y > -1f + 1e-7) handleNDir.y = (float) (-1f + 1e-7);
				float theta = Mathf.Asin(-handleNDir.y) / Mathf.PI * 2;
				//Debug.LogWarningFormat("HandController: handle {0}, handleNormal {1}, theta {2}", handleDir, handleNDir, theta);
				Vector3 move = ort * (0.1f * theta * Vector3.forward);
				playerController.transform.position += move;
			}
		}
	}

	// Store the previous state of triggers to detect edges
	protected bool is_hand_closed_previous_frame = false;

	// Store the object atached to this hand
	// N.B. This can be extended by using a list to attach several objects at the same time
	protected ObjectAnchor object_grasped = null;

	protected Vector3[] recentPosition;

	/// <summary>
	/// This method handles the linking of object anchors to this hand controller
	/// </summary>
	protected void handle_controller_behavior () {

		// Check if there is a change in the grasping state (i.e. an edge) otherwise do nothing
		bool hand_closed = is_hand_closed();
		if ( hand_closed == is_hand_closed_previous_frame ) return;
		is_hand_closed_previous_frame = hand_closed;



		//==============================================//
		// Define the behavior when the hand get closed //
		//==============================================//
		if ( hand_closed ) {

			// Log hand action detection
			Debug.LogWarningFormat( "{0} get closed", this.transform.parent.name );

			// Determine which object available is the closest from the left hand
			int best_object_id = -1;
			float best_object_distance = float.MaxValue;
			float oject_distance;

			// Iterate over objects to determine if we can interact with it
			for ( int i = 0; i < anchors_in_the_scene.Length; i++ ) {

				// Skip object not available
				if ( !anchors_in_the_scene[i].is_available() ) continue;

				// Compute the distance to the object
				oject_distance = Vector3.Distance( this.transform.position, anchors_in_the_scene[i].transform.position );

				// Keep in memory the closest object
				// N.B. We can extend this selection using priorities
				if ( oject_distance < best_object_distance && oject_distance <= anchors_in_the_scene[i].get_grasping_radius() ) {
					best_object_id = i;
					best_object_distance = oject_distance;
				}
			}

			// If the best object is in range grab it
			if ( best_object_id != -1 ) {

				// Store in memory the object grasped
				object_grasped = anchors_in_the_scene[best_object_id];

				// Log the grasp
				Debug.LogWarningFormat( "{0} grasped {1}", this.transform.parent.name, object_grasped.name );

				// Grab this object
				object_grasped.attach_to( this );
			}



		//==============================================//
		// Define the behavior when the hand get opened //
		//==============================================//
		} else if ( object_grasped != null ) {
			// Log the release
			Debug.LogWarningFormat("{0} released {1}", this.transform.parent.name, object_grasped.name );

			// Release the object
			object_grasped.detach_from( this );
		}
	}
}
