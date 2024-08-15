/*/
* Script by Devin Curry
* http://Devination.com
* https://youtube.com/user/curryboy001
* Please like and subscribe if you found my tutorials helpful :D
* Twitter: https://twitter.com/Devination3D
/*/
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FloatingPlayer2DController : MonoBehaviour
{
	public float moveForce = 5, boostMultiplier = 2;
	Rigidbody2D myBody;

	void Start ()
	{
		myBody = this.GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate ()
	{
		Vector2 moveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),
			CrossPlatformInputManager.GetAxis("Vertical"))
			* moveForce;
		Vector3 lookVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal_2"),
			CrossPlatformInputManager.GetAxis("Vertical_2"), 4096);

		if (lookVec.x != 0 && lookVec.y != 0)
			transform.rotation = Quaternion.LookRotation(lookVec, Vector3.back);
		
		bool isBoosting = CrossPlatformInputManager.GetButton("Boost");
		myBody.AddForce(moveVec * (isBoosting ? boostMultiplier : 1));
	}
}