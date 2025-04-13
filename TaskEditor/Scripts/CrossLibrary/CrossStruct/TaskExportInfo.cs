using System;
using System.Collections.Generic;

namespace BbxCommon.Internal
{
    public class TaskExportCrossVariable
    {
        public static string ListElementSplit = "%||%";
    }

    // List<int> will be stored as:
    // TypeName: "List"
    // GenericType1: "int"
    // GenericType2: null

    // Dictionary<string, int> will be stored as:
    // TypeName: "Dictionary"
    // GenericType1: "string"
    // GenericType2: "int"
    public class TaskExportTypeInfo
    {
        public string TypeName;
        public TaskExportTypeInfo GenericType1;
        public TaskExportTypeInfo GenericType2;

		public TaskExportTypeInfo() { }

		public TaskExportTypeInfo(string typeName)
		{
			TypeName = typeName;
		}

		public bool CheckMeet(TaskExportTypeInfo target)
		{
			if (target == null)
				return false;
            if (TypeName != target.TypeName)
				return false;
			if (GenericType1 == null && target.GenericType1 != null)
				return false;
			if (GenericType1 != null && GenericType1.CheckMeet(target.GenericType1) == false)
				return false;
            if (GenericType2 == null && target.GenericType2 != null)
                return false;
            if (GenericType2 != null && GenericType2.CheckMeet(target.GenericType2) == false)
                return false;
            return true;
        }
	}

	public class TaskExportFieldInfo
	{
		public string FieldName;
		public TaskExportTypeInfo TypeInfo;
	}

	public class TaskExportInfo
	{
		public string TaskTypeName;
		public List<TaskExportFieldInfo> FieldInfos = new();
	}

	public class TaskContextExportInfo
	{
		public string TaskContextTypeName;
		public List<TaskExportFieldInfo> FieldInfos = new();
	}

	public class TaskEnumExportInfo
	{
		public string EnumTypeName;
		public List<string> EnumValues = new();

		public void GenerateInfo(Type type)
		{
			EnumTypeName = type.FullName;
			EnumValues.AddRange(type.GetEnumNames());
		}
	}
}
