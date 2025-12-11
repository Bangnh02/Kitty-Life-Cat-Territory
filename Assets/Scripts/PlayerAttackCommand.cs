using Avelog;
using System.Collections;
using UnityEngine;

public class PlayerAttackCommand : CommandBase
{
	private Coroutine rotateCoroutine;

	public override CommandId CommandId => CommandId.Attack;

	public override CommandId CancelMask => CommandId.Eat | CommandId.Drink;

	private PlayerMovement PlayerMovement => PlayerSpawner.PlayerInstance.PlayerMovement;

	private PlayerCombat PlayerCombat => PlayerSpawner.PlayerInstance.PlayerCombat;

	public override void Execute()
	{
		if (!PlayerMovement.IsFalling && PlayerCombat.IsAttackTime())
		{
			PlayerCombat.AttackBox(CancelRotate);
			if (rotateCoroutine != null)
			{
				PlayerMovement.StopCoroutine(rotateCoroutine);
			}
			if (PlayerCombat.LastAttackedTarget != null)
			{
				rotateCoroutine = PlayerMovement.StartCoroutine(Rotate(PlayerCombat.LastAttackedTarget));
			}
		}
	}

	private void CancelRotate()
	{
		if (rotateCoroutine != null)
		{
			PlayerMovement.StopCoroutine(rotateCoroutine);
		}
		rotateCoroutine = null;
		CompleteExecution();
	}

	private IEnumerator Rotate(ActorCombat target)
	{
		Vector3 vector = target.transform.position - PlayerMovement.transform.position;
		if (Vector3.Angle(vector, PlayerMovement.transform.forward) <= ManagerBase<PlayerManager>.Instance.MinAngleTurnByAttack)
		{
			CompleteExecution();
			yield break;
		}
		Vector3 endForward = Vector3.ProjectOnPlane(vector, Vector3.up);
		while (true)
		{
			float rotAngle = ManagerBase<PlayerManager>.Instance.RotationSpeed * Time.deltaTime;
			Quaternion rotation = QuaternionUtils.GetRotation(new Vector3(PlayerMovement.transform.forward.x, 0f, PlayerMovement.transform.forward.z), endForward, rotAngle);
			PlayerMovement.transform.rotation *= rotation;
			if (rotation == Quaternion.identity)
			{
				break;
			}
			yield return null;
		}
		CompleteExecution();
	}
}
