namespace Common.Saves.Interfaces
{
    public interface ISavesProvider
    {
        void SetSavesData<T>(int saveValue) where T : ISaves;
        int GetSavesData<T>() where T : ISaves;
    }
}