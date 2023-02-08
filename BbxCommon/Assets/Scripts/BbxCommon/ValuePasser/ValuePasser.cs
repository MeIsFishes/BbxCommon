using System.Collections.Generic;
using BbxCommon.ValuePasserInternal;

namespace BbxCommon
{
    public class ValuePasser : PooledObject
    {
        public Dictionary<string, NumberData> NumberDic = new Dictionary<string, NumberData>();
        public Dictionary<string, object> ObjectDic = new Dictionary<string, object>();
        public VariablePasser VariablePasser = new VariablePasser();
    }
}
