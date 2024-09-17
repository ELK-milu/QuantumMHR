using ExitGames.Client.Photon.StructWrapping;
using Photon.Deterministic;
using Quantum;
using System;
using UnityEngine;

namespace StatePattern.PlayerState
{
	// 用于标识可直接切换为铁虫丝移动的接口
	public interface IWireable
	{
		
	}
	
	public class FieldState : BasePlayerState
	{
		public FieldState (PlayerHandler playerController, Animator animator) : base(playerController, animator)
		{
		}
		public override void OnEnter()
		{

		}

		public override void Update()
		{
		}

		public override void FixedUpdate()
		{
		}

		public override void OnExit()
		{
		}
	}
	public class Field_Idle_State : FieldState,IWireable
	{
		public override void OnEnter()
		{
			_playerController.SetMaster(true);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Idle_State");
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Filed_Idle,
				"Is_Filed_Idle",
				true);
			_animator.CrossFade(field_Idle_Hash,CROSS_FADE_DURATION);
		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Idle_State");
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Filed_Idle,
				"Is_Filed_Idle",
				false);
		}

		public Field_Idle_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
	}
	public class Field_Locomotion_State : FieldState,IWireable
	{
		public Field_Locomotion_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }

		public override void OnEnter()
		{
			_playerController.SetMaster(true);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Locomotion_State");
			_animator.CrossFade(field_Locomotion_Hash, 2 * CROSS_FADE_DURATION);
		}

		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Locomotion_State");
		}

	}
	public class Field_Stop_run_State : FieldState
	{
		public Field_Stop_run_State (PlayerHandler playerController, Animator animator) : base(playerController, animator)
		{
		}

		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Stop_run_State");
			_playerController.SetMaster(false);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Run_Stopping,
				"Is_Field_Run_Stopping",
				true);
			if(_playerController.PlayerStateMachine.CurrentSpeed > 0.95f)
			{
				_animator.CrossFade(field_stop_run_fast_Hash, 0.1f);
			}
			else if (_playerController.PlayerStateMachine.CurrentSpeed > 0.6f)
			{
				_animator.CrossFade(field_stop_run_fast_Hash, 0.35f);
			}
			else
			{
				_animator.CrossFade(field_stop_run_Hash, 4 * CROSS_FADE_DURATION);
			}

		}
		public override void FixedUpdate()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
			{
				_playerController.SetMaster(true);
			}
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
			{
				Debug.Log("ChangeState Field_Stop_run_State To Field_Idle_State");
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Idle_State));
			}
		}

		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Stop_run_State");
			_playerController.SetMaster(true);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Run_Stopping,
				"Is_Field_Run_Stopping",
				false);
		}
	}
	public class Field_Exhaust_State : FieldState
	{
		public Field_Exhaust_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Exhaust_State");
			_playerController.SetMaster(false);
			_animator.CrossFade(field_start_exhaust_Hash, CROSS_FADE_DURATION);
		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Exhaust_State");
		}
	}
	public class Field_Stop_Exhaust_State : FieldState
	{
		public Field_Stop_Exhaust_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Exhaust_State");
			_playerController.SetMaster(false);
			_animator.CrossFade(field_stop_exhaust_Hash, 0.9f);
		}
		public override void FixedUpdate()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
			{
				Debug.Log("ChangeState Field_Stop_run_State To Field_Idle_State");
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Idle_State));
			}
		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Exhaust_State");
			_playerController.SetMaster(true);
		}
	}

	public class Field_Start_Run2Scroll_State : FieldState
	{
		public Field_Start_Run2Scroll_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Start_Run2Scroll_State");
			_playerController.SetMaster(false);
			if (_playerController.PlayerStateMachine.InputVectorX > 0 && _playerController.PlayerStateMachine.InputVectorY > 0)
			{
				var inputVector = new FPVector3((FP)_playerController.PlayerStateMachine.InputVectorX / 10, (FP)0,(FP)_playerController.PlayerStateMachine.InputVectorY / 10);
				_animator.transform.localPosition = _playerController.transform.localPosition;
				var quaternion = FPQuaternion.LookRotation(inputVector);
				_animator.transform.rotation = quaternion.ToUnityQuaternion();
			}
			_animator.Play(field_start_run2scroll_Hash);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				true);
			_playerController.PlayerAttributeCost(0,PlayerAttributionValues.Scroll_Cost);
		}
		AnimatorStateInfo animate;
		public override void FixedUpdate()
		{
			animate = _animator.GetCurrentAnimatorStateInfo(0);   
			if (animate.normalizedTime >= 0.95f)
			{
				Debug.LogFormat("<color=#{0}>{1}</color>",Color.blue.ColorToHex(),"Field_Start_Run2Scroll_State To Field_Locomotion_State");
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
			if (animate.normalizedTime >= 0.8f)
			{
				_playerController.SetMaster(true);
				_animator.AnimatorSetBool(
					out _playerController.PlayerStateMachine.Is_Field_Scrolling,
					"Is_Field_Scrolling",
					false);
			}

		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Start_Run2Scroll_State");
			_playerController.SetMaster(true);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				false);
		}
	}

	public class Field_Start_Scroll_State : FieldState
	{
		public Field_Start_Scroll_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Start_Scroll_State");
			_playerController.SetMaster(false);
			if (_playerController.PlayerStateMachine.InputVectorX > 0 && _playerController.PlayerStateMachine.InputVectorY > 0)
			{
				var inputVector = new FPVector3((FP)_playerController.PlayerStateMachine.InputVectorX / 10, (FP)0,(FP)_playerController.PlayerStateMachine.InputVectorY / 10);
				_animator.transform.localPosition = _playerController.transform.localPosition;
				var quaternion = FPQuaternion.LookRotation(inputVector);
				_animator.transform.rotation = quaternion.ToUnityQuaternion();
			}
			_animator.Play(field_start_scroll_Hash);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				true);
			_playerController.PlayerAttributeCost(0,PlayerAttributionValues.Scroll_Cost);
		}
		AnimatorStateInfo animate;
		public override void FixedUpdate()
		{
			animate = _animator.GetCurrentAnimatorStateInfo(0);   
			if (animate.normalizedTime >= 0.95f)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
			if (animate.normalizedTime >= 0.8f)
			{
				_playerController.SetMaster(true);
				_animator.AnimatorSetBool(
					out _playerController.PlayerStateMachine.Is_Field_Scrolling,
					"Is_Field_Scrolling",
					false);			
			}
		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Start_Scroll_State");
			_playerController.SetMaster(true);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				false);
		}
	}
	
	public class Field_Start_Scroll2Locomotion_State : FieldState
	{
		public Field_Start_Scroll2Locomotion_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Start_Scroll2Locomotion_State");
			_playerController.SetMaster(false);
			_animator.CrossFade(field_start_scroll2locomotion_Hash, CROSS_FADE_DURATION);
		}
		public override void FixedUpdate()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
		}
		public override void OnExit()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Start_Scroll2Locomotion_State");
			_playerController.SetMaster(true);
		}
	}
	#region WireState
	public class WireState:FieldState,IWireable
	{
		public WireState(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				true);
		}
	}
	public class WireUpState:FieldState
	{
		public WireUpState(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
	}
	
	public class Field_Wire_Start_Up_State:WireUpState
	{
		public Field_Wire_Start_Up_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		private bool flag = false;
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Up_State");
			_playerController.SetMaster(false);
			_animator.CrossFade(field_wire_start_up_Hash,CROSS_FADE_DURATION);
			flag = false;
		}

		private AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			flag = _playerController.PlayerStateMachine.Is_WiredUp_UseWire;
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.normalizedTime >= 0.92f && nowState.shortNameHash == field_wire_start_up_Hash)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}

			if (nowState.normalizedTime >= 0.3f && flag && nowState.shortNameHash == field_wire_start_up_Hash)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Wire_Start_Rising));
			}
		}
		public override void OnExit()
		{
			_playerController.SetMaster(true);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Up_State");
		}
	}

	public class Field_Wire_Start_Rising : WireState
	{
		public Field_Wire_Start_Rising (PlayerHandler playerController, Animator animator) : base(playerController, animator) { }

		public override void OnEnter()
		{
			base.OnEnter();
			Debug.LogFormat("<color=#{0}>{1}</color>", Color.green.ColorToHex(), "Enter Field_Wire_Start_Rising");
			_playerController.SetMaster(true);
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
			CommandPlayerJump commandJump = new CommandPlayerJump()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandJump);
			_animator.CrossFade(field_wire_start_rising_Hash, CROSS_FADE_DURATION);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_start_rising_Hash &&  _playerController.PlayerStateMachine.Is_Wire_Jump)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Wire_Jump_State));
			}
			if (nowState.shortNameHash == field_wire_start_rising_Hash &&  nowState.normalizedTime >= 1f)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Wire_Start_Falling_State));
			}
		}

		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Rising");
			_playerController.SetMaster(true);
		}
	}

	public class Field_Wire_Start_Right_State:WireState
	{
		public Field_Wire_Start_Right_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Right_State");

			_playerController.SetMaster(false);
			_animator.transform.rotation = Quaternion.LookRotation(FPVector3.Right.ToUnityVector3());
			_animator.CrossFade(field_wire_start_Hash, CROSS_FADE_DURATION);
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}

		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_move_Hash &&  nowState.normalizedTime >= WireTowards.WireAnimEnd)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
		}

		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Right_State");
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
	}
	
	public class Field_Wire_Start_Left_State:WireState
	{
		public Field_Wire_Start_Left_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public AnimatorStateInfo nextState;
		public override void OnEnter()
		{
			base.OnEnter();

			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Left_State");

			_playerController.SetMaster(false);
			_animator.transform.rotation = Quaternion.LookRotation(FPVector3.Left.ToUnityVector3());
			_animator.CrossFade(field_wire_start_Hash, CROSS_FADE_DURATION);
			// Get the next state info
			nextState = _animator.GetNextAnimatorStateInfo(0);
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_move_Hash &&  nowState.normalizedTime >= WireTowards.WireAnimEnd)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
		}
		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Left_State");
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
	}
	
	public class Field_Wire_Start_Down_State:WireState
	{
		public Field_Wire_Start_Down_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public AnimatorStateInfo nextState;
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Down_State");
			

			_playerController.SetMaster(false);
			_animator.transform.rotation = Quaternion.LookRotation(FPVector3.Back.ToUnityVector3());
			_animator.CrossFade(field_wire_start_Hash, CROSS_FADE_DURATION);
			// Get the next state info
			nextState = _animator.GetNextAnimatorStateInfo(0);
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_move_Hash &&  nowState.normalizedTime >= WireTowards.WireAnimEnd)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
		}

		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Down_State");
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
	}

	public class Field_Wire_Start_Forward_State:WireState
	{
		public Field_Wire_Start_Forward_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			base.OnEnter();
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Forward_State");
			_playerController.SetMaster(false);
			_animator.CrossFade(field_wire_start_Hash, CROSS_FADE_DURATION);
			_animator.transform.rotation = Quaternion.LookRotation(FPVector3.Forward.ToUnityVector3());
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = true,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_move_Hash &&  nowState.normalizedTime >= WireTowards.WireAnimEnd)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
		}

		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Forward_State");
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
	}
	
	public class WireJumpState : FieldState,IWireable
	{
		public WireJumpState(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
	}

	public class Wire_Jump_State:WireJumpState
	{
		public Wire_Jump_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Wire_Jump_State");
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
			_playerController.SetMaster(true);
			_animator.transform.rotation = Quaternion.LookRotation(FPVector3.Forward.ToUnityVector3());
			_animator.CrossFade(field_wire_jump_Hash, CROSS_FADE_DURATION);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.shortNameHash == field_wire_jump_Hash &&  nowState.normalizedTime >= 0.95f)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Wire_Start_Falling_State));
			}
		}

		public override void OnExit()
		{
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Wiring,
				"Is_Wiring",
				false);
			_playerController.SetMaster(true);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Wire_Jump_State");
		}
	}
	
	#endregion

	public class Falling_State : FieldState,IWireable
	{
		protected static readonly int field_wire_start_falling_Hash = Animator.StringToHash("field_wire_start_falling_loop");

		public Falling_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
	}
	
	public class Field_Wire_Start_Falling_State : Falling_State
	{
		public Field_Wire_Start_Falling_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
        public override void OnEnter()
        {
            Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Wire_Start_Falling_State");
            _animator.AnimatorSetBool(
	            out _playerController.PlayerStateMachine.Is_Wiring,
	            "Is_Wiring",
	            false);
            CommandSetWire commandSetWire = new CommandSetWire()
            {
	            Player = _playerController.GetPlayerRef()._index,
	            flag = false,
            };
            QuantumRunner.Default.Game.SendCommand(commandSetWire);
            _playerController.SetMaster(true);
            _animator.CrossFade(field_wire_start_falling_Hash, CROSS_FADE_DURATION);
        }

        public override void OnExit()
        {
	        _playerController.SetMaster(true);
	        Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Wire_Start_Falling_State");
        }
        
	}
	
	public class Field_Falling_Scroll_State : FieldState
	{
		public Field_Falling_Scroll_State(PlayerHandler playerController, Animator animator) : base(playerController, animator) { }
		public override void OnEnter()
		{
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.green.ColorToHex(),"Enter Field_Falling_Scroll_State");
			_playerController.SetMaster(true);
			_animator.CrossFade(field_falling_scroll_Hash, CROSS_FADE_DURATION);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				true);
		}
		public AnimatorStateInfo nowState;
		public override void FixedUpdate()
		{
			nowState = _animator.GetCurrentAnimatorStateInfo(0);
			if (nowState.normalizedTime >= 0.95f && nowState.shortNameHash == field_falling_scroll_Hash)
			{
				_playerController.PlayerStateMachine._stateMachine.ChangeState(typeof(Field_Locomotion_State));
			}
			if (nowState.normalizedTime >= 0.8f && nowState.shortNameHash == field_falling_scroll_Hash)
			{
				CommandSetWire commandSetWire = new CommandSetWire()
				{
					Player = _playerController.GetPlayerRef()._index,
					flag = false,
				};
				QuantumRunner.Default.Game.SendCommand(commandSetWire);
			}
		}
		public override void OnExit()
		{
			_playerController.SetMaster(true);
			_animator.AnimatorSetBool(
				out _playerController.PlayerStateMachine.Is_Field_Scrolling,
				"Is_Field_Scrolling",
				false);
			Debug.LogFormat("<color=#{0}>{1}</color>",Color.red.ColorToHex(),"Exit Field_Falling_Scroll_State");
			CommandSetWire commandSetWire = new CommandSetWire()
			{
				Player = _playerController.GetPlayerRef()._index,
				flag = false,
			};
			QuantumRunner.Default.Game.SendCommand(commandSetWire);
		}
	}
}
