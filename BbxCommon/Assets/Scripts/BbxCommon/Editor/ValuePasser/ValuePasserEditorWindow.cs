using System;
using System.Text;
using UnityEditor;

namespace BbxCommon.ValuePasserInternal
{
    public class ValuePasserEditorWindow : EditorWindow
    {
        public static void ExportWriter(Type classType, string path)
        {
            var writerClass = new StringBuilder();
            var objectCreaterFunc = new StringBuilder();

            writerClass.Append("using BbxCommon;\n");
            writerClass.Append("using BbxCommon.ValuePasserInternal;\n");
            if (classType.Namespace != "BbxCommon" && classType.Namespace != "BbxCommon.ValuePasserInternal")
                writerClass.Append("using " + classType.Namespace + ";\n");
            writerClass.Append("\n");
            writerClass.Append("namespace ExportedFile\n");
            writerClass.Append("{\n");
            writerClass.Append(InsertTab(1) + "public class " + classType.Name + "ValueWriter\n");
            writerClass.Append(InsertTab(1) + "{\n");
            writerClass.Append(InsertTab(2) + "public " + classType.Name + " WriterReference;\n");
            writerClass.Append("\n");

            objectCreaterFunc.Append(InsertTab(2) + "public static ValuePasser WriteToValuePasser(ValuePasser valuePasser)\n");
            objectCreaterFunc.Append(InsertTab(2) + "{\n");
            foreach (var member in classType.GetMembers())
            {
                var attributes = member.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is VariableWriterMemberAttribute writerAttribute)
                    {
                        writerAttribute.MemberType = member.ReflectedType;
                        writerAttribute.MemberName = member.Name;
                        if (ValuePasserEditorDatabase.VariableDatas.ContainsKey(member.ReflectedType))
                        {
                            var variableDataType = ValuePasserEditorDatabase.VariableDatas[member.ReflectedType];
                            objectCreaterFunc.Append(InsertTab(3) + "valuePasser.VariablePasser[" + member.Name + "] as " + variableDataType.Name + ".SetValue(writer." + member.Name + ");\n");
                        }
                    }
                }
            }
        }

        private static string InsertTab(int count)
        {
            string res = "";
            for (int i = 0; i < count; i++)
            {
                res += "    ";
            }
            return res;
        }
    }
}
