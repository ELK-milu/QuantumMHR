namespace Quantum.Interface
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
