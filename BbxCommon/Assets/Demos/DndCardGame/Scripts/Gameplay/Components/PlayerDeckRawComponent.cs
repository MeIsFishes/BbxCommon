using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class PlayerDeckRawComponent : EcsRawComponent
    {
        public List<Dice> Dices = new();
    }
}
