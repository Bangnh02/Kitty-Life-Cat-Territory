public class ImportantQuestCategory : QuestCategory
{
	protected override void OnInit()
	{
	}

	public override Quest SelectQuest()
	{
		return categoryQuests.Find((Quest x) => x.CanStart());
	}
}
