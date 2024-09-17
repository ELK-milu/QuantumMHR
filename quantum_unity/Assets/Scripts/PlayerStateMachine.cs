using Photon.Deterministic;
using Quantum;
using StatePattern.PlayerState;
using StatePattern.StateSystem;
using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStateMachine : CharacterStateMachine
{

	#region 基础状态判断条件
	private bool _isGround = false;
	private bool _isLocomotionBtnPerform = false;
	private bool _isExhaust = false;
	private bool _isFullEnergy = false;
	private bool _isScroll = false;
	private bool _isFalling = false;
	
	// Wire
	private bool _isWiredCombineBtnPerform;
	private bool _isWiredUp;
	private bool _isWiredDown;
	private bool _isWiredLeft;
	private bool _isWiredRight;
	private bool _isWiredForward;
	#endregion

	#region 最终状态判断
	public bool Is_Filed_Idle = true;
	public bool Is_Field_Locomotion = false;
	public bool Is_Field_Run_Stop = false;
	public bool Is_Filed_Exhaust = false;
	public bool Is_Field_Scroll = false;
	public bool Is_Field_Locomotion_Scroll = false;
	public bool Is_Field = false;
	public bool Is_Falling = false;
	private bool Is_Ground = false;

	
	// Wire
	
	// 铁虫丝移动时按下滚键,在空中跳跃
	public bool Is_Wire_Jump = false;
	public bool Is_Wire = false;
	public bool Is_Wire_Control = false;
	public bool Is_Wiring = false;
	public bool Is_Field_Wire_Start_Up = false;
	public bool Is_WiredUp_UseWire;
	public bool Is_Field_Wire_Start_Right = false;
	public bool Is_Field_Wire_Start_Left = false;
	public bool Is_Field_Wire_Start_Down = false;
	public bool Is_Field_Wire_Start_Forward = false;


	
	
	// 动画播放ing状态，用于更细微的动画控制，由状态在OnEnter和OnExit时主动控制
	public bool Is_Field_Scrolling = false;
	public bool Is_Field_Run_Stopping = false;

	
	public bool Is_FullEnergy = false;
	public bool IsSkill02 = false;
	#endregion
	
	// 判断模型动画和物理模拟的主从关系
	public bool IsMaster = true;
	public int Speed = Animator.StringToHash("Speed");
	public float CurrentSpeed = 0;
	public SyncPhyicEntity _syncPhyicEntity;
	public PlayerModelController _PlayerModelController;
	public PlayerHandler PlayerHandler;

	public short InputVectorX;
	public short InputVectorY;
	public override void SetStateMachine(PlayerController playerController,StateMachine stateMachine,Animator animator)
	{
		PlayerHandler =  playerController as PlayerHandler;
        _stateMachine = stateMachine;
		_animator = animator;
		_syncPhyicEntity = _animator.GetComponent<SyncPhyicEntity>();
		_PlayerModelController = _animator.GetComponent<PlayerModelController>();
		_stateMachine = new StateMachine();

		// 构造状态时也可初始化
		var field_Locomotion_State = new Field_Locomotion_State(PlayerHandler, _animator);
		var field_Idle_State = new Field_Idle_State(PlayerHandler, _animator);
		var field_Stop_run_State = new Field_Stop_run_State(PlayerHandler, _animator);
		var field_Exhaust_State = new Field_Exhaust_State(PlayerHandler, _animator);
		var field_Stop_Exhaust_State = new Field_Stop_Exhaust_State(PlayerHandler, _animator);
		
		var field_Start_Run2Scroll_State = new Field_Start_Run2Scroll_State(PlayerHandler, _animator);
		var field_Start_Scroll_State = new Field_Start_Scroll_State(PlayerHandler, _animator);

		var wire_Jump_State = new Wire_Jump_State(PlayerHandler, _animator);
		var Field_Wire_Start_Up_State = new Field_Wire_Start_Up_State(PlayerHandler, _animator);
		var Field_Wire_Start_Right_State = new Field_Wire_Start_Right_State(PlayerHandler, _animator);
		var Field_Wire_Start_Left_State = new Field_Wire_Start_Left_State(PlayerHandler, _animator);
		var Field_Wire_Start_Down_State = new Field_Wire_Start_Down_State(PlayerHandler, _animator);
        var Field_Wire_Start_Forward_State = new Field_Wire_Start_Forward_State(PlayerHandler, _animator);
        var Field_Wire_Start_Falling_State = new Field_Wire_Start_Falling_State(PlayerHandler, _animator);
        var field_Falling_Scroll_State = new Field_Falling_Scroll_State(PlayerHandler, _animator);
        var field_Wire_Start_Rising = new Field_Wire_Start_Rising(PlayerHandler, _animator);
        _stateMachine.SetState(field_Idle_State);
        


		
		var g_sword_skill_02_move2r_State = new G_sword_skill_02_move2r_State(PlayerHandler, _animator);
		
		
		_stateMachine.At(field_Idle_State, field_Locomotion_State, new FuncPredicate(() => Is_Field_Locomotion));
		_stateMachine.At(field_Locomotion_State,field_Idle_State, new FuncPredicate(() => Is_Field_Run_Stop));
		_stateMachine.At(field_Locomotion_State,field_Exhaust_State, new FuncPredicate(() => Is_Filed_Exhaust));
		_stateMachine.At(field_Exhaust_State,field_Stop_Exhaust_State, new FuncPredicate(() => Is_FullEnergy));
		
		_stateMachine.At(field_Locomotion_State,field_Start_Run2Scroll_State, new FuncPredicate(() => Is_Field_Locomotion_Scroll));
		_stateMachine.At(field_Start_Run2Scroll_State,field_Start_Run2Scroll_State, new FuncPredicate(() => Is_Field_Locomotion_Scroll));
		_stateMachine.At(field_Start_Scroll_State,field_Start_Run2Scroll_State, new FuncPredicate(() => Is_Field_Locomotion_Scroll));

		_stateMachine.At(field_Idle_State,field_Start_Scroll_State, new FuncPredicate(() => Is_Field_Scroll));
		_stateMachine.At(field_Idle_State,field_Idle_State, new FuncPredicate(() => Is_Field_Scroll));
		
		//_stateMachine.At(field_Start_Run2Scroll_State,field_Start_Scroll2Locomotion_State, new FuncPredicate(() => !Is_Field_Scrolling));
		//_stateMachine.At(field_Start_Scroll_State,field_Start_Scroll2Locomotion_State, new FuncPredicate(() => !Is_Field_Scrolling));

		_stateMachine.At(field_Idle_State,field_Idle_State, new FuncPredicate(() => Is_Field_Scroll));

        
		// 下落动画
		//_stateMachine.At(Field_Wire_Start_Falling_State, field_Falling_Scroll_State, new FuncPredicate(() => Is_Ground));


		// TODO:铁虫丝需要添加非固定状态控制 Field_Wire_Start_Move_State
		
		// 铁虫丝的固定十字方位移动适配手柄操作
		// TODO:Is_Field_Wire_Start_Up等精化条件判断
		
		// wire_start_up
		_stateMachine.At(typeof(IWireable),Field_Wire_Start_Up_State, new FuncPredicate(() => Is_Field_Wire_Start_Up));
		// wire_start_right

		_stateMachine.At(typeof(IWireable),Field_Wire_Start_Right_State, new FuncPredicate(() => Is_Field_Wire_Start_Right));

		//_stateMachine.At(field_Idle_State,Field_Wire_Start_Right_State, new FuncPredicate(() => Is_Field_Wire_Start_Right));
		//_stateMachine.At(field_Locomotion_State,Field_Wire_Start_Right_State, new FuncPredicate(() => Is_Field_Wire_Start_Right));
		
		// wire_start_left

		_stateMachine.At(typeof(IWireable),Field_Wire_Start_Left_State, new FuncPredicate(() => Is_Field_Wire_Start_Left));
		//_stateMachine.At(field_Idle_State,Field_Wire_Start_Left_State, new FuncPredicate(() => Is_Field_Wire_Start_Left));
		//_stateMachine.At(field_Locomotion_State,Field_Wire_Start_Left_State, new FuncPredicate(() => Is_Field_Wire_Start_Left));

		
		// wire_start_down

		_stateMachine.At(typeof(IWireable),Field_Wire_Start_Down_State, new FuncPredicate(() => Is_Field_Wire_Start_Down));
		//_stateMachine.At(field_Idle_State,Field_Wire_Start_Down_State, new FuncPredicate(() => Is_Field_Wire_Start_Down));
		//_stateMachine.At(field_Locomotion_State,Field_Wire_Start_Down_State, new FuncPredicate(() => Is_Field_Wire_Start_Down));

		
		// wire_start_forward
		_stateMachine.At(typeof(IWireable),Field_Wire_Start_Forward_State, new FuncPredicate(() => Is_Field_Wire_Start_Forward));

		//_stateMachine.At(field_Idle_State,Field_Wire_Start_Forward_State, new FuncPredicate(() => Is_Field_Wire_Start_Forward));
		//_stateMachine.At(field_Locomotion_State,Field_Wire_Start_Forward_State, new FuncPredicate(() => Is_Field_Wire_Start_Forward));
		
		// ResetToIdle
		_stateMachine.Any(field_Idle_State, new FuncPredicate(() => IsSkill02));
		
		//_stateMachine.At(typeof(WireState),typeof(WireState), new FuncPredicate(() => Is_Wire_Control));
		_stateMachine.At(typeof(Falling_State),wire_Jump_State, new FuncPredicate(() => Is_Wire_Jump));
		_stateMachine.At(typeof(WireState),wire_Jump_State, new FuncPredicate(() => Is_Wire_Jump));
		//_stateMachine.At(typeof(WireState),field_Idle_State, new FuncPredicate(() => !Is_Wire));
		_stateMachine.At(typeof(WireJumpState), field_Falling_Scroll_State, new FuncPredicate(() => Is_Ground));
		_stateMachine.At(typeof(Falling_State), field_Falling_Scroll_State, new FuncPredicate(() => Is_Ground));
	}

	public void SetStatus(PlayerLink playerLink,CharacterController3D movement,float speed)
	{
		GetQuantumStatus(playerLink,movement,speed);
		Get_Is_Filed_Idle();
		Get_Is_Field_Locomotion();
		Get_Is_Field_Run_Stop();
		Get_Is_Field_Exhaust();
		Get_Is_Full_Energy();
		Get_Is_Falling();
		Get_Is_Ground();
		Get_Is_Scroll(playerLink);
		Get_Is_Field_Locomotion_Scroll();
		
		// Wire
		Get_Is_Jump();
		Get_Is_Wire();
		Get_Is_Wire_Control();
		Get_Is_Field_Wire_Start_Up();
		Get_Is_Field_Wire_Start_Right();
		Get_Is_Field_Wire_Start_Left();
		Get_Is_Field_Wire_Start_Down();
		Get_Is_Field_Wire_Start_Forward();
		Get_Is_Filed();
		
		//SetAnimator();
		_animator.SetFloat(Speed, CurrentSpeed);

	}

	private void Get_Is_Jump()
	{
		Is_Wire_Jump = _isScroll;
	}

	private void Get_Is_Filed()
	{
		Is_Field = Is_Filed_Idle | Is_Field_Locomotion | Is_Field_Run_Stop | Is_Filed_Exhaust 
		           | Is_Field_Locomotion_Scroll | Is_Field_Scroll | Is_Field_Scrolling | Is_Field_Run_Stopping && !Is_Wire && !Is_Wiring;
		_PlayerModelController.isField = Is_Field;
	}

	private void Get_Is_Field_Locomotion_Scroll()
	{
		Is_Field_Locomotion_Scroll = Is_Field_Locomotion && Is_Field_Scroll;
	}

	private void Get_Is_Scroll(PlayerLink playerLink)
	{
		Is_Field_Scroll = _isScroll && playerLink.Attribution.Energy>=PlayerAttributionValues.Limit_Min_Scroll_Engery  && !Is_Field_Scrolling;
	}

	// Wire
	private void Get_Is_Wire()
	{
		Is_Wire = Is_Wire_Control | Is_Wiring ;
	}
	private void Get_Is_Wire_Control()
	{
		Is_Wire_Control = Is_Field_Wire_Start_Right | Is_Field_Wire_Start_Left | Is_Field_Wire_Start_Up | Is_Field_Wire_Start_Forward |Is_Field_Wire_Start_Down;
	}
	private void Get_Is_Field_Wire_Start_Up()
	{
		Is_Field_Wire_Start_Up = _isWiredUp && !Is_Field_Scrolling;
	}
	
	private void Get_Is_Field_Wire_Start_Right()
	{
		Is_Field_Wire_Start_Right = _isWiredRight && !Is_Field_Scrolling;
	}
	
	private void Get_Is_Field_Wire_Start_Left()
	{
		Is_Field_Wire_Start_Left = _isWiredLeft && !Is_Field_Scrolling;
	}
	private void Get_Is_Field_Wire_Start_Down()
	{
		Is_Field_Wire_Start_Down = _isWiredDown && !Is_Field_Scrolling;
	}
	private void Get_Is_Field_Wire_Start_Forward()
	{
		Is_Field_Wire_Start_Forward = _isWiredForward && !Is_Field_Scrolling;
	}


	
	
	/// <summary>
	/// Debug用
	/// </summary>
	private void SetAnimator()
	{
#if UNITY_EDITOR
		// ---------General-----------
		_animator.SetBool("Is_Falling",Is_Falling);
		_animator.SetBool("Is_Ground",Is_Ground);
		_animator.SetBool("Is_FullEnergy",Is_FullEnergy);
		
		// -----------Field---------
		_animator.SetBool("Is_Filed_Idle",Is_Filed_Idle);
		_animator.SetBool("Is_Field_Run_Stop",Is_Field_Run_Stop);
		_animator.SetBool("Is_Field_Locomotion",Is_Field_Locomotion);
		_animator.SetBool("Is_Field_Exhaust",Is_Filed_Exhaust);
		_animator.SetBool("Is_Field_Scroll",Is_Field_Scroll);
		_animator.SetBool("Is_Field_Scrolling",Is_Field_Scrolling);
		_animator.SetBool("Is_Field_Run_Stopping",Is_Field_Run_Stopping);
		_animator.SetBool("Is_Field_Locomotion_Scroll",Is_Field_Locomotion_Scroll);
		_animator.SetBool("Is_Field",Is_Field);

		// ---------Field_Wired-----------
		_animator.SetBool("Is_Wire_Jump",Is_Wire_Jump);
		_animator.SetBool("Is_Wire",Is_Wire);
		_animator.SetBool("Is_Wiring",Is_Wiring);
		_animator.SetBool("Is_Field_Wire_Start_Up",Is_Field_Wire_Start_Up);
		_animator.SetBool("Is_Field_Wire_Start_Right",Is_Field_Wire_Start_Right);
		_animator.SetBool("Is_Field_Wire_Start_Left",Is_Field_Wire_Start_Left);
		_animator.SetBool("Is_Field_Wire_Start_Down",Is_Field_Wire_Start_Down);
		_animator.SetBool("Is_Field_Wire_Start_Forward",Is_Field_Wire_Start_Forward);

#endif
	}

	#region 状态判断
	private void Get_Is_Full_Energy()
	{
		Is_FullEnergy = _isFullEnergy;
	}
	private void Get_Is_Falling()
	{
		Is_Falling = _isFalling;
	}
	private void Get_Is_Ground()
	{
		Is_Ground = _isGround;
	}
	private void Get_Is_Field_Exhaust()
	{
		if (Is_Filed_Exhaust && !Is_FullEnergy)
		{
			return;
		}
		Is_Filed_Exhaust = _isExhaust;//&& IsFiled
	}
	
	private void Get_Is_Field_Run_Stop()
	{
		Is_Field_Run_Stop = !_isLocomotionBtnPerform&& _isGround;
	}

	private void Get_Is_Field_Locomotion()
	{
		Is_Field_Locomotion = IsMaster && _isLocomotionBtnPerform && _isGround && CurrentSpeed>0.05 && !Is_Wire;
	}
	private void Get_Is_Filed_Idle()
	{
		Is_Filed_Idle = IsMaster && !_isLocomotionBtnPerform && _isGround && Is_Field_Run_Stop;
	}
	#endregion

	/// <summary>
	/// 所有输入不能从本地直接获取，只有从Quantum端验证并转发的输入才可控制实体
	/// 由Quantum端获取状态进行动画切换
	/// </summary>
	/// <param name="playerLink"></param>
	/// <param name="movement"></param>
	/// <param name="speed"></param>
	public void GetQuantumStatus (PlayerLink playerLink,CharacterController3D movement,float speed)
	{
		InputVectorX = playerLink.InputDirctionX;
		InputVectorY = playerLink.InputDirctionY;
		_isGround = movement.Grounded;
		IsSkill02 = UnityEngine.Input.GetKeyDown(KeyCode.T);
		_isExhaust = playerLink.Attribution.Energy == 0;
		_isFullEnergy = playerLink.Attribution.Energy == playerLink.Attribution.MaxEnergy;
		_isScroll = playerLink.State.IsPlayerScroll;
		_isFalling = playerLink.State.IsFalling;

		// Wire
		_isWiredCombineBtnPerform = playerLink.State.IsWireCombine;
		_isWiredUp = playerLink.State.IsWireUp;
		
		UpdateSpeed(speed);
		if (CurrentSpeed>0.05)
		{
			_isLocomotionBtnPerform = true;
		}
		else
		{
			_isLocomotionBtnPerform = false;
		}
	}

	#region QuantumEvent
	
	public void OnPlayerWireForward (EventOnPlayerWireForward callback)
	{
		_isWiredForward = true;
	}

	public void OnPlayerWireDown (EventOnPlayerWireDown callback)
	{
		_isWiredDown = true;
	}

	public void OnPlayerWireLeft (EventOnPlayerWireLeft callback)
	{
		_isWiredLeft = true;
	}

	public void OnPlayerWireRight (EventOnPlayerWireRight callback)
	{
		_isWiredRight = true;
	}

	public void OnPlayerWireUp (EventOnPlayerWireUp callback)
	{
		Is_WiredUp_UseWire = true;
	}
	
	public void OnPlayerSetWire (EventOnPlayerSetWire callback)
	{
		if (callback.flag == false)
		{
			_isWiredForward = false;
			_isWiredDown = false;
			_isWiredLeft = false;
			_isWiredRight = false;
			Is_WiredUp_UseWire = false;
		}
	}
	
	#endregion
	
	

	public void SetMaster (bool flag)
	{
		_syncPhyicEntity.IsMaster = !flag;
		IsMaster = flag;
	}
	
	public void UpdateSpeed (float speed)
	{
		if (!_animator) return;
		CurrentSpeed = speed;
	}

	
}
