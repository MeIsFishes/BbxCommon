using Godot;
using System;

namespace BbxCommon
{
    public partial class TimelineCondition : TaskNode
	{
		[Export]
		public ColorRect Border;
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
            NodeButton.AddThemeStyleboxOverride("hover", new StyleBoxFlat { BgColor = hoverColor });
            NodeButton.AddThemeStyleboxOverride("normal", new StyleBoxFlat { BgColor = normalColor });
            NodeButton.AddThemeStyleboxOverride("pressed", new StyleBoxFlat { BgColor = pressedColor });
        }

        public override void OnTaskSelected(bool selected)
        {
            if (selected)
			{
				Border.Color = new Color(0xe9cb00ff);
			}
			else
			{
				Border.Color = new Color(0, 0, 0);
			}
        }
    }
}
