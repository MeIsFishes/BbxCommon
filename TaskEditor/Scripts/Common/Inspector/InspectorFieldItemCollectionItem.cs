using BbxCommon.Internal;
using Godot;
using System;

namespace BbxCommon
{
	public partial class InspectorFieldItemCollectionItem : Control
	{
		[Export]
		public LineEdit Value1Edit;
		[Export]
		public OptionButton Value1Option;
		[Export]
		public LineEdit Value2Edit;
		[Export]
		public OptionButton Value2Option;
		[Export]
		public Button BtnAdd;
		[Export]
		public Button BtnRemove;

		private InspectorFieldItem m_FieldItemBelongs;

		private enum ECollection
		{
			List,
			Dictionary,
		}
		private ECollection m_CollectionType;

        public override void _Ready()
        {
			BtnAdd.Pressed += OnBtnAddPressed;
			BtnRemove.Pressed += OnBtnRemovePressed;
        }

		public string GetValue1()
		{
			if (Value1Edit.Visible == true)
				return Value1Edit.Text;
			if (Value1Option.Visible == true)
				return Value1Option.Text;
			return "";
		}

		public string GetValue2()
		{
			if (Value2Edit.Visible == true)
				return Value2Edit.Text;
			if (Value2Option.Visible == true)
				return Value2Option.Text;
			return "";
		}

        public void InitList(InspectorFieldItem fieldItem, TaskExportTypeInfo typeInfo, string value)
		{
			m_CollectionType = ECollection.List;
			m_FieldItemBelongs = fieldItem;
			if (TaskUtils.IsEnum(typeInfo.GenericType1))
			{
                Value1Edit.Visible = false;
                Value1Option.Visible = false;
                Value2Edit.Visible = false;
                Value2Option.Visible = true;
				TaskUtils.SetEnumPresetValues(Value2Option, TaskUtils.GetEnumInfo(typeInfo.GenericType1));
				Value2Option.Select(value);
            }
			else
			{
				Value1Edit.Visible = false;
				Value1Option.Visible = false;
				Value2Edit.Visible = true;
				Value2Option.Visible = false;
				Value2Edit.Text = value;
			}
		}

		private void OnBtnAddPressed()
		{
			var parent = GetParent();
			var index = parent.GetChildren().IndexOf(this) + 1;
			m_FieldItemBelongs.InsertCollectionItem(index, GetValue1(), GetValue2());
		}

		private void OnBtnRemovePressed()
		{
            var parent = GetParent();
            m_FieldItemBelongs.RemoveCollectionItem(this);
        }
	}
}
