using System.Collections.Generic;
using UnityEngine;
using BbxCommon;

namespace Dcg
{
    public interface IEngineLoadingItem
    {
        void Load();
        void Unload();
    }

    public class DcgGameEngine : MonoSingleton<DcgGameEngine>
    {
        public List<IEngineLoadingItem> LoadingItems = new List<IEngineLoadingItem>();

        protected void Awake()
        {
            base.Awake();
            var interactingDataAsset = Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset");
            interactingDataAsset.ApplyInteractingData();
        }
    }
}
