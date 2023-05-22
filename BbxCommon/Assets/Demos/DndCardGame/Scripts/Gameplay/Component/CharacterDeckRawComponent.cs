using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class CharacterDeckRawComponent : EcsRawComponent
    {
        public List<Dice> Dices = new();
    }
}
