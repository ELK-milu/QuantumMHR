using StatePattern.StateSystem;
using UnityEngine;

public abstract class CharacterStateMachine
{
	public StateMachine _stateMachine;
	public Animator _animator;
	public abstract void SetStateMachine(PlayerController playerController,StateMachine stateMachine,Animator animator);
}