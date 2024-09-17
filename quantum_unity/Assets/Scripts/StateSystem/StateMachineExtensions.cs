using StatePattern.StateSystem;
using System;

public static class StateMachineExtensions
{
	/// <summary>
	/// 此方法请在所有状态添加后使用，否则后加入的状态类型无法添加转换
	/// </summary>
	/// <param name="stateMachine"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="condition"></param>
	/// <returns></returns>
	public static StateMachine At(this StateMachine stateMachine, Type from, IState to, IPredicate condition)
	{
		stateMachine.AddTransition(stateMachine.GetNodeStates(from), to, condition);
		return stateMachine;
	}
	
	public static StateMachine At(this StateMachine stateMachine, Type from, Type to, IPredicate condition)
	{
		stateMachine.AddTransition(stateMachine.GetNodeStates(from), stateMachine.GetNodeStates(to), condition);
		return stateMachine;
	}
	public static StateMachine At(this StateMachine stateMachine, IState from, IState to, IPredicate condition)
	{
		stateMachine.AddTransition(from, to, condition);
		return stateMachine;
	}
	public static StateMachine Any(this StateMachine stateMachine, IState to, IPredicate condition)
	{
		stateMachine.AddAnyTransition(to, condition);
		return stateMachine;
	}
}
