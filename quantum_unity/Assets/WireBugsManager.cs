using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Collections;
using UnityEngine;

public class WireBugsManager : PlayerUIManager
{
	public WireBugController[] WireBugsObjects;
	private int _nowWireBugCount;
	private FP _wireCountDown;
	private WireBug _wireBug;
	private QList<WireBugStatus> _list;

	public override void UIUpdate()
	{
		base.UIUpdate();
		if(_playerUIController.Frame is null) return;
		_nowWireBugCount = _playerUIController.Frame.Get<PlayerLink>(_entityRef).Attribution.WireTotalNum;
		_wireCountDown = _playerUIController.Frame.Get<PlayerLink>(_entityRef).Attribution.WireCountDown;
		_wireBug = _playerUIController.Frame.Get<WireBug>(_entityRef);
		_playerUIController.Frame.TryResolveList(_wireBug.WireBugStatus,out _list);
		for (int i = 0; i < WireBugsObjects.Length; i++)
		{
			if (i <= _nowWireBugCount - 1)
			{
				WireBugsObjects[i].gameObject.SetActive(true);
			}
			else
			{
				WireBugsObjects[i].gameObject.SetActive(false);
			}
			if(_list.Count <= 0) continue;
			if (_list[i].Available)
			{
				WireBugsObjects[i].SetActive(true);
			}
			else
			{
				WireBugsObjects[i].SetActive(false);
				WireBugsObjects[i].SetProcess(_list[i].CoolCount/_wireCountDown);
			}
		}
		for (int i = _nowWireBugCount; i < WireBugsObjects.Length; i++)
		{
			WireBugsObjects[i].SetActive(false);
		}
	}
}
