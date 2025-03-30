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
        private readonly ulong m_ID;
        private string m_Group;
        public static EntityID INVALID => new EntityID();

        public EntityID(ulong id, string group)
        {
            m_ID = id;
            m_Group = group;
        }

        // public static implicit operator ulong(EntityID id)
        // {
        //     return id.m_EntityID;
        // }
        //
        // public static implicit operator EntityID(ulong value)
        // {
        //     EntityID id = new EntityID(value);
        //     return id;
        // }

        public string GetGroup()
        {
            return m_Group;
        }
        
        public static bool operator ==(EntityID a, EntityID b)
        {
            return a.m_ID == b.m_ID 
                   && a.m_Group == b.m_Group;
        }
        
        public static bool operator !=(EntityID a, EntityID b)
        {
            return a.m_ID != b.m_ID 
                   || a.m_Group != b.m_Group;
        }
        
        public bool Equals(EntityID other)
        {
            return m_ID == other.m_ID 
                   && m_Group == other.m_Group;
        }
      
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(EntityID))
                return false;

            EntityID id = (EntityID)obj;
            return id.m_ID == m_ID 
                   && id.m_Group == m_Group;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(m_ID,m_Group);
        }
        
    }
    
}