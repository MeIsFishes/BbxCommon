using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public abstract class HudViewBase : UiViewBase
    {
        [SerializeField]
        [FoldoutGroup("HUD")]
        internal bool AutoUpdatePos;
        [SerializeField]
        [FoldoutGroup("HUD"), ShowIf("AutoUpdatePos"), Tooltip("The offset relative to the entity it is bound with.")]
        internal Vector3 HudOffset;
    }
}
