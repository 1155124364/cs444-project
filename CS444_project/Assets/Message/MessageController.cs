using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour
{

    [Header("Message Prefab")]
    public GameObject messagePrefab;
    [Header("Center Eye")]
    public GameObject centerEye;
    public GameObject newMessage;

    public void destroyNewMessage() {
        Destroy(newMessage);
    }

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
