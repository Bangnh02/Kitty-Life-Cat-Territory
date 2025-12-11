using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class QuestInfoChildRole : QuestInfoItem
{
	private Image childRoleImage;

	private bool isCurRole = true;

	private FamilyManager.FamilyMemberRole Role
	{
		get
		{
			if (!isCurRole)
			{
				return ManagerBase<FamilyManager>.Instance.GrowingChildNextStage;
			}
			return ManagerBase<FamilyManager>.Instance.GrowingChild.role;
		}
	}

	private FamilyManager.GenderType Gender => ManagerBase<FamilyManager>.Instance.GrowingChild.genderType;

	protected override void Initialize()
	{
		childRoleImage = GetComponent<Image>();
		List<QuestInfoChildRole> list = base.transform.parent.GetComponentsInChildren<QuestInfoChildRole>().ToList();
		list.Sort((QuestInfoChildRole x, QuestInfoChildRole y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
		isCurRole = (list.IndexOf(this) == 0);
	}

	protected override void OnQuestStart(Quest quest)
	{
		SetupRoleSprite();
	}

	protected override void OnQuestUpdateProgress(Quest quest)
	{
		SetupRoleSprite();
	}

	private void SetupRoleSprite()
	{
		if (ManagerBase<FamilyManager>.Instance.GrowingChild != null)
		{
			Sprite roleSprite = ManagerBase<UIManager>.Instance.GetRoleSprite(Role, Gender);
			childRoleImage.sprite = roleSprite;
		}
	}
}
