using Data.Builds.Blocks;

namespace Model.Creator.Interfaces
{
    public interface IBlocksInfoProvider
    {
        BlockViewInfo[] GetBlockViewInfo();
        BlockPropertyInfo[] GetBlockPropertyInfo();
    }
}