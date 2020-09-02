using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeacefulProtests
{
    [DefOf]
    public static class PeacefulFactionDefOf
    {
        static PeacefulFactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
        }

        public static FactionDef PeacefulProtesters;
    }

}
