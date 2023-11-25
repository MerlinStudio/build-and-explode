using UnityEngine;

namespace Common.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(ShopConfig), fileName = nameof(ShopConfig))]

    public class ShopConfig : ScriptableObject
    {
        [SerializeField] private int m_buyBombForAd;
        
        public int BuyBombForAd => m_buyBombForAd;
    }
}