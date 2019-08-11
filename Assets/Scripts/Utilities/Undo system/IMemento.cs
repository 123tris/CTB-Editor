namespace RuntimeUndo
{
    public interface IMemento<out T>
    {
        T Revert();
    }
}