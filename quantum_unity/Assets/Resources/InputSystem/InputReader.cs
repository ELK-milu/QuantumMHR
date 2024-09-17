using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader",menuName = "ScriptableObjects/InputReader")]
public class InputReader : ScriptableObject,PlayerInputActions.IPlayerActions
{
    /// <summary>
    /// 带参委托说明需要对Input的参数进行处理
    /// </summary>
    public event Action<Vector2> OnMoveHandler = delegate (Vector2 vector2) {  }; 

    public event Action<Vector2,bool> OnLookHandler = delegate (Vector2 vector2, bool b) {  };
    /// <summary>
    /// 无参委托只需在按键时触发事件即可
    /// </summary>
    public event Action OnAttackHandler = delegate {  };
    public event Action OnLockCameraHandler = delegate {  };
    public event Action<bool> OnScrollHandler = delegate {  };
    public event Action<bool> OnDashHandler = delegate {  };
    
    public event Action<bool> OnWireCombineHandler = delegate {  };
    public event Action<bool> OnWireUpHandler = delegate {  };
    public event Action<bool> OnWireDownHandler = delegate {  };
    public event Action<bool> OnWireLeftHandler = delegate {  };
    public event Action<bool> OnWireRightHandler = delegate {  };
    public event Action<bool> OnWireForwardHandler = delegate {  };
    
    public event Action<bool> OnTESTCombineHandler = delegate {  };
    

    private static PlayerInputActions _playerInputActions;
    public PlayerInputActions Instance
    {
        get
        {
            if (_playerInputActions == null)
            {
                _playerInputActions = new PlayerInputActions();
                _playerInputActions.Player.SetCallbacks(this);
            }
            return _playerInputActions;
        }
    }
    public Vector3 Direction => Instance.Player.Move.ReadValue<Vector2>();
    private void OnEnable()
    {
        Instance.Enable();
    }

    private void OnDisable()
    {
        Instance.Disable();
    }
    public void OnMove (InputAction.CallbackContext context)
    {
        OnMoveHandler?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnScroll (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                OnScrollHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnScrollHandler?.Invoke(false);
                break;
        }    
    }

    public void OnDash (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case  InputActionPhase.Started:
                OnDashHandler?.Invoke(true);
                break;
            case InputActionPhase.Performed:
                OnDashHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnDashHandler?.Invoke(false);
                break;
        }
    }

    public void OnWireCombine (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case  InputActionPhase.Started:
                OnWireCombineHandler?.Invoke(true);
                break;
            case InputActionPhase.Performed:
                OnWireCombineHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireCombineHandler?.Invoke(false);
                break;
        }    
    }

    public void OnWireUp (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case  InputActionPhase.Started:
                OnWireUpHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireUpHandler?.Invoke(false);
                break;
        }    
    }

    public void OnWireDown (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                OnWireDownHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireDownHandler?.Invoke(false);
                break;
        }
    }

    public void OnWireLeft (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                OnWireLeftHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireLeftHandler?.Invoke(false);
                break;
        }
    }
    public void OnWireRight (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                OnWireRightHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireRightHandler?.Invoke(false);
                break;
        }
    }

    public void OnWireForward (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                OnWireForwardHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnWireForwardHandler?.Invoke(false);
                break;
        }    
    }

    public void OnTEST (InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case  InputActionPhase.Started:
                OnTESTCombineHandler?.Invoke(true);
                break;
            case InputActionPhase.Performed:
                OnTESTCombineHandler?.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                OnTESTCombineHandler?.Invoke(false);
                break;
        }
    }

    public void OnLook (InputAction.CallbackContext context)
    {
    }

    public void OnFire (InputAction.CallbackContext context)
    {
    }

}
