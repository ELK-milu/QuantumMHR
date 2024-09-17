using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIController :MonoBehaviour,IEntityRegister
{

	public PlayerUIManager[] PlayerUIManagers;
	public EntityRef _entityRef { get; private set; }
	public Frame Frame { get; private set; }
	public GameUIController GameUI;

	/// <summary>
	/// 获取实体引用
	/// </summary>
	/// <param name="Frame"></param>
	public void SetFrame(Frame frame)
	{
		Frame = frame;
	}
	public void SetRef(EntityRef entityRef)
	{
		_entityRef = entityRef;
		foreach (var uiManager in PlayerUIManagers)
		{
			uiManager.SetRef(_entityRef);
			uiManager.SetGameUI(GameUI);
			uiManager.SetPlayerUIController(this);
			uiManager.gameObject.SetActive(true);
		}
	}
	
	public void DispatchRef()
	{
		foreach (var uiManager in PlayerUIManagers)
		{
			uiManager.gameObject.SetActive(false);
		}	
	}

}
