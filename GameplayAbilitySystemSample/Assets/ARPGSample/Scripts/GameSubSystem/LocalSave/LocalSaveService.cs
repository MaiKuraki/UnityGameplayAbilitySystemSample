namespace ARPGSample.GameSubSystem
{
    public interface ILocalSave
    {
        bool SaveDataValid { get; }
    }
    public class LocalSaveService : ILocalSave
    {
        private bool bSaveDataValid;
        public bool SaveDataValid => bSaveDataValid;
    }
}