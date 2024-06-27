using System;
using Unity.Entities;

namespace BbxCommon
{
    public class EntityIDComponent :EcsRawComponent
    {
        public EntityID EntityUniqueID;
    }

    public struct EntityID: IEquatable<EntityID>
    {
        private readonly ulong m_EntityID;
        public static EntityID INVALID => new EntityID();

        private EntityID(ulong id)
        {
            m_EntityID = id;
        }

        public static implicit operator ulong(EntityID id)
        {
            return id.m_EntityID;
        }
        
        public static implicit operator EntityID(ulong value)
        {
            EntityID id = new EntityID(value);
            return id;
        }
        
        public static bool operator ==(EntityID a, EntityID b)
        {
            return a.m_EntityID == b.m_EntityID;
        }
        
        public static bool operator !=(EntityID a, EntityID b)
        {
            return a.m_EntityID != b.m_EntityID;
        }
        
        public bool Equals(EntityID other)
        {
            return m_EntityID == other.m_EntityID;
        }
      
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(EntityID))
                return false;

            EntityID id = (EntityID)obj;
            return id.m_EntityID == this.m_EntityID;
        }
        
        public override int GetHashCode()
        {
            return m_EntityID.GetHashCode();
        }
        
    }
    
}