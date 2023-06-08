using System.Collections.Generic;
using BbxCommon.Ui;

namespace Dcg
{
    public class UiCardsModel : UiModelBase
    {
        public List<UiModelVariable<Dice>> DiceDeck = new();

        public override void OnAllocate()
        {
            
        }
    }
}