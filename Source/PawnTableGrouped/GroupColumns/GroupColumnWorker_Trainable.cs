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
        private static void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain)
        {
            typeof(TrainingCardUtility).GetMethod("DoTrainableTooltip", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { rect, pawn, td, canTrain });
        }


        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            if (!column.IsUniform())
            {
                DoMixedValuesWidget(rect, column);
            }
            else
            {
                var pawn = GetRepresentingPawn(column.Group.Pawns);
                if (pawn.training == null)
                {
                    return;
                }
                bool visible = false;
                AcceptanceReport canTrain = pawn.training?.CanAssignToTrain(ColumnDef.trainable, out visible) ?? false;
                if (!visible || !canTrain.Accepted)
                {
                    return;
                }
                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                DoTrainableCheckbox(new Rect(rect.x + dx, rect.y + dy, 24f, 24f), column, ColumnDef.trainable, canTrain);
            }

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        private void DoTrainableCheckbox(Rect rect, PawnTableGroupColumn column, TrainableDef td, AcceptanceReport canTrain)
        {
            //bool learned = pawn.training.HasLearned(td);

            bool wanted = (bool)GetGroupValue(column.Group.Pawns);
            bool oldWanted = wanted;
                
            Widgets.Checkbox(rect.position, ref wanted, rect.width, !canTrain.Accepted, true);

            if (wanted != oldWanted)
            {
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.AnimalTraining, KnowledgeAmount.Total);
                column.SetGroupValue(wanted);
            }

            //DoTrainableTooltip(rect, pawn, td, canTrain);           
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
            if (pawn.training != null && (pawn.training?.CanAssignToTrain(ColumnDef.trainable).Accepted ?? false))
            {
                pawn.training?.SetWantedRecursive(ColumnDef.trainable, (bool)value);
            }
        }

        public override bool IsVisible(Pawn pawn)
        {
            return pawn.training?.CanAssignToTrain(ColumnDef.trainable).Accepted ?? false;
        }
    }
}
