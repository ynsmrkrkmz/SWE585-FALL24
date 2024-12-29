using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour {
    private const float Y_ANGLE_MIN = -30f;
    private const float Y_ANGLE_MAX = 50f;

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private float distance = 8f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float sensivityX = 4f;
    private float sensivityY = 1f;


	// Use this for initialization
	private void Start () {
        camTransform = transform;
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");
        currentY= Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
	}

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        if (Input.GetButton("Fire2"))
        {
            lookAt.localRotation = camTransform.rotation;
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, new Vector3(2.5f, 4f, -5f), 5f * Time.deltaTime);
        }
        else
        {
            
            camTransform.position = lookAt.position + new Vector3(0, 5, 0) + rotation * dir;
            camTransform.LookAt(lookAt.position + new Vector3(0, 4, 0));
        }
        
    }
}
