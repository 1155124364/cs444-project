/*
	HandController.cs
	Description: Control the behavior of the hand controller, mainly the interaction part.
*/

using UnityEngine;
using System;
using System.Collections.Generic;

public class HandController : MonoBehaviour {

	// Public members to store references to required objects
	public enum HandType : int { LeftHand, RightHand };
	[Header( "Hand Properties" )]
	public HandType handType;
	[Header( "Player Controller" )]
	public MainPlayerController playerController;
	[Header("Beamer")]
	public GameObject beamerPrefab;
	[Header("Marker")]
	public GameObject markerPrefab;

	// Protected members to store references to required objects
	protected EBike eBike;
	protected OrderController orderController = null;
	protected GameObject beamer = null;
	protected GameObject marker = null;
	protected GameObject pointedObject = null;
	protected GameObject grabbedObject = null;
	
	// Inner status variables.
	// Variables to store whether some buttons are pressed in the previous frame, so that to detect change in the button status.
	protected bool triggerPulledPreviousFrame = false;
	protected bool buttonBYPressedPreviousFrame = false;
	protected bool buttonAPressedPreviousFrame = false;
	protected bool buttonXPressedPreviousFrame = false;

	// Variables used for remote grab.
	protected bool remoteGrab = false;
	protected float maximumRemoteGrabDistance = 5.0f;
	protected Vector3 originalPointingDirection;
	protected Type grabbedObjectType;

	// Store the recent position of the controller, to calculate the velocity when release an object from the controller.
	protected Vector3[] recentPosition;

	// Variables used when riding the bike.
	// hasOrt: whether the rotation of the bike is decided.
	protected bool hasOrt = false;
	protected Quaternion ort;

	/*
	Methods for remote grab:
		getTargetPoint
		remoteGrabUI
		destroyGrabUI
		releaseObject
		grabObject
	*/

	// Method for getting the target point of remote grab.
	// Returns whether a target point is within the maximum grabbing distance.
	protected bool getTargetPoint(Vector3 position, Vector3 direction, out Vector3 targetPoint, out Collider objectCollider) {
		// Initialization.
		targetPoint = new Vector3();
		objectCollider = null;

		// Cast a ray. Return false when the raycast failed, or the target point is beyond the maximum distance.
		RaycastHit raycastHit;
		if (!Physics.Raycast(position, direction, out raycastHit, Mathf.Infinity)) return false;
		if (raycastHit.distance > maximumRemoteGrabDistance) return false;

		// Set the returned variables.
		targetPoint = raycastHit.point;
		objectCollider = raycastHit.collider;
		return true;
	}

	// Method to show the remote grab beamer and marker.
	protected void remoteGrabUI() {
		// If already have something grabbed, the beamer and marker should not be shown.
		if (grabbedObject != null) return;

		// Compute the pointing direction of the beamer.
		Vector3 pointingDirection = this.transform.rotation * originalPointingDirection;

		// Generate a beamer instance if there's none.
		if (beamer == null) {
			beamer = GameObject.Instantiate(beamerPrefab, this.transform);
			beamer.transform.position = this.transform.position + pointingDirection * 0.15f;
			beamer.transform.SetParent(this.transform);
		}

		// Get the target point of the pointing, and set the marker on the target point.
		Vector3 targetPoint;
		Collider objectCollider;
		if (getTargetPoint(this.transform.position, pointingDirection, out targetPoint, out objectCollider)) {
			// If successfully pointed at something, show the marker.
			if (marker == null) {
				marker = GameObject.Instantiate(markerPrefab, this.transform);
			}
			marker.transform.position = targetPoint;
			pointedObject = objectCollider.gameObject;
		}
		else {
			// Otherwise, the marker should be destroyed.
			if (marker != null) Destroy(marker);
		}
	}

	// Method for destroy the beamer and marker.
	protected void destroyGrabUI() {
		if (marker != null) Destroy(marker);
		if (beamer != null) Destroy(beamer);
	}

	// Method to release the grabbed object.
	// Return whether the release is successful.
	protected bool releaseObject(GameObject grabbedObject) {
		// If none object is grabbed, return directly
		if (grabbedObject == null) return false;

		// Compute the velocity when release (which will be given to the released object, so that to implement the throwing behavior)
		Vector3 velocity = (recentPosition[0] - recentPosition[3]) * 20.0f;

		// Deal with different object type.
		// This should have been done with inheritance and double dispatch pattern, but it was too late when I've found this situation. So I decide not to reconstruct ;)
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
			container.released(this, velocity);
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

	// Method to grab an object.
	// This should be called after the pointed object is decided.
	protected bool grabObject(GameObject grabbedObject) {
		// If the caller want to grab a null, it should directly return false.
		if (grabbedObject == null) return false;

		// Deal with different object type. Same situation as in release method.
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

		// Deal with pressing the button.
		// Pressing button behavior is not a grabbing behavior, so it should only call the method of button, but should not change the inner status of grabbing.
		WallButton wallButton = grabbedObject.GetComponent<WallButton>();
		if (wallButton != null) {
			wallButton.selected();
			return false;
		}
		return false;
	}

	/*
	Methods for button behaviors:
		buttonBYBehavior
		buttonABehavior
		buttonXBehavior
		triggerBehavior
	*/

	// Method for behavior of button B and button Y
	// Button B and button Y are used to turn on/off the selection mode (which is the mode for grabbing, button pressing and cutting)
	protected void buttonBYBehavior() {

		bool buttonBYPressed = false;
		// Different button detection for different hands.
		if (handType == HandType.LeftHand) buttonBYPressed = (OVRInput.Get(OVRInput.Button.Four));
		else buttonBYPressed = (OVRInput.Get(OVRInput.Button.Two));
		// Only when the press status change from not pressed to pressed, will some actions been taken.
		if (buttonBYPressed == buttonBYPressedPreviousFrame) return;
		buttonBYPressedPreviousFrame = buttonBYPressed;

		if (buttonBYPressed) {
			// Action of button B and button Y: turn on/off the selection mode.
			// i.e. modify the remoteGrab variable.
			// Note that if the player is on the bike, the player may not enter the selection mode.
			if (playerController.isOnBike()) return;
			if (grabbedObject != null) return;
			remoteGrab = !remoteGrab;
			if (!remoteGrab) destroyGrabUI();
		}
	}

	// Method for behavior of button A (right hand)
	// When button A is pressed, pop a status message.
	protected void buttonABehavior() {
		bool buttonAPressed = false;
		if (handType == HandType.LeftHand) return;
		else buttonAPressed = (OVRInput.Get(OVRInput.Button.One));
		if (buttonAPressed == buttonAPressedPreviousFrame) return;
		buttonAPressedPreviousFrame = buttonAPressed;

		if (buttonAPressed) {
			orderController.popStatus();
		}
	}

	// Method for behavior of button X (left hand)
	// When button X is pressed, close the pop up message.
	protected void buttonXBehavior() {
		bool buttonXPressed = false;
		if (handType == HandType.LeftHand) buttonXPressed = (OVRInput.Get(OVRInput.Button.Three));
		else return;
		if (buttonXPressed == buttonXPressedPreviousFrame) return;
		buttonXPressedPreviousFrame = buttonXPressed;

		if (buttonXPressed) {
			orderController.closeMessage();
		}
	}

	// Method for behavior of index trigger (regardless of which hand)
	// Multiple situations is dealt differently when the trigger is pulled.
	protected void triggerBehavior() {

		// Take action only when the trigger status changes from not pulled to pulled.
		bool triggerPulled = false;
		if (handType == HandType.LeftHand) triggerPulled = (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5);
		else triggerPulled = (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5);
		if (triggerPulled == triggerPulledPreviousFrame) return;
		triggerPulledPreviousFrame = triggerPulled;

		if (triggerPulled) {
			if (playerController.isOnBike()) {
				// If the player is on the bike, the player may get off the bike when pulling the trigger.
				checkGetOffBike();
			} else {
				// Otherwise, the player is not on the bike.
				if (remoteGrab) {
					if (grabbedObject == null) {
						// If remoteGrabe == true, and no object has been grabbed, the player is in selection mode.
						// In this situation, use grabObject method to try grabbing the pointed object.
						// If successfully grabbed an object, the pointed object should set to null (exit the selection mode).
						if (grabObject(pointedObject)) pointedObject = null;
					} else {
						// Otherwise, the player has already grabbed something, pulling the trigger will release the grabbed object.
						releaseObject(grabbedObject);
						remoteGrab = false;
					}
					// If none of the situation above, i.e. empty hand, no selection mode, try to get on the bike.
				} else checkGetOnBike();
			}
		}
	}

	/*
	Methods for riding bike:
		checkGetOnBike
		checkGetOffBike
		moveWithBike
	*/

	// Method to try getting on the bike.
	protected void checkGetOnBike() {
		// Compute the distance to the bike.
		float bikeDistance = Vector3.Distance(this.transform.position, eBike.transform.position);

		// If within the distance, get on the bike.
		if (bikeDistance < 1.5f) {
			remoteGrab = false;
			playerController.rideBike();
		}
	}

	// Method to try getting off the bike.
	protected void checkGetOffBike() {
		playerController.getOffBike();
	}

	// Method for riding the bike.
	// When the player hold the right controller horizontally, and rotate it, the bike will move.
	// To reduce motion sickness, when the bike is moving, the player may not change the forward direction.
	// When the bike is stopped, the player may change the forward direction by turning around body.
	protected void moveWithBike() {
		// Only right handle of a bike have throttle.
		if (handType != HandType.RightHand) return;

		// Initial direction of handlebar.
		Vector3 handle = new Vector3(0.0f, 1f, 1f);
		Vector3 handleN = new Vector3(0.0f, -1f, 1f);
		handle.Normalize();
		handleN.Normalize();
		Quaternion rawRotation = this.transform.rotation;

		// Direction of handlebar after rotation.
		Vector3 handleDir = rawRotation * handle;
		Vector3 handleNDir = rawRotation * handleN;

		// Take action only when the handlebar (controller) is hold horizontally.
		if ((handleDir.y > -0.2) && (handleDir.y < 0.2)) {

			// Get the forward direction of player
			// The forward direction is equal to the eye direction.
			Quaternion ortTry = playerController.transform.rotation;
			Vector3 ortEuler = ortTry.eulerAngles;
			
			// The bike should only rotate alone Y-axis.
			ortEuler.z = ortEuler.x = 0f;
			
			// The forward direction should only change when the bike is not moving.
			if (!hasOrt) {
				ort = Quaternion.Euler(ortEuler);
				hasOrt = true;
				eBike.setRotation(ort);
			}

			// The moving speed should be decided according to the rotation of handlebar. If rotated with larger angle, the velocity should also be greater.
			if (handleNDir.y < -0.1f) {
				if (handleNDir.y > -1f + 1e-7) handleNDir.y = (float) (-1f + 1e-7);
				float theta = Mathf.Asin(-handleNDir.y) / Mathf.PI * 2;
				Vector3 move = ort * (0.1f * theta * Vector3.forward);
				playerController.transform.position += move;
			} else {
				// If the velocity is 0, the player may change the forward direction.
				hasOrt = false;
			}
		} else {
			// If the handlebar direction is incorrect, which means the velocity is 0, the player may change the forward direction.
			hasOrt = false;
		}
	}

	void Start () {
		// Get the needed object references when start.
		if (playerController == null) playerController = GameObject.FindObjectOfType<MainPlayerController>();
		if (eBike == null) eBike = GameObject.FindObjectOfType<EBike>();
		if (orderController == null) orderController = GameObject.FindObjectOfType<OrderController>();

		// Initialize some inner status variables.
		originalPointingDirection = new Vector3(0f, 1f, 1f);
		originalPointingDirection.Normalize();
		recentPosition = new Vector3[4]{this.transform.position, this.transform.position, this.transform.position, this.transform.position};
	}

	// Automatically called at each frame
	void Update () {
		// Update the recent position list.
		for (int i = 3; i > 0; i--) {
			recentPosition[i] = recentPosition[i - 1];
		}
		recentPosition[0] = this.transform.position;

		// Deal with different button behaviors.
		buttonBYBehavior();
		buttonABehavior();
		buttonXBehavior();

		// If in selection mode, show the beamer and marker by remoteGrabUI method.
		if (remoteGrab) {
			remoteGrabUI();
		}

		// Deal with trigger behavior. This should after the remote grab UI shown.
		triggerBehavior();

		// If the player is on the bike, deal with bike riding behaviors.
		if (playerController.isOnBike()) {
			moveWithBike();
		} else {
			hasOrt = false;
		}
	}

}
