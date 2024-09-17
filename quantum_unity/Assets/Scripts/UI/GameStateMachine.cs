using Quantum;
using StatePattern.StateSystem;
using System;
using UnityEngine;

public class GameStateMachine
{
	protected static GameStateMachine instance;
	public static GameStateMachine Instance
	{
		get{
            if(instance == null){
	            instance = new GameStateMachine();
            }
            return instance;
        }
	}
	public StateMachine StateMachine { get; private set; }
	public bool IsActive;
	private GameSession _gameSession;

	public GameStateMachine()
	{
		foreach (GameState state in Enum.GetValues(typeof(GameState)))
		{
			if (StateMachine == null)
			{
				StateMachine = new StateMachine();
			}
			SetState(state);
		}
	}

	public void SetState (GameState state)
	{
		switch (state)
		{
			case GameState.Countdown:
				StateMachine.SetState(new CountdownState());
				break;
			case  GameState.GameOver:
				StateMachine.SetState(new GameOverState());
				break;
			case GameState.Paused:
				StateMachine.SetState(new PausedState());
				break;
			case GameState.Playing:
				StateMachine.SetState(new PlayingState());
				break;
			default:
				throw new ArgumentOutOfRangeException("state", state, null);
		}
	}
	public void SetSession (GameSession session)
	{
		_gameSession = session;
		switch (session.State)
		{
			case GameState.Countdown:
				StateMachine.ChangeState(new CountdownState());
				break;
			case  GameState.GameOver:
				StateMachine.ChangeState(new GameOverState());
				break;
			case GameState.Paused:
				StateMachine.ChangeState(new PausedState());
				break;
			case GameState.Playing:
				StateMachine.ChangeState(new PlayingState());
				break;
			default:
				throw new ArgumentOutOfRangeException("state", session.State, null);
		}
	}
	public GameSession GetSession ()
	{
		return _gameSession;
	}
	public void ChangeState (GameState state)
	{
		switch (state)
		{
			case GameState.Countdown:
				StateMachine.ChangeState(new CountdownState());
				break;
			case  GameState.GameOver:
				StateMachine.ChangeState(new GameOverState());
				break;
			case GameState.Paused:
				StateMachine.ChangeState(new PausedState());
				break;
			case GameState.Playing:
				StateMachine.ChangeState(new PlayingState());
				break;
			default:
				throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	public void Update()
	{
		if (!IsActive)
		{
			return;
		}
		StateMachine.Update();
	}
	public BaseGameState GetCurrentState()
	{
		return StateMachine.CurrentState.State as BaseGameState;
	}
	
	
}
