using Godot;
using System;
using System.IO;

namespace BbxCommon
{
	public class EditorSettings : Singleton<EditorSettings>
	{
		public string TaskInfoPath;

		public void Save()
		{
			JsonApi.Serialize(this, Path.GetFullPath("EditorSettings.json"));
		}

        protected override void OnSingletonInit()
        {
			if (JsonApi.TryDeserialize(Path.GetFullPath("EditorSettings.json"), this) == false)
			{
				DebugApi.Log("Doesn't exist EditorSettings.json, created a new one.");
			}
        }
    }
}
