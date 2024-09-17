using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerModelController : ModelViewController
{
    public PlayerRef Player;
    public PlayerHandler PlayerHandler;
    [Header("重要部位")]
    public Transform L_Weapen;
    public Transform R_Weapen;
    public Transform Back;
    
    [Header("显示对象")]
    public List<GameObject> FieldShowObj;
    public List<GameObject> BattleShowObj;
    public bool isField;
    
    [Header("特效控制")]
    [SerializeField]
    private GameObject _wireEffect;
    [SerializeField]
    private WireTowards _wireEffectTowards;

    
    public override void RegisterQuantumEvent()
    {
        QuantumEvent.Subscribe<EventOnPlayerWireRight>(this, OnPlayerWireRight);
        QuantumEvent.Subscribe<EventOnPlayerWireLeft>(this, OnPlayerWireLeft);
        QuantumEvent.Subscribe<EventOnPlayerWireDown>(this, OnPlayerWireDown);
        QuantumEvent.Subscribe<EventOnPlayerWireForward>(this, OnPlayerWireForward);
        QuantumEvent.Subscribe<EventOnPlayerWireUp>(this, OnPlayerWireUp);


        QuantumEvent.Subscribe<EventOnPlayerSetWire>(this, OnPlayerSetWire);
    }

    #region Quantum Events
    
    private void OnPlayerWireUp (EventOnPlayerWireUp callback)
    {
        if(Player == default) return;
        if(PlayerHandler.PlayerStateMachine.Is_Wire) return;
        _wireEffectTowards.StartPosition = transform.position + new Vector3(0,1,0);
        _wireEffectTowards.NowPosition = transform.position + new Vector3(0,6,0)+ transform.forward*3;
        _wireEffectTowards.EndPosition = _wireEffectTowards.NowPosition ;
    }
    private void OnPlayerWireRight (EventOnPlayerWireRight callback)
    {
        if(Player == default) return;
        if(PlayerHandler.PlayerStateMachine.Is_Wire) return;
        _wireEffectTowards.StartPosition = transform.position + new Vector3(0,1,0);
        _wireEffectTowards.NowPosition = _wireEffectTowards.StartPosition;
        _wireEffectTowards.EndPosition = _wireEffectTowards.NowPosition + new Vector3(8,0,0);
    }
    
    private void OnPlayerWireLeft (EventOnPlayerWireLeft callback)
    {
        if(Player == default) return;
        if(PlayerHandler.PlayerStateMachine.Is_Wire) return;
        _wireEffectTowards.StartPosition = transform.position + new Vector3(0,1,0);
        _wireEffectTowards.NowPosition = _wireEffectTowards.StartPosition;
        _wireEffectTowards.EndPosition = _wireEffectTowards.NowPosition + new Vector3(-8,0,0);
    }
    
    private void OnPlayerWireDown (EventOnPlayerWireDown callback)
    {
        if(Player == default) return;
        if(PlayerHandler.PlayerStateMachine.Is_Wire) return;
        _wireEffectTowards.StartPosition = transform.position + new Vector3(0,1,0);
        _wireEffectTowards.NowPosition = _wireEffectTowards.StartPosition;
        _wireEffectTowards.EndPosition = _wireEffectTowards.NowPosition + new Vector3(0,0,-8);
    }
    
    private void OnPlayerWireForward (EventOnPlayerWireForward callback)
    {
        if(Player == default) return;
        if(PlayerHandler.PlayerStateMachine.Is_Wire) return;
        _wireEffectTowards.StartPosition = transform.position + new Vector3(0,1,0);
        _wireEffectTowards.NowPosition = _wireEffectTowards.StartPosition;
        _wireEffectTowards.EndPosition = _wireEffectTowards.NowPosition + new Vector3(0,0,8);
    }
    
    private void OnPlayerSetWire (EventOnPlayerSetWire callback)
    {
        if(Player == default) return;

        if (callback.flag == false)
        {
            _wireEffectTowards.EndPosition = transform.position + new Vector3(0,1,0);
            _wireEffectTowards.DoClose();
        }
        else
        {
            _wireEffect.SetActive(true);
            _wireEffectTowards.DoOpen();
        }
    }

    #endregion

    void Update()
    {
        foreach (var obj in FieldShowObj)
        {
            //obj.SetActive(isField);
            obj.SetActive(true);
        }
        foreach (var obj in BattleShowObj)
        {
            //obj.SetActive(!isField);
            obj.SetActive(false);
        }
    }
}
