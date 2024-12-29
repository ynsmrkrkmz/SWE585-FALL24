using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform player;
	public float smoothing=5f;
	Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = transform.position - player.position;
	
	}
	

	void FixedUpdate () {
		Vector3 newCamPos = player.position + offset;
		transform.position = Vector3.Lerp (transform.position, newCamPos, smoothing * Time.deltaTime);
	
	}
}
