namespace State.Creator.Interfaces
{
    public interface IInitializerCreator
    {
        void Init(params ICreator[] creators);
        void DeInit();
    }
}