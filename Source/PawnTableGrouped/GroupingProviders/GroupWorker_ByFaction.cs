using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
	class GroupWorker_ByFaction : SortingGroupWorker
	{
        class PawnComparer : IEqualityComparer<Pawn>
        {
            public bool Equals(Pawn x, Pawn y)
            {
                var xFaction = GetPawnFaction(x);
                var yFaction = GetPawnFaction(y);

				if (xFaction == null && yFaction == null)
				{
					var xLabel = GetPawnLabel(x);
					var yLabel = GetPawnLabel(y);
					return EqualityComparer<string>.Default.Equals(xLabel, yLabel);
				}
				else
				{
					return EqualityComparer<Faction>.Default.Equals(xFaction, yFaction);
				}
            }

            public int GetHashCode(Pawn obj)
			{
				var faction = GetPawnFaction(obj);
				if (faction != null)
				{
					return EqualityComparer<Faction>.Default.GetHashCode(faction);
				} 
				else
				{
					var label = GetPawnLabel(obj);
					return EqualityComparer<string>.Default.GetHashCode(label);
				}
            }
        }

        class GroupComparer : IComparer<PawnTableGroup>
        {
			public int Compare(PawnTableGroup x, PawnTableGroup y)
			{
				var xFaction = GetPawnFaction(x.KeyPawn);
				var yFaction = GetPawnFaction(y.KeyPawn);

				var xHasFaction = xFaction == null;
				var yHasFaction = yFaction == null;


				if (xHasFaction != yHasFaction)
				{
					// pawns with no faction at the bottom
					return -Math.Sign(Convert.ToInt32(yHasFaction) - Convert.ToInt32(xHasFaction));
				}
				else if (xHasFaction)
				{
					var xPlayer = xFaction?.IsPlayer ?? false;
					var yPlayer = yFaction?.IsPlayer ?? false;
					if (xPlayer != yPlayer)
					{
						// player's faction at the top
						return Math.Sign(Convert.ToInt32(yPlayer) - Convert.ToInt32(xPlayer));
					}
					else
					{
						var xOrder = xFaction?.def.listOrderPriority ?? 0;
						var yOrder = yFaction?.def.listOrderPriority ?? 0;
						// ingame faction order
						return Math.Sign(yOrder - xOrder);
					}
				}
				else
				{
					// sort by label if no faction
					var xLabel = GetPawnLabel(x.KeyPawn);
					var yLabel = GetPawnLabel(y.KeyPawn);
					return Comparer<string>.Default.Compare(xLabel, yLabel);
				}
			}
		}

        public override IEqualityComparer<Pawn> GroupingEqualityComparer { get; protected set; }
        public override IComparer<PawnTableGroup> GroupsSortingComparer { get; protected set; }

        public GroupWorker_ByFaction()
        {
            GroupingEqualityComparer = new PawnComparer();
            GroupsSortingComparer = new GroupComparer();
        }

		static private Faction GetPawnFaction(Pawn pawn)
        {
			if (pawn == null)
            {
				return null;
            }
#if rw_1_1
			return
				pawn.GetExtraHomeFaction() ??
				pawn.GetExtraHostFaction(); ??
				pawn.Faction 
#else
			return
				pawn.GetExtraMiniFaction() ??
				pawn.GetExtraHomeFaction() ??
				pawn.GetExtraHostFaction() ??
				pawn.Faction;				
#endif

		}

		static private string GetPawnLabel(Pawn pawn)
        {
			return pawn?.kindDef?.label;
        }

        public override TaggedString TitleForGroup(IEnumerable<Pawn> groupPawns, Pawn keyPawn)
        {
			var faction = GetPawnFaction(keyPawn);
			if (faction == null)
            {
				return GetPawnLabel(keyPawn).CapitalizeFirst();
            }
			if (faction.Color == null)
            {
				return faction.Name;
            }
			else
            {
				return faction.Name.Colorize(faction.Color);
			}
		}

		public override string MenuItemTitle()
        {
            return "by faction";
        }

        public override string Key()
        {
            return "faction";
        }
    }
}
