using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using StatePattern.StateSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour,IStateFunction
{
	[SerializeField]
	TextMeshProUGUI countDownText;
	
	public event Action OnCountDownUpdateHandler = delegate {  };
	public event Action OnPlayingUpdateHandler = delegate {  };
	public event Action OnGameOverUpdateHandler = delegate {  };

	private void OnEnable()
	{
		OnRegister();
	}
	
	private void OnDisable()
	{
		OnDeregister();
	}

	private void Update()
	{
		if (!GameStateMachine.Instance.IsActive)
		{
			countDownText.text = "GameSession singleton not found";
		}
	}

	
	#region 事件注册
	public void OnRegister ()
	{
		Register(true);
	}

	public void OnDeregister()
	{
		Register(false);
	}

	public void Register (bool flag)
	{
		var countdownState = GameStateMachine.Instance.StateMachine.GetNodeState(typeof(CountdownState)) as CountdownState;
		var playingState = GameStateMachine.Instance.StateMachine.GetNodeState(typeof(PlayingState)) as PlayingState;
		var gameOverState = GameStateMachine.Instance.StateMachine.GetNodeState(typeof(GameOverState)) as GameOverState;
		countdownState.UpdateRegister(flag,OnCountDownUpdate);
		playingState.UpdateRegister(flag,OnPlayingUpdate);
		gameOverState.UpdateRegister(flag,OnGameOverUpdate);
	}

	public void OnCountDownUpdate()
	{
		OnCountDownUpdateHandler?.Invoke();
		countDownText.gameObject.SetActive(true);
		int countDown = (int)GameStateMachine.Instance.GetSession().TimeUntilStart;
		countDownText.text = $"{countDown}";
	}
	
	public void OnPlayingUpdate()
	{
		//StartCoroutine(DoText());
		OnPlayingUpdateHandler?.Invoke();
		countDownText.gameObject.SetActive(false);
	}

	IEnumerator DoText()
	{
		countDownText.text = "go";
		yield return new WaitForSeconds(1f);
		countDownText.gameObject.SetActive(false);
	}

	public void OnGameOverUpdate()
	{
		OnGameOverUpdateHandler?.Invoke();
		countDownText.gameObject.SetActive(true);
		countDownText.text = "Game Over";
	}
	#endregion


}