namespace Game.Data.Models
{
    public class GameDataModel : DataModel<GameData>, IAutoBindDataModel
    {
        public int PastLevelNumber
        {
            get => Data.PastLevelNumber;
            set => Data.PastLevelNumber = value;
        }

        public LevelResultData LevelResultData { get; set; }

        protected override void OnDataLoaded()
        {
            base.OnDataLoaded();
        }
    }
}