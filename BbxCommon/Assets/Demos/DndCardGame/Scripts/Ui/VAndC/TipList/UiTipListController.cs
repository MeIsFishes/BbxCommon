using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.Networking;

namespace Dcg.Ui
{
    public class UiTipListController : UiControllerBase<UiTipListView>
    {
        protected override void OnUiInit()
        {
            var itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("游戏基本信息",
                "游戏采用DND（龙与地下城）基本战斗规则为基础，在地图中点击“下一个房间”选项即可进入下一个房间，" +
                "并遭遇随机怪物。战斗中右上角可选择切换使用剑或匕首，然后点击攻击，即可拖入自由骰。自由骰填充完毕" +
                "后，即可确定攻击。自由骰的填充顺序与文本描述顺序一致，如对于武器剑，第一个自由骰是攻击骰，第二个" +
                "是伤害骰。\n" +
                "在战斗场景下点击抽牌堆和弃牌堆可以查看牌堆情况。");
            itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("攻击结算流程",
                "每次攻击时，会首先投掷攻击骰，对抗对方的AC（护甲等级）值，若攻击骰不小于对方AC，则判定为命中。" +
                "只有当攻击命中时，才会结算伤害，此时使用伤害骰来造成伤害。在构建攻击骰、伤害骰、AC骰时，除基础" +
                "骰子外，还会加入buff和属性调整值。在DND原版规则中，属性调整值为固定值，而本游戏将属性调整值也变成" +
                "了骰子，计算公式为随着属性的提高，附加d4、d6、d8、d10、d12的属性调整骰，若属性值大于5，则直接附赠" +
                "一个d12，然后剩余值递归结算。同时，AC值算法也由固定值改为了随机值，这是为了防止像原版一样超高AC" +
                "的高达战士。");
            itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("暴击伤害",
                "目前的设定是当攻击骰的结果达到攻击对象AC骰结果2.5倍时触发暴击。暴击时会将伤害骰的基础骰部分投掷" +
                "两份，如伤害骰的基础骰原本是1d6+1d4，暴击后会变为2d6+2d4。");
            itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("战斗结算",
                "战斗胜利后会随机生成3个骰子，选择一个加入自己的骰子池。此处也可以直接跳过。战斗失败后会直接重新开始" +
                "游戏，没有做重新开始的结算界面是因为工时比较赶，而且这种花边功能感觉做了大概后面也要删的=。=");
            itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("游戏难度",
                "目前提供2种敌人，力量型的拦路强盗和敏捷型的女刺客。实战下来女刺客似乎会强一些。目前游戏没有递增的" +
                "难度系统，不管战斗奖励还是敌人都是在公共池里面完全随机，所以可能会出现开局暴毙或者拿到好骰子后一路" +
                "平推的情况。如果出现骰子成型后无敌，想要重新体验可能需要自杀一下。");
            itemController = m_View.UiList.ItemWrapper.AddItem<UiTipListItemController>();
            itemController.Init("其他事项说明",
                "这个demo主要展示最核心的战斗玩法以及与DND原版的主要区别，开发过程中以搭建底层管线为主，很多操作上" +
                "的优化不必苛责，也不保证胡乱操作是否会出bug。最终成品会包含更多的元素，包括但不限于多个战役、原版职业、" +
                "多样的施法方式、骰子词缀等。目前针对这些新功能已有技术解决方案。");
        }
    }
}
