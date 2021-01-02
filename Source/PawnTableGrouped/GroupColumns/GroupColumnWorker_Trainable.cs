using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class GroupColumnWorker_Trainable : GroupColumnWorker
    {
        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => p.training.GetWanted(ColumnDef.trainable));
        }

        private static void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain)
        {
            typeof(TrainingCardUtility).GetMethod("DoTrainableTooltip", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { rect, pawn, td, canTrain });
        }


        public override void DoCell(Rect rect, PawnTableGroup group, PawnTable table, int columnIndex)
        {
            if (!group.IsUniform(columnIndex))
            {
                DoMixedValuesIcon(rect);
            }
            else
            {
                var pawn = group.Pawns.First();
                if (pawn.training == null)
                {
                    return;
                }
                bool visible;
                AcceptanceReport canTrain = pawn.training.CanAssignToTrain(ColumnDef.trainable, out visible);
                if (!visible || !canTrain.Accepted)
                {
                    return;
                }
                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                DoTrainableCheckbox(new Rect(rect.x + dx, rect.y + dy, 24f, 24f), pawn, ColumnDef.trainable, canTrain);
            }
        }

        private static void DoTrainableCheckbox(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain)
        {
            //bool learned = pawn.training.HasLearned(td);
            bool wanted = pawn.training.GetWanted(td);
            bool oldWanted = wanted;
                
            Widgets.Checkbox(rect.position, ref wanted, rect.width, !canTrain.Accepted, true);

            if (wanted != oldWanted)
            {
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.AnimalTraining, KnowledgeAmount.Total);
                pawn.training.SetWantedRecursive(td, wanted);
            }

            DoTrainableTooltip(rect, pawn, td, canTrain);           
        }


        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return true;
        }

        public override object GetValue(Pawn pawn)
        {
            return pawn.training?.GetWanted(ColumnDef.trainable) ?? false;
        }

        public override void SetValue(Pawn pawn, object value)
        {
            pawn.training?.SetWantedRecursive(ColumnDef.trainable, (bool)value);
        }

        public override bool IsVisible(IEnumerable<Pawn> pawns)
        {
            return Mod.Settings.interactiveGroupHeader &&
                pawns.All(p =>
                {
                    bool visible;
                    p.training.CanAssignToTrain(ColumnDef.trainable, out visible);
                    return visible;
                });
        }
    }
}
