using BbxCommon.Ui;
using System.Collections.Generic;

namespace BbxCommon
{
    internal class HudRawComponent : EcsRawComponent
    {
        internal List<IHudController> HudControllers = new();

        internal void AddHudController<T>(T hudController) where T : UiControllerBase, IHudController
        {
            var typeId = ClassTypeId<IHudController, T>.Id;
            if (typeId >= HudControllers.Count)
                HudControllers.ModifyCount(typeId + 1);
            HudControllers[typeId] = hudController;
        }

        internal T GetHudController<T>() where T : UiControllerBase, IHudController
        {
            var typeId = ClassTypeId<IHudController, T>.Id;
            if (typeId >= HudControllers.Count)
                return null;
            return HudControllers[typeId] as T;
        }

        internal void RemoveHudController<T>() where T : UiControllerBase, IHudController
        {
            var typeId = ClassTypeId<IHudController, T>.Id;
            if (typeId < HudControllers.Count)
                HudControllers[typeId] = null;
        }

        internal void ClearHudControllers()
        {
            for (int i = 0; i < HudControllers.Count; i++)
                HudControllers[i] = null;
        }

        public override void OnCollect()
        {
            HudControllers.Clear();
        }
    }
}
