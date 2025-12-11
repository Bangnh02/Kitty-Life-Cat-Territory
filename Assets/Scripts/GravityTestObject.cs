using UnityEngine;

public class GravityTestObject : MonoBehaviour
{
	[SerializeField]
	private float jumpSpeed = 65f;

	[SerializeField]
	private float gravity = -10f;

	[SerializeField]
	private bool testCorrection = true;

	[SerializeField]
	private bool isJumping;

	[SerializeField]
	private float vertSpeed;

	[SerializeField]
	private CharacterController characterController;

	private float startJumpTime;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && !isJumping)
		{
			isJumping = true;
			startJumpTime = Time.time;
			vertSpeed = jumpSpeed;
			return;
		}
		float num = gravity * Time.deltaTime;
		if (characterController.isGrounded)
		{
			vertSpeed = Mathf.Clamp(vertSpeed, -10f, float.MaxValue);
		}
		float y = characterController.transform.position.y;
		if (testCorrection)
		{
			vertSpeed += num / 2f;
			Vector3 a = Vector3.up * vertSpeed;
			characterController.Move(a * Time.deltaTime);
			vertSpeed += num / 2f;
		}
		else
		{
			vertSpeed += num;
			Vector3 a2 = Vector3.up * vertSpeed;
			characterController.Move(a2 * Time.deltaTime);
		}
		float num2 = characterController.transform.position.y - y;
		if (isJumping && characterController.isGrounded)
		{
			isJumping = false;
			float num3 = (vertSpeed - num / 2f) * Time.deltaTime;
			float num4 = (1f - num2 / num3) * Time.deltaTime;
			float num5 = Time.time - startJumpTime - num4;
			UnityEngine.Debug.Log($"Jump time = {num5}");
		}
	}
}
