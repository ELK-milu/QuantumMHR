using Photon.Deterministic;
using StatePattern.StateSystem;
using System.Collections.Generic;
using UnityEngine;

namespace StatePattern.PlayerState
{
	public abstract class BasePlayerState : IState
	{
		protected readonly PlayerHandler _playerController;
		protected readonly Animator _animator;

		#region Field Hash
		protected static readonly int field_Idle_Hash = Animator.StringToHash("field_idle_loop");
		protected static readonly int field_Locomotion_Hash = Animator.StringToHash("Locomotion");
		protected static readonly int field_stop_run_Hash = Animator.StringToHash("field_stop_run");
		protected static readonly int field_stop_run_fast_Hash = Animator.StringToHash("field_stop_run_fast");
		protected static readonly int field_start_exhaust_Hash = Animator.StringToHash("field_start_exhaust");
		protected static readonly int field_stop_exhaust_Hash = Animator.StringToHash("field_stop_exhaust");
		protected static readonly int field_start_run2scroll_Hash = Animator.StringToHash("field_start_run2scroll");
		protected static readonly int field_start_scroll_Hash = Animator.StringToHash("field_start_scroll");
		protected static readonly int field_start_scroll2locomotion_Hash = Animator.StringToHash("field_start_scroll2locomotion_Hash");
		protected static readonly int field_wire_start_Hash = Animator.StringToHash("field_wire_start");
		protected static readonly int field_wire_move_Hash = Animator.StringToHash("field_wire_move");
		protected static readonly int field_wire_start_up_Hash = Animator.StringToHash("field_wire_start_up");
		protected static readonly int field_wire_start_rising_Hash = Animator.StringToHash("field_wire_start_rising");
		protected static readonly int field_wire_jump_Hash = Animator.StringToHash("field_wire_jump");

		// fall
		protected static readonly int field_falling_scroll_Hash = Animator.StringToHash("field_falling_scroll");





		#endregion

		#region Battle hash
		protected static readonly int G_sword_skill_02_move2r_Hash = Animator.StringToHash("G_sword_skill_02_move2r");
		#endregion

		protected const float CROSS_FADE_DURATION = 0.1f;
		protected const float CROSS_FADE_DURATION_005 = 0.05f;


		/// <summary>
		/// 存储了动画的哈希值和过渡时长的字典
		/// </summary>
		/// <param name="int">动画的哈希值</param>
		/// <param name="float">动画的过渡时长</param>
		protected static Dictionary<int, float> animHashDictionary = new Dictionary<int, float>();

		public BasePlayerState(PlayerHandler playerController, Animator animator)
		{
			_playerController = playerController;
			_animator = animator;
			_playerController.PlayerStateMachine._stateMachine.GetOrAddNode(this);
		}

		public abstract void OnEnter();

		public abstract void Update();

		public abstract void FixedUpdate();
		public abstract void LaterUpdate();


		public abstract void OnExit();
	}

}