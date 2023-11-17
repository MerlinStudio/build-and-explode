using Data.Builds.Blocks;

namespace Model.Creator.Interfaces
{
    public interface IBuildProvider
    {
        BlockViewInfo[] GetBlockViewInfo();
        BlockPropertyInfo[] GetBlockPropertyInfo();
    }
}