using UnityEngine;

public class EnemyProcessingSwitch : ProcessingSwitch
{
	private enum MoveModeState
	{
		None,
		NavAgentMode,
		ManualMode
	}

	public delegate void SwitchMoveModeHandler(bool isNavAgentMode);

	[SerializeField]
	[Header("Дополнительные параметры для ботов")]
	private EnemyModel enemyModel;

	public float distanceToSwitchMoveMode;

	private MoveModeState curState;

	private float updateTimer;

	private EnemyController EnemyController => enemyModel.EnemyController;

	public bool OnNavAgentMoveModeDistance
	{
		get;
		private set;
	}

	public event SwitchMoveModeHandler switchMoveModeEvent;

	public void CheckSwitchMoveMode()
	{
		OnNavAgentMoveModeDistance = (base.SqrDistanceToTarget < Mathf.Pow(distanceToSwitchMoveMode, 2f));
		if (OnNavAgentMoveModeDistance)
		{
			if (NeedSwitch(MoveModeState.NavAgentMode))
			{
				curState = MoveModeState.NavAgentMode;
				this.switchMoveModeEvent?.Invoke(isNavAgentMode: true);
			}
		}
		else if (NeedSwitch(MoveModeState.ManualMode))
		{
			curState = MoveModeState.ManualMode;
			this.switchMoveModeEvent?.Invoke(isNavAgentMode: false);
		}
	}

	private bool NeedSwitch(MoveModeState newState)
	{
		return newState != curState;
	}

	public override void UpdateByTime()
	{
		base.UpdateByTime();
		CheckSwitchMoveMode();
	}

	protected override bool NeedUpdateProcessingStateByTimer()
	{
		return EnemyController.CurState == EnemyController.State.Patrol;
	}
}
