using UnityEngine;
using System.Collections;

public class PlayerAvatar : MonoBehaviour 
{ 
	private bool gameActive = true;

	private CharacterController motor;
	private float speed = 10.0f;
	private Vector3 move = Vector3.zero;
	private bool dir = true;

	private void Start()
	{
		motor = GetComponent<CharacterController>();
		SwapDirection();
	}

	private void Update	()
	{
		if(!gameActive)
			return;

		if(Input.GetMouseButtonDown(0))
			SwapDirection();

		motor.Move(move * Time.deltaTime);
		if(transform.position.z > 0.5f || transform.position.z < -0.5f)
			transform.position = new Vector3(transform.position.x,transform.position.y,0);
	}

	private void SwapDirection()
	{
		dir = !dir;
		move = new Vector3((dir)?-speed:speed,0,0);	
	}

	public void OnLevelUp()
	{
		speed+=0.25f;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		switch(hit.gameObject.tag)
		{
		case "Wall":
			gameActive = false;
			GameManager.instance.GameEnded();
			move = Vector3.zero;
			break;
		case "Point":
			hit.gameObject.SetActive(false);
			GameManager.instance.OnCollectPoint();
			break;
		default:
			break;
		}
	}
}
