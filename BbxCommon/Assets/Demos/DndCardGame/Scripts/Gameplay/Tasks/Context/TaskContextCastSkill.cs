using System;
using System.Collections.Generic;
using BbxCommon;
using Unity.Entities;

namespace Dcg
{
    public class TaskContextCastSkill : TaskContextBase
    {
        public EntityID AttackerEntityId;
        public EntityID TargetEntityId;

        public List<Dice> WildDices;

        public enum EField
        {
            AttackerEntityId,
            TargetEntityId,
            WildDices
        }

        public override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterObject(EField.AttackerEntityId, AttackerEntityId);
            RegisterObject(EField.TargetEntityId, TargetEntityId);
            RegisterObject(EField.WildDices, WildDices);
        }

        public override void OnCollect()
        {
            AttackerEntityId = EntityID.INVALID;
            TargetEntityId = EntityID.INVALID;
            WildDices?.Clear();
        }
    }
}
