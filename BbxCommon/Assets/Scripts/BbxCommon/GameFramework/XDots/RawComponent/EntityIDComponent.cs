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
        private string m_District;
        public static EntityID INVALID => new EntityID();

        public EntityID(ulong id, string district)
        {
            m_ID = id;
            m_District = district;
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

        public string GetDistrict()
        {
            return m_District;
        }
        
        public static bool operator ==(EntityID a, EntityID b)
        {
            return a.m_ID == b.m_ID 
                   && a.m_District == b.m_District;
        }
        
        public static bool operator !=(EntityID a, EntityID b)
        {
            return a.m_ID != b.m_ID 
                   || a.m_District != b.m_District;
        }
        
        public bool Equals(EntityID other)
        {
            return m_ID == other.m_ID 
                   && m_District == other.m_District;
        }
      
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(EntityID))
                return false;

            EntityID id = (EntityID)obj;
            return id.m_ID == m_ID 
                   && id.m_District == m_District;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(m_ID,m_District);
        }
        
    }
    
}