using UnityEngine;
using System.Collections;

public class ScaleCamera : MonoBehaviour 
{
	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
		cam.orthographicSize = (20.0f / Screen.width * Screen.height / 2.0f);
	}
}
