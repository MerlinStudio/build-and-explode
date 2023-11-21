using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Model.Creator.Interfaces;

namespace Model.Creator.Creators
{
    public class ManagerCreator : IManagerCreator
    {
        private List<ICreator> m_creators;

        public void Init(params ICreator[] creators)
        {
            m_creators = new List<ICreator>(creators);
            m_creators.ForEach(c => c.Init());
        }

        public void DeInit()
        {
            m_creators.ForEach(c => c.DeInit());
            m_creators.Clear();
        }

        public void AddCreator(ICreator creator)
        {
            if (!m_creators.Contains(creator))
            {
                m_creators.Add(creator);
            }
        }
        
        public UniTask<TItem> Create<TItem, TCreator>(string id) where TCreator : ICreator where TItem : CreatedItem
        {
            var creator = m_creators.Find(c => c is TCreator);
            return creator.Create<TItem>(id);
        }
    }
}