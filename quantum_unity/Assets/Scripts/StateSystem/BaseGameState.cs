using System;
using UnityEngine;

namespace StatePattern.StateSystem
{
	public abstract class BaseGameState : IState
	{
		public event Action OnEnterEventHandler;
		public event Action OnUpdateEventHandler;
		public event Action OnFixedUpdateEventHandler;
		public event Action OnLaterUpdateEventHandler;
		public event Action OnExitEventHandler;

		public virtual void OnEnter()
		{
			OnEnterEventHandler?.Invoke();
		}

		public virtual void Update()
		{
			OnUpdateEventHandler?.Invoke();
		}

		public virtual void FixedUpdate()
		{
			OnFixedUpdateEventHandler?.Invoke();
		}

		public void LaterUpdate()
		{
			OnLaterUpdateEventHandler?.Invoke();
		}

		public virtual void OnExit()
		{
			OnExitEventHandler?.Invoke();
		}

		public void OnEnterRegister (bool isRegister,Action action)
		{
			if (isRegister)
			{
				OnEnterEventHandler += action;
			}
			else
			{
				OnEnterEventHandler -= action;
			}
		}
		public void UpdateRegister (bool isRegister,Action action)
		{
			if (isRegister)
			{
				OnUpdateEventHandler += action;
			}
			else
			{
				OnUpdateEventHandler -= action;
			}
		}
		public void FixedUpdateRegister (bool isRegister,Action action)
		{
			if (isRegister)
			{
				OnFixedUpdateEventHandler += action;
			}
			else
			{
				OnFixedUpdateEventHandler -= action;
			}
		}
		public void ExitRegister (bool isRegister,Action action)
		{
			if (isRegister)
			{
				OnExitEventHandler += action;
			}
			else
			{
				OnExitEventHandler -= action;
			}
		}


	}
	public class CountdownState : BaseGameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			Debug.Log("CountdownState Update");
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
	public class PlayingState : BaseGameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
	public class PausedState : BaseGameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
	public class GameOverState : BaseGameState
	{
		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}

	public interface IStateFunction
	{
		public void OnRegister()
		{
			Register(true);
		}

		public void OnDeregister()
		{
			Register(false);
		}

		public void Register (bool flag);
	}
}
