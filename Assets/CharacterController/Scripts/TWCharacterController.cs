using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWCharacterController : MonoBehaviour
{
	[SerializeField]
#if UNITY_EDITOR
	[Help("KeyBoard \"V\" to toggle between first and third person view\nWASD to move and mouse to look", UnityEditor.MessageType.Info)]
#endif
	[Tooltip("Doesn't do anything. Just comments shown in inspector")]
	private int noFunction;

	[Space(10)]
	public float speed = 5.0f;              // 5f
	public float jumpMultiplier = 100f;		// 100f
	public float lookSensitivity = 3.0f;	// 3.0f
	public bool invertY = false;            // false

	private Camera thisCamera;
	private Rigidbody thisRigidbody;
	private float inputX;
	private float inputY;
	private Vector3 moveHorizontal;
	private Vector3 moveVertical;
	private Vector3 vel;
	private float rotY;
	private float rotX;
	private Vector3 moveRotation;
	private Vector3 cameraRotation;
	private bool isGrounded;

	private bool toggleView = false;
	private Vector3 firstPersonPos = new Vector3(0f, 1.5f, 0f);
	private Vector3 thirdPersonPos = new Vector3(0f, 2f, -4f);

	#region mono
	private void Start()
	{
		thisRigidbody = GetComponent<Rigidbody>();
		thisCamera = GetComponentInChildren<Camera>();
		thisCamera.transform.localPosition = firstPersonPos;
	}

	private void Update()
	{
		ToggleView();

		// keyboard shortcut
		if (Input.GetKeyUp(KeyCode.V))
			toggleView = !toggleView;
	}

	private void FixedUpdate()
	{
		CharacterControl();
		JumpControl();
	}
	#endregion

	#region move
	private void CharacterControl()
	{
		// input
		inputX = Input.GetAxis("Horizontal");
		inputY = Input.GetAxis("Vertical");
		rotY = Input.GetAxisRaw("Mouse X");
		rotX = Input.GetAxisRaw("Mouse Y");

		if (invertY) rotX *= -1;

		moveHorizontal = transform.right * inputX;
		moveVertical = transform.forward * inputY;
		vel = (moveHorizontal + moveVertical).normalized * speed;

		moveRotation = new Vector3(0, rotY, 0) * lookSensitivity;
		cameraRotation = new Vector3(rotX, 0, 0) * lookSensitivity;

		if (vel != Vector3.zero)
			thisRigidbody.MovePosition(thisRigidbody.position + vel * Time.fixedDeltaTime);

		if (moveRotation != Vector3.zero)
			thisRigidbody.MoveRotation(thisRigidbody.rotation * Quaternion.Euler(moveRotation));

		if (thisCamera != null)
		{
			//negate this value so it rotates like a FPS not like a plane
			thisCamera.transform.Rotate(-cameraRotation);
		}
	}

	public void JumpControl()
	{
		if (Input.GetButton("Jump") && isGrounded)
		{
			isGrounded = false;
			thisRigidbody.AddForce(Vector3.up * jumpMultiplier, ForceMode.Impulse);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		isGrounded = true;
	}
	#endregion

	private void ToggleView()
	{
		if (!toggleView)
		{
			if (thisCamera.transform.localPosition != firstPersonPos)
				thisCamera.transform.localPosition = firstPersonPos;
		}
		else
		{
			if (thisCamera.transform.localPosition != thirdPersonPos)
				thisCamera.transform.localPosition = thirdPersonPos;
		}
	}
}
