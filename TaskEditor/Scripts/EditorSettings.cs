using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace BbxCommon
{
	public class EditorSettings : Singleton<EditorSettings>
	{
		private string m_ExportInfoPath;
		public string ExportInfoPath
		{
			get => m_ExportInfoPath;
			set
			{
				if (m_ExportInfoPath != value)
				{
					m_ExportInfoPath = value;
					Save();
				}
			}
		}

		private string m_LastSaveTargetPath;
		public string LastSaveTargetPath
		{
			get => m_LastSaveTargetPath;
			set
			{
				if (m_LastSaveTargetPath != value)
				{
					m_LastSaveTargetPath = value;
					Save();
				}
			}
		}

        private void Save()
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
