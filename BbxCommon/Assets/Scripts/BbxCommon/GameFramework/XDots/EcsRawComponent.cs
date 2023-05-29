
namespace BbxCommon
{
    internal static class EcsRawComponentId
    {
        internal static int CurId;
    }

    internal static class EcsRawComponentId<T> where T : EcsRawComponent
    {
        private static bool Inited;
        private static int m_Id;
        internal static int Id  // auto register id
        {
            get
            {
                if (Inited)
                    return m_Id;
                else
                {
                    m_Id = EcsRawComponentId.CurId++;
                    Inited = true;
                    return m_Id;
                }
            }
        }
    }

    public abstract class EcsRawComponent : EcsData
    {
        
    }

    public abstract class EcsSingletonRawComponent : EcsRawComponent, IEcsSingletonData
    {

    }
}
