using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIBase.Enums
{
    public enum EffectTag
    {
        AddFromDeckToHand
    }

    public enum SuperEffectTag
    {

    }

    public static class EffectTagExtentions
    {
        public static IList<SuperEffectTag> GetSuperTags(this EffectTag t)
        {
            return new List<SuperEffectTag>();
        }

        public static IList<EffectTag> GetSubTags(this SuperEffectTag t)
        {
            return new List<EffectTag>();
        }
    }
}
