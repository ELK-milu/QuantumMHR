namespace StatePattern.StateSystem
{
	public interface IState
	{
		void OnEnter();
		void Update();
		void FixedUpdate();
		void LaterUpdate();
		void OnExit();
	}

}