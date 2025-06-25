using Godot;
using System;

namespace BbxCommon
{
    public partial class TimelineCondition : TaskNode
	{
		[Export]
        public BbxButton ConditionButton;
        [Export]
		public Color EnterConditionColor;
		[Export]
		public Color ConditionColor;
		[Export]
		public Color ExitConditionColor;

		private EConditionType m_ConditionType;

		public void SetConditionType(EConditionType conditionType)
		{
			m_ConditionType = conditionType;
			Color color = default;
			switch (m_ConditionType)
			{
				case EConditionType.EnterCondition:
					color = EnterConditionColor;
					break;
                case EConditionType.Condition:
                    color = ConditionColor;
                    break;
                case EConditionType.ExitCondition:
                    color = ExitConditionColor;
                    break;
            }
			var hoverColor = color.Lightened(0.2f);
			var normalColor = color;
			var pressedColor = color.Darkened(0.2f);
			ConditionButton.AddThemeStyleboxOverride("Hover", new StyleBoxFlat{ BgColor = hoverColor });
            ConditionButton.AddThemeStyleboxOverride("Normal", new StyleBoxFlat { BgColor = normalColor });
            ConditionButton.AddThemeStyleboxOverride("Pressed", new StyleBoxFlat { BgColor = pressedColor });
        }
    }
}
