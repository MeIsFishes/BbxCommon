using UnityEngine;

namespace BbxCommon
{
    public enum EContextType
    {
        None,
        Test,
        Buff,
        Action,
        ActionMix,
        Projectile,
    }

    public abstract class TaskTimelineContextBase : PooledObject
    {
        public EContextType Type;
        public GameObject Caster;
        public GameObject Target;

        protected abstract void InitContextType();

        public TaskTimelineContextBase()
        {
            InitContextType();
        }
    }
}
