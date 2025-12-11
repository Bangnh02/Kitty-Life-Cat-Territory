using UnityEngine;
using UnityEngine.UI;

public class EnemyStaminaPanel : MonoBehaviour
{
	[SerializeField]
	private Image staminaBar;

	private EnemyController enemyController;

	public void Setup(EnemyController enemyController)
	{
		this.enemyController = enemyController;
		enemyController.staminaChangeEvent += OnStaminaChange;
		EnemyController.changeStateEvent += OnChangeState;
		OnStaminaChange();
		OnChangeState(enemyController, enemyController.CurState);
	}

	private void OnChangeState(EnemyController enemyController, EnemyController.State newState)
	{
		if (!(enemyController != this.enemyController))
		{
			if (newState == EnemyController.State.Escape)
			{
				staminaBar.enabled = true;
			}
			else
			{
				staminaBar.enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		EnemyController.changeStateEvent -= OnChangeState;
		if (enemyController != null)
		{
			enemyController.staminaChangeEvent -= OnStaminaChange;
		}
	}

	private void OnStaminaChange()
	{
		staminaBar.fillAmount = enemyController.CurStamina / enemyController.MaxStamina;
	}
}
