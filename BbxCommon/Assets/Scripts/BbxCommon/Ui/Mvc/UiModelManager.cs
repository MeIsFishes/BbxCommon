using System.Collections.Generic;

namespace BbxCommon.Ui
{
    internal static class UiModelManager
    {
        private static List<UiModelBase> m_UiModels = new();

        internal static void AddUiModel<T>() where T : UiModelBase, new()
        {
            var model = ObjectPool<T>.Alloc();
            var typeId = ClassTypeId<UiModelBase, T>.Id;
            m_UiModels.ModifyCount(typeId + 1);
            m_UiModels[typeId] = model;
        }

        internal static T GetUiModel<T>() where T : UiModelBase
        {
            var typeId = ClassTypeId<UiModelBase, T>.Id;
            if (m_UiModels.Count <= typeId + 1)
                return null;
            return (T)m_UiModels[typeId];
        }

        internal static void RemoveUiModel<T>() where T : UiModelBase
        {
            var typeId = ClassTypeId<UiModelBase, T>.Id;
            if (m_UiModels.Count <= typeId + 1)
                return;
            m_UiModels[typeId].CollectToPool();
            m_UiModels[typeId] = null;
        }
    }
}
