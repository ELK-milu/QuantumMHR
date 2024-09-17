using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSessionController : MonoBehaviour
{
	private void Update()
	{
		JudgeState();
		GameStateMachine.Instance.Update();
	}

	private void JudgeState()
	{
		if (Utils.TryGetQuantumFrame(out Frame frame))
		{
			if (frame.TryGetSingletonEntityRef<GameSession>(out var entity) == false)
			{
				GameStateMachine.Instance.IsActive = false;
				return;
			}
			GameStateMachine.Instance.IsActive = true;
			var gameSession = frame.GetSingleton<GameSession>();
			GameStateMachine.Instance.SetSession(gameSession);
		}
		else
		{
			GameStateMachine.Instance.IsActive = false;
		}
	}
}
