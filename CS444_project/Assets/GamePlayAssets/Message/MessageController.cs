/*
    MessageController.cs
    Description: Control the pop-up message in front of the player.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour {

    // Store the reference to the required GameObjects.
    [Header("Message Prefab")]
    public GameObject messagePrefab;
    [Header("Center Eye")]
    public GameObject centerEye;
    public GameObject newMessage;

    // Destroy existing message.
    public void destroyNewMessage() {
        Destroy(newMessage);
    }

    // Pop a new message, and destroy it after 5 seconds.
    public void popMessage(string message) {
        if (newMessage != null) {
            Destroy(newMessage);
        }
        newMessage = GameObject.Instantiate(messagePrefab, centerEye.transform);
        newMessage.transform.position = centerEye.transform.position + centerEye.transform.rotation * Vector3.forward * 2.5f;
        newMessage.transform.SetParent(centerEye.transform);
        newMessage.GetComponent<TMP_Text>().text = message;
        Invoke("destroyNewMessage", 5);
    }

}