using UnityEngine;
using System.Collections;

public class AvatarSpiner : MonoBehaviour
{
	private Transform t;
	private Vector3 r = new Vector3(0.0f,0.0f,0.2f);

	void Start () {
		t = transform;
	}
	
	// Update is called once per frame
	void Update () {
		t.Rotate(r);
	}
}
