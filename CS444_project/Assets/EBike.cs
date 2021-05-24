using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBike : MonoBehaviour
{

    protected MainPlayerController mainPlayerController = null;
    protected Collider[] subColliders;

    public void rideOn(MainPlayerController mainPlayerController) {
        this.mainPlayerController = mainPlayerController;
        //GetComponent<Collider>().enabled = false;
        //Debug.LogWarningFormat("{0} collider(s) found!", subColliders.Length);

        if ((subColliders == null) || (subColliders.Length == 0)) {
            subColliders = gameObject.GetComponentsInChildren<Collider>();
        }

        for (int i = 0; i < subColliders.Length; i++) {
            subColliders[i].enabled = false;
        }
        Vector3 mainPosition = mainPlayerController.transform.position;
        mainPosition.y = 0.0f;
        mainPlayerController.myMoveTo(mainPosition);
    }

    public void getOff(MainPlayerController mainPlayerController) {
        if (this.mainPlayerController != mainPlayerController) return;
        this.mainPlayerController = null;
        //this.transform.GetComponent<Collider>().enabled = true;
        for (int i = 0; i < subColliders.Length; i++) {
            subColliders[i].enabled = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        subColliders = gameObject.GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogWarningFormat("{0} collider(s) found!", subColliders.Length);
        //Debug.LogWarningFormat("bike Position: {0}", this.transform.position);
        if (mainPlayerController != null) {
            Vector3 mainPosition = mainPlayerController.transform.position;
            Vector3 mainRotation = mainPlayerController.transform.rotation.eulerAngles;
            mainRotation.x = 0;
            mainRotation.z = 0;
            mainPosition.y = 0;
            mainPlayerController.myMoveTo(mainPosition);
            this.transform.rotation = Quaternion.Euler(mainRotation);
            this.transform.position = mainPosition;
        }
    }
}
