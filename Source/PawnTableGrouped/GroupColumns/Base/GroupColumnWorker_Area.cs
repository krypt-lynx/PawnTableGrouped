using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public struct AreaData
    {
        public enum DataType
        {
            Undefined = 0,
            Area,
#if rw_1_3_or_later
            Pen,
#endif
        }

        public static AreaData Undefined = new AreaData();
          

        public DataType Type;
        public Area Area;
#if rw_1_3_or_later
        public CompAnimalPenMarker Pen;
#endif

        public AreaData(Area area)
        {
            this.Type = DataType.Area;
            this.Area = area;
#if rw_1_3_or_later
            this.Pen = null;
#endif
        }

#if rw_1_3_or_later
        public AreaData(CompAnimalPenMarker pen)
        {
            this.Type = DataType.Pen;
            this.Area = null;
            this.Pen = pen;
        }
#endif
    }

    public abstract class GroupColumnWorker_Area : GroupColumnWorker
    {
        public override void DoCell(Rect rect, PawnTableGroupColumn column, PawnTable table)
        {
            GuiTools.PushColor(Mouse.IsOver(rect) ? Color.white : Metrics.GroupHeaderOpacityColor);
            //#if rw_1_2_or_earlier
            switch (((AreaData)column.GetGroupValueCached()).Type)
            {
                case AreaData.DataType.Area:
                    KWidgets.DoAllowedAreaSelectors(rect, column, GetAreaLabel);
                    break;
#if rw_1_3_or_later
                case AreaData.DataType.Pen:
                    var pawn = GetRepresentingPawn(column.Group.Pawns);
					AnimalPenGUI.DoAllowedAreaMessage(rect, pawn);
                    break;
#endif
            }

            GuiTools.PopColor();

            if (Event.current.type == EventType.MouseUp && Mouse.IsOver(rect))
            {
                Event.current.Use();
            }
        }

        public virtual string GetAreaLabel(Area area)
        {
            return AreaUtility.AreaAllowedLabel_Area(area);
        }

        public override bool CanSetValues()
        {
            return true;
        }
            


        public override string GetStringValue(Pawn pawn)
        {
            var value = (AreaData)GetValue(pawn);
            switch (value.Type)
            {
                case AreaData.DataType.Area:
                    return value.Area?.Label ?? "Unrestricted";
#if rw_1_3_or_later
                case AreaData.DataType.Pen:
                    if (value.Pen != null) 
                    {
                        return $"{"InPen".Translate()}: {value.Pen.label}";
                    }
                    else
                    {
                        return $"({"Unpenned".Translate()})";
                    }
#endif
                default:
                    return "";
            }
        }
    }
}
