using Quantum;

public interface IEntityRegister
{
	EntityRef _entityRef { get; }
	Frame Frame { get; }
	void SetFrame(Frame frame);
	void SetRef(EntityRef entityRef);
	void DispatchRef();
}
