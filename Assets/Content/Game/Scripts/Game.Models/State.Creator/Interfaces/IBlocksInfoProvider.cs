using Data.Builds.Blocks;

namespace State.Creator.Interfaces
{
    public interface IBlocksInfoProvider
    {
        BlockViewInfo[] GetBlockViewInfo();
        BlockPropertyInfo[] GetBlockPropertyInfo();
    }
}