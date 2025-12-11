using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderEvents : MonoBehaviour
{
	public delegate void CollisionHandler(Collision collision);

	private Collider collider;

	public event CollisionHandler collisionEnterEvent;

	public event CollisionHandler collisionStayEvent;

	public event CollisionHandler collisionExitEvent;

	private void Start()
	{
		collider = GetComponent<Collider>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.collisionEnterEvent?.Invoke(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		this.collisionStayEvent?.Invoke(collision);
	}

	private void OnCollisionExit(Collision collision)
	{
		this.collisionExitEvent?.Invoke(collision);
	}
}
