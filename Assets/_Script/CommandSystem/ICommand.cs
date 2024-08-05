namespace CommandSystem
{
    /// <summary>
    /// Abstract command definition - must include Undo operation
    /// </summary>
    public interface ICommand
    {
        void Execute();
        bool CanExecute();
        void Undo();
    }
}