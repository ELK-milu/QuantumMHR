using UnityEngine;

namespace StatePattern.PlayerState
{
	public class BattleState : BasePlayerState
	{
		public BattleState(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			throw new System.NotImplementedException();
		}

		public override void Update()
		{
			throw new System.NotImplementedException();
		}

		public override void FixedUpdate()
		{
			throw new System.NotImplementedException();
		}

		public override void LaterUpdate()
		{
			throw new System.NotImplementedException();
		}

		public override void OnExit()
		{
			throw new System.NotImplementedException();
		}
	}
	public class G_sword_skill_02_move2r_State : BattleState
	{
		public G_sword_skill_02_move2r_State (PlayerHandler playerController, Animator animator) : base(playerController, animator)
		{
		}

		private bool isAnimate = false;
		public override void OnEnter()
		{
			Debug.Log("Enter G_sword_skill_02_move2r_State");
			_playerController.SetMaster(false);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.IsSkill02,
				"Skill",
				true);
			_animator.Play(G_sword_skill_02_move2r_Hash);
		}
		public override void OnExit()
		{
			_playerController.SetMaster(true);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.IsSkill02,
				"Skill",
				false);
		}
	}
}
