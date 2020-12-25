using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WildlifeTabAlt
{
    public class MainTabWindow_WildlifeAlt : MainTabWindow_GroupedPawnTable
    {
        private static PawnTableDef pawnTableDef;
        protected override PawnTableDef PawnTableDef
        {
            get
            {
                return pawnTableDef ?? (pawnTableDef = DefDatabase<PawnTableDef>.GetNamed("WildlifeGrouped"));
            }
        }

        private static PawnTableGroupDef groupDef;
        protected override PawnTableGroupDef GroupDef
        {
            get
            {
                return groupDef ?? (groupDef = DefDatabase<PawnTableGroupDef>.GetNamed("WildlifeGrouped"));
            }
        }

        // Token: 0x17001183 RID: 4483
        // (get) Token: 0x0600615C RID: 24924 RVA: 0x0021EAF1 File Offset: 0x0021CCF1
        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                return Find.CurrentMap.mapPawns.AllPawns.Where((Pawn p) => p.Spawned && (p.Faction == null || p.Faction == Faction.OfInsects) && p.AnimalOrWildMan() && !p.Position.Fogged(p.Map) && !p.IsPrisonerInPrisonCell());
            }
        }

        // Token: 0x0600615D RID: 24925 RVA: 0x00218862 File Offset: 0x00216A62
        public override void PostOpen()
        {
            base.PostOpen();
            Find.World.renderer.wantedMode = WorldRenderMode.None;
        }
    }
}
