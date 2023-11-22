using Cysharp.Threading.Tasks;

namespace State.SavaLoader.Interfaces
{
    public interface ISaveConstructionController
    {
        UniTask Construction();
        bool CheckConstructionProgress();
    }
}