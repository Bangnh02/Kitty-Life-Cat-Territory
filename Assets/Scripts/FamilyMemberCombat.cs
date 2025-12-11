using System.Collections.Generic;
using UnityEngine;

public class FamilyMemberCombat : ActorCombat
{
	[SerializeField]
	private List<AttackTriggerInfo> kittenAttackTriggers;

	private FamilyManager.FamilyMemberData familyMember;

	private FamilyMemberController familyMemberController;

	protected override float AttackPower => familyMember.CurHitPower * ManagerBase<FamilyManager>.Instance.HitFrequency;

	protected override float AttackFrequency => ManagerBase<FamilyManager>.Instance.HitFrequency;

	protected override List<AttackTriggerInfo> AttackTriggers
	{
		get
		{
			if (familyMember.role == FamilyManager.FamilyMemberRole.FirstStageChild || familyMember.role == FamilyManager.FamilyMemberRole.SecondStageChild)
			{
				return kittenAttackTriggers;
			}
			return base.AttackTriggers;
		}
	}

	private void Start()
	{
		familyMemberController = GetComponent<FamilyMemberController>();
		familyMember = familyMemberController.familyMemberData;
	}

	protected override Animator GetAnimator()
	{
		return familyMemberController.Model.Animator;
	}
}
