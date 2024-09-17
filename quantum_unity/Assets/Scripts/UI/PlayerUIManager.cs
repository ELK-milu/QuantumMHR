using Quantum;
using UnityEngine;
public class PlayerUIManager:MonoBehaviour
{
    protected PlayerUIController _playerUIController{ get; private set; }
    protected GameUIController _gameUI { get; private set; }
    protected EntityRef _entityRef { get; private set; }
    public virtual void UIUpdate()
    {
        if(_playerUIController is null) return;
        if(_playerUIController.Frame is null) return;
    }
    public void SetPlayerUIController(PlayerUIController playerUIController)
    {
        _playerUIController = playerUIController;
    }
    public void SetGameUI(GameUIController gameUI)
    {
        _gameUI = gameUI;
    }
    public void SetRef(EntityRef entityRef)
    {
        _entityRef = entityRef;
    }

    public virtual void OnEnable()
    {
        if(!_gameUI) return;
        _gameUI.OnPlayingUpdateHandler += UIUpdate;
    }

    public virtual void OnDisable()
    {
        if(!_gameUI) return;
        _gameUI.OnPlayingUpdateHandler -= UIUpdate;
    }
}