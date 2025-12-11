public abstract class CommandBase
{
	public virtual CommandId CommandId
	{
		get;
	}

	public virtual CommandId CancelMask => CommandId.None;

	public bool IsExecuting
	{
		get;
		protected set;
	}

	public virtual bool CanForceRun => false;

	public abstract void Execute();

	public virtual void Cancel()
	{
		IsExecuting = false;
	}

	protected virtual void CompleteExecution()
	{
		IsExecuting = false;
	}

	public virtual float GetExecutedTimePart()
	{
		return 0f;
	}
}
