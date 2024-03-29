﻿using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
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
    public class GCW_Checkbox_Config
    {
        public GraphicData Checked = null;
        public GraphicData Unchecked = null;
    }

    public abstract class GroupColumnWorker_CheckboxBase : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
            if (!column.IsUniformCached())
            {
                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                Vector2 vector = new Vector2(rect.x + dx, rect.y + dy);
                Rect rect2 = new Rect(vector.x, vector.y, 24f, 24f);
                DoMixedValuesIcon(rect);

                bool value;
                if (KWidgets.DraggableSource(rect2, (bool)column.GetDefaultValue(), out value))
                {
                    column.SetGroupValue(value);
                }

            }
            else
            {
                var value = (bool)column.GetGroupValueCached();

                int dx = (int)((rect.width - 24f) / 2f);
                int dy = 3;
                Vector2 vector = new Vector2(rect.x + dx, rect.y + dy);

                bool oldValue = value;
                var config = GetWorkerConfig<GCW_Checkbox_Config>();
                Widgets.Checkbox(vector, ref value, 24f, false, ColumnDef.paintable,
                    (Texture2D)config.Checked?.Graphic?.MatSingle?.mainTexture,
                    (Texture2D)config.Unchecked?.Graphic?.MatSingle?.mainTexture);

                if (value != oldValue)
                {
                    column.SetGroupValue(value);
                }
            }
            GuiTools.PopColor();

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        public override bool CanSetValues()
        {
            return true;
        }

        public override string GetStringValue(Pawn pawn)
        {
            return (bool)GetValue(pawn) ? "Yes" : "No";
        }
    }
}
