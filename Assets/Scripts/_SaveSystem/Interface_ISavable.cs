namespace _SaveSystem
{
    public interface ISaveable
    {
        object CaptureState();

        void RestoreState(object _state);
    }
}