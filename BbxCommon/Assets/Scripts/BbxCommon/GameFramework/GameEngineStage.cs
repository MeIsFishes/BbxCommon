using BbxCommon.Internal;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace BbxCommon
{
    public partial class GameEngineBase<TEngine>
    {
        private GameStage CreateGameEngineStage()
        {
            var stage = StageWrapper.CreateStage("Game Engine Stage");

            stage.AddDataGroup("GameEngineDefault");

            stage.AddLoadItem<InitReflectionAndResource>();

            stage.AddGameEngineEarlyUpdateSystem<InputSystem>();

            stage.AddGameEngineLateUpdateSystem<TaskSystem>();

            return stage;
        }

        private class InitReflectionAndResource : IStageLoad
        {
            public void Load(GameStage stage)
            {
                // reflect types
                foreach (var type in ReflectionApi.GetAllTypesEnumerator())
                {
                    if (type.IsAbstract == false && type.IsSubclassOf(typeof(CsvDataBase)))
                    {
                        var constructor = type.GetConstructor(Type.EmptyTypes);
                        var csvObj = (CsvDataBase)constructor.Invoke(null);
                        var dataGroup = csvObj.GetDataGroup();
                        if (dataGroup != null)
                        {
                            if (ResourceApi.DataGroupCsvPairs.ContainsKey(dataGroup) == false)
                                ResourceApi.DataGroupCsvPairs[dataGroup] = new();
                            ResourceApi.DataGroupCsvPairs[dataGroup].Add(csvObj);
                        }
                    }
                }
                // init resource
                ResourceManager.Init();
                DebugApi.Log(ResourceManager.ToString());
            }

            public void Unload(GameStage stage)
            {
                
            }
        }
    }
}
