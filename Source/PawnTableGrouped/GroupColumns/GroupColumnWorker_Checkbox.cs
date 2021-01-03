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
using Verse.Sound;

namespace PawnTableGrouped
{
    public class GroupColumnWorker_Checkbox : GroupColumnWorker
    {
        public override object GetValue(Pawn pawn)
        {
            return (bool)typeof(PawnColumnWorker_Checkbox).GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Checkbox)ColumnDef.Worker, new object[] { pawn });
        }

        public override void SetValue(Pawn pawn, object value)
        {
            if (HasCheckbox(pawn))
            {
                typeof(PawnColumnWorker_Checkbox).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Checkbox)ColumnDef.Worker, new object[] { pawn, value });
            }
        }

        public bool HasCheckbox(Pawn pawn)
        {
            return (bool)typeof(PawnColumnWorker_Checkbox).GetMethod("HasCheckbox", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Checkbox)ColumnDef.Worker, new object[] { pawn });
        }

        public string GetTip(Pawn pawn)
        {
            return (string)typeof(PawnColumnWorker_Checkbox).GetMethod("GetTip", BindingFlags.NonPublic | BindingFlags.Instance).Invoke((PawnColumnWorker_Checkbox)ColumnDef.Worker, new object[] { pawn });
        }

        public override bool IsUniform(IEnumerable<Pawn> pawns)
        {
            return pawns.IsUniform(p => GetValue(p)) && pawns.All(p => HasCheckbox(p));
        }

        public override void DoCell(Rect rect, PawnTableGroup group, PawnTable table, int columnIndex)
        {
            if (!group.IsUniform(columnIndex))
            {
                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                Vector2 vector = new Vector2(rect.x + dx, rect.y + dy);
                Rect rect2 = new Rect(vector.x, vector.y, 24f, 24f);
                DoMixedValuesIcon(rect);

                bool value;
                if (KWidgets.DraggableSource(rect2, (bool)group.GetDefaultValue(columnIndex), out value))
                {
                    group.SetGroupValue(columnIndex, value);
                }

            }
            else
            {
                var value = (bool)group.GetGroupValue(columnIndex);

                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                Vector2 vector = new Vector2(rect.x + dx, rect.y + dy);

                bool oldValue = value;

                Widgets.Checkbox(vector, ref value, 24f, false, ColumnDef.paintable);

                if (value != oldValue)
                {
                    group.SetGroupValue(columnIndex, value);
                }
            }

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override object DefaultValue(IEnumerable<Pawn> pawns)
        {
            return true;
        }

        public override bool IsVisible(IEnumerable<Pawn> pawns)
        {
            return pawns.Any(x => HasCheckbox(x));
        }
    }
}
