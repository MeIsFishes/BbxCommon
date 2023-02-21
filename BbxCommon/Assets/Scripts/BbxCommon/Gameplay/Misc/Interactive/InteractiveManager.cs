using System.Collections.Generic;

namespace BbxCommon
{
    public class InteractiveManager : Singleton<InteractiveManager>
    {
        private Dictionary<int, HashSet<int>> m_InteractiveDatas = new Dictionary<int, HashSet<int>>();
    }
}
