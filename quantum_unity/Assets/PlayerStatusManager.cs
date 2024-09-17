using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色状态UI管理
/// </summary>
public class PlayerStatusManager : PlayerUIManager
{
    [SerializeField]
    Slider _energyBar;
    public override void UIUpdate()
    {
        base.UIUpdate();
        if (_playerUIController.Frame is null) return;
        if (_playerUIController.Frame.TryGet<PlayerLink>(_entityRef, out var playerLink))
        {
            _energyBar.maxValue = playerLink.Attribution.MaxEnergy;
            _energyBar.value = playerLink.Attribution.Energy;
        }
    }

}
