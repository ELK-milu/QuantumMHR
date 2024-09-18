using Cinemachine;
using Photon.Deterministic;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using StatePattern.PlayerState;
using StatePattern.StateSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public unsafe class PlayerHandler : PlayerController
{
	public PlayerModelController SelfPlayerViewController;
	[SerializeField]
	private LocalInput _localInput;
	[SerializeField]
	private GameObject _perfab;
	[SerializeField]
	private GameObject _model;
	[SerializeField]
	private EntityView _entityView;
	[SerializeField]
	private Animator _animator;
	public bool IsLocalPlayer  { get;private set;}
	private StateMachine _stateMachine;

	#region Update获取的实体每帧信息
	private QuantumGame _game { get;set;}
	private Frame _frame { get;set;}
	private CharacterController3D _movement { get;set;}
	private PlayerRef _playerRef { get;set;}
	private PlayerLink _playerLink { get;set;}

	#endregion

	#region 实体交互用
	public PlayerStateMachine PlayerStateMachine { get;private set;}
    public event Action<Frame> OnFrameUpdateHandler = delegate {  };
    private List<IEntityRegister> _linkedScripts = new List<IEntityRegister>();
    #endregion

    #region  私有变量获取
    public PlayerRef GetPlayerRef()
    {
	    return _playerRef;
    }
    #endregion
    
    #region Entity Event
	public override void OnEntityInstantiated()
	{
		PlayerStateMachine = new PlayerStateMachine();
		GameObject model = null;
		if (!_animator)
		{
			var parent = Instantiate(_perfab);
			transform.SetParent(parent.transform);
			model = Instantiate(_model,parent.transform);
			_animator = model.GetComponent<Animator>();
			_animator.GetComponent<SyncPhyicEntity>().Initiate(transform);
			if (!SelfPlayerViewController)
			{
				SelfPlayerViewController = model.GetComponent<PlayerModelController>();
			}
			SelfPlayerViewController.PlayerHandler = this;
		}

		
		_localInput = GameObject.FindObjectOfType<LocalInput>().GetComponent<LocalInput>();

		Debug.Log("PlayerCharacter Controller OnEntityInstantiated");
		QuantumGame game = QuantumRunner.Default.Game;
		Frame frame = game.Frames.Verified;
		if (frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink))
		{
			if (game.PlayerIsLocal((playerLink.PlayerRef)))
			{
				CinemachineVirtualCamera virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
				virtualCamera.m_Follow = transform;
				SelfPlayerViewController.Player = playerLink.PlayerRef;
				
				IsLocalPlayer = true;
				
				LocalPlayerRegister();
			}
			else
			{
				IsLocalPlayer = false;
			}
			PlayerStateMachine.SetStateMachine(this,_stateMachine, _animator);
		}
		SubscribeQuantumEvent();

	}

	private void LocalPlayerRegister()
	{
		// 本地玩家注册UI对应实体
		var playerUIControllerRegister = FindObjectOfType<PlayerUIController>().GetComponent<IEntityRegister>();
		OnFrameUpdateHandler += playerUIControllerRegister.SetFrame;
		playerUIControllerRegister.SetRef(_entityView.EntityRef);
		_linkedScripts.Add(playerUIControllerRegister);
		// UI同步请求
		CommandPlayerStateSync command = new CommandPlayerStateSync()
		{
			Player = _playerRef._index,
		};
		QuantumRunner.Default.Game.SendCommand(command);
	}

	public override void OnEntityDestroyed()
	{
		OnFrameUpdateHandler = delegate (Frame frame) {  };
		foreach (var registers in _linkedScripts)
		{
			registers.DispatchRef();
		}
		_linkedScripts.Clear();
		UnSubscribeQuantumEvent();
	}
	#endregion

	#region Mono

	// 获取Quantum的固定Update时长进行更新
	private FP localTime = 0;
	private void FixedUpdate()
	{
		if (localTime != 0)
		{
			PlayerStateMachine._stateMachine.FidedUpdate();
			if (!PlayerStateMachine.IsMaster)
			{
				ResetTransform();
			}
			OnFrameUpdateHandler?.Invoke(_frame);
			StateMachineStatusUpdate(_playerLink,_movement);
			PlayerStateMachine._stateMachine.Update();
			if (!PlayerStateMachine.IsMaster)
			{
				transform.position = _animator.transform.position;
				transform.rotation = _animator.transform.rotation;
			}
			localTime = 0;
		}
	}

	public override void OnUpdateView(QuantumGame game)
	{
		base.OnUpdateView(game);
		GetQuantumInfo();
	}
	
	private void GetQuantumInfo()
	{
		_game = QuantumRunner.Default.Game;
		_frame = IsLocalPlayer ? _game.Frames.Predicted : _game.Frames.Verified;
		_playerLink = _frame.Get<PlayerLink>(_entityView.EntityRef);
		_playerRef = _playerLink.PlayerRef;
		_movement = _frame.Get<CharacterController3D>(_entityView.EntityRef);
		if (localTime == 0)
		{
			localTime = _frame.DeltaTime;
		}
	}
	#endregion

	#region FSM根据实体更新状态
	
	/// <summary>
	/// 决定位置信息控制方式，true代表输入器控制，false代表使用动画自带节点信息
	/// </summary>
	/// <param name="flag"></param>
	public void SetMaster(bool flag)
	{
		PlayerStateMachine.SetMaster(flag);
	}


	private void StateMachineStatusUpdate(PlayerLink playerLink,CharacterController3D movement)
	{
		PlayerStateMachine.SetStatus(playerLink,movement,_movement.Velocity.Magnitude.AsFloat / 6);
	}

	#endregion

	#region Quantum Event
	
	private void SubscribeQuantumEvent()
	{
		//QuantumEvent.Subscribe<EventOnPlayerJumped>(this, OnPlayerJumped);
		QuantumEvent.Subscribe<EventOnPlayerWireForward>(this, PlayerStateMachine.OnPlayerWireForward);
		QuantumEvent.Subscribe<EventOnPlayerWireLeft>(this, PlayerStateMachine.OnPlayerWireLeft);
		QuantumEvent.Subscribe<EventOnPlayerWireRight>(this, PlayerStateMachine.OnPlayerWireRight);
		QuantumEvent.Subscribe<EventOnPlayerWireDown>(this, PlayerStateMachine.OnPlayerWireDown);
		QuantumEvent.Subscribe<EventOnPlayerWireUp>(this, PlayerStateMachine.OnPlayerWireUp);
		QuantumEvent.Subscribe<EventOnPlayerSetWire>(this, PlayerStateMachine.OnPlayerSetWire);
	}


	private void UnSubscribeQuantumEvent()
	{
	}

	#endregion
	
	#region 修改实体值
	public void ResetTransform()
	{
		try
		{
			CommandResetPos command = new CommandResetPos()
			{
				Player = _playerRef._index,
				TransformPos = (_animator.transform.localPosition).ToFPVector3(),
				TransQuaternion = (_animator.transform.localRotation).ToFPQuaternion(),
			};
			QuantumRunner.Default.Game.SendCommand(command);
		}
		catch(Exception e)
		{
			Debug.Log($"ResetTransform出错啦 error:{e.Message}");
		}
	}
	public void ResetTransform(Vector3 transformPos,Quaternion trnasQuaternion)
	{
		try
		{
			CommandResetPos command = new CommandResetPos()
			{
				Player = _playerRef._index,
				TransformPos = transformPos.ToFPVector3(),
				TransQuaternion = trnasQuaternion.ToFPQuaternion(),
			};
			QuantumRunner.Default.Game.SendCommand(command);
		}
		catch(Exception e)
		{
			Debug.Log($"ResetTransform出错啦 error:{e.Message}");
		}
	}
	public void PlayerAttributeCost(int healthCost,int energyCost)
	{
		try
		{
			CommandPlayerAttributeCost command = new CommandPlayerAttributeCost()
			{
				Player = _playerRef._index,
				HealthCost = healthCost,
				EnergyCost = energyCost,
			};
			QuantumRunner.Default.Game.SendCommand(command);
		}
		catch(Exception e)
		{
			Debug.Log($"SetTransform出错啦 error:{e.Message}");
		}
	}
	#endregion

}