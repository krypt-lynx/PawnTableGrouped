using Cassowary;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WildlifeTabAlt
{
    public interface IPawnTableGrouped
    {
        void override_RecacheIfDirty();
        void override_PawnTableOnGUI(Vector2 position);

        float override_CalculateTotalRequiredHeight();
        
    }

    public class PawnTableGroup
    {
        public List<Pawn> Pawns = null;
        public string Title = null;
    }

    public class PawnTableGroupedImpl
    {
        PawnTableAccessor accessor;
        PawnTable table; // todo: weak ref
        private PawnTableDef def;


        public PawnTableGroupedImpl(PawnTable table, PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight)
        {
            this.table = table;
            this.def = def;

            accessor = new PawnTableAccessor(table);

            table.SetDirty();
            ConstructGUI();
        }

        CGuiRoot host = new CGuiRoot();
        CListView list;
        CElement header;

        void ConstructGUI()
        {
            header = host.AddElement(new CPawnListHeader(table, accessor));
            list = host.AddElement(new CListView
            {
                ShowScrollBar = CScrollBarMode.Show
            });

            host.StackTop((header, header.intrinsicHeight), list);
        }


        static private string PawnLabel(Pawn pawn)
        {
            if (!pawn.RaceProps.Humanlike && pawn.Name != null && !pawn.Name.Numerical)
            {
                return pawn.Name.ToStringShort.CapitalizeFirst() + ", " + pawn.KindLabel;
            }
            else
            {
                return pawn.LabelCap;
            }
        }

        private void PopulateList(IEnumerable<PawnTableGroup> groups)
        {
            list.ClearRows();

            foreach (var group in groups)
            {
                var row = new CPawnListSection(group, IsExpanded(group));
                row.Action = (sectionRow) =>
                {
                    var g = ((CPawnListSection)sectionRow).Group;
                    SetExpanded(g, !IsExpanded(g));
                    table.SetDirty();
                };
                list.AppendRow(row);
                row.AddConstraint(row.height ^ 30);

                if (IsExpanded(group))
                {
                    foreach (var pawn in group.Pawns)
                    {
                        var pawnRow = list.AppendRow(new CPawnListRow(table, accessor, pawn));

                        pawnRow.AddConstraint(pawnRow.height ^ pawnRow.intrinsicHeight);
                    }
                }
            }
        }


        List<PawnTableGroup> sections;
        Dictionary<string, bool> ExpandedState = new Dictionary<string, bool>();
        bool IsExpanded(PawnTableGroup group)
        {
            if (ExpandedState.ContainsKey(group.Title))
            {
                return ExpandedState[group.Title];
            }
            else
            {
                return true;
            }
        }

        void SetExpanded(PawnTableGroup group, bool expanded)
        {
            ExpandedState[group.Title] = expanded;
        }

        public void RecacheGroups()
        {
            //  IEqualityComparer
            // var groups = PawnsListForReading.GroupBy(x => x, new ByColumnComparer(DefDatabase<PawnColumnDef>.GetNamed("Label")));

            // return input.OrderByDescending((Pawn p) => p.RaceProps.baseBodySize).ThenBy((Pawn p) => p.def.label);

            var groups = table.PawnsListForReading.GroupBy(p => p.kindDef.race).OrderByDescending(g => g.Key.race.baseBodySize).ThenBy(g => g.Key.label);
            sections = groups.Select(x => new PawnTableGroup
            {
                Title = x.Key.label.CapitalizeFirst() ?? "<unknown race>",
                Pawns = x.ToList(),
            }).ToList();

        }

        public float CalculateTotalRequiredHeight()
        {
            float height = accessor.cachedHeaderHeight;
            foreach (var section in sections)
            {
                height += 30;
                if (IsExpanded(section))
                {
                    foreach (var pawn in section.Pawns)
                    {
                        height += accessor.CalculateRowHeight(pawn);
                    }
                }
            }

            return height;
        }

        public void PawnTableOnGUI(Vector2 position)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            accessor.RecacheIfDirty();

            host.InRect = new Rect((int)position.x, (int)position.y, (int)accessor.cachedSize.x, (int)accessor.cachedSize.y);
            host.UpdateLayoutIfNeeded();
            host.DoElementContent();
        }


        public void RecacheIfDirty()
        {
            if (!accessor.dirty)
            {
                return;
            }
            accessor.dirty = false;
            accessor.RecachePawns();
            RecacheGroups();

            accessor.RecacheRowHeights();
            accessor.cachedHeaderHeight = accessor.CalculateHeaderHeight();
            accessor.cachedHeightNoScrollbar = CalculateTotalRequiredHeight();
            accessor.RecacheSize();
            accessor.RecacheColumnWidths();
            //RecacheColumnWidths();
            accessor.RecacheLookTargets();

            PopulateList(sections);
        }/*

        private void RecacheColumnWidths()
        {
            float num = accessor.cachedSize.x - 16f;
            float num2 = 0f;
            this.RecacheColumnWidths_StartWithMinWidths(out num2);
            if (num2 == num)
            {
                return;
            }
            if (num2 > num)
            {
                this.SubtractProportionally(num2 - num, num2);
                return;
            }
            bool flag;
            this.RecacheColumnWidths_DistributeUntilOptimal(num, ref num2, out flag);
            if (flag)
            {
                return;
            }
            this.RecacheColumnWidths_DistributeAboveOptimal(num, ref num2);
        }*/
        /*
        private void RecacheColumnWidths_StartWithMinWidths(out float minWidthsSum)
        {
            minWidthsSum = 0f;
            accessor.cachedColumnWidths.Clear();
            for (int i = 0; i < this.def.columns.Count; i++)
            {
                float minWidth = this.GetMinWidth(this.def.columns[i]);
                accessor.cachedColumnWidths.Add(minWidth);
                minWidthsSum += minWidth;
            }
        }     

        private void SubtractProportionally(float toSubtract, float totalUsedWidth)
        {
            for (int i = 0; i < accessor.cachedColumnWidths.Count; i++)
            {
                List<float> list = accessor.cachedColumnWidths;
                int index = i;
                list[index] -= toSubtract * accessor.cachedColumnWidths[i] / totalUsedWidth;
            }
        }

        private void RecacheColumnWidths_DistributeUntilOptimal(float totalAvailableSpaceForColumns, ref float usedWidth, out bool noMoreFreeSpace)
        {
            accessor.columnAtOptimalWidth.Clear();
            for (int i = 0; i < this.def.columns.Count; i++)
            {
                accessor.columnAtOptimalWidth.Add(accessor.cachedColumnWidths[i] >= this.GetOptimalWidth(this.def.columns[i]));
            }
            int num = 0;
            for (; ; )
            {
                num++;
                if (num >= 10000)
                {
                    break;
                }
                float num2 = float.MinValue;
                for (int j = 0; j < this.def.columns.Count; j++)
                {
                    if (!accessor.columnAtOptimalWidth[j])
                    {
                        num2 = Mathf.Max(num2, (float)this.def.columns[j].widthPriority);
                    }
                }
                float num3 = 0f;
                for (int k = 0; k < accessor.cachedColumnWidths.Count; k++)
                {
                    if (!accessor.columnAtOptimalWidth[k] && (float)this.def.columns[k].widthPriority == num2)
                    {
                        num3 += this.GetOptimalWidth(this.def.columns[k]);
                    }
                }
                float num4 = totalAvailableSpaceForColumns - usedWidth;
                bool flag = false;
                bool flag2 = false;
                for (int l = 0; l < accessor.cachedColumnWidths.Count; l++)
                {
                    if (!accessor.columnAtOptimalWidth[l])
                    {
                        if ((float)this.def.columns[l].widthPriority != num2)
                        {
                            flag = true;
                        }
                        else
                        {
                            float num5 = num4 * this.GetOptimalWidth(this.def.columns[l]) / num3;
                            float num6 = this.GetOptimalWidth(this.def.columns[l]) - accessor.cachedColumnWidths[l];
                            if (num5 >= num6)
                            {
                                num5 = num6;
                                accessor.columnAtOptimalWidth[l] = true;
                                flag2 = true;
                            }
                            else
                            {
                                flag = true;
                            }
                            if (num5 > 0f)
                            {
                                List<float> list = accessor.cachedColumnWidths;
                                int index = l;
                                list[index] += num5;
                                usedWidth += num5;
                            }
                        }
                    }
                }
                if (usedWidth >= totalAvailableSpaceForColumns - 0.1f)
                {
                    goto Block_13;
                }
                if (!flag || !flag2)
                {
                    goto IL_243;
                }
            }
            Log.Error("Too many iterations.", false);
            goto IL_243;
        Block_13:
            noMoreFreeSpace = true;
        IL_243:
            noMoreFreeSpace = false;
        }

        private void RecacheColumnWidths_DistributeAboveOptimal(float totalAvailableSpaceForColumns, ref float usedWidth)
        {
            accessor.columnAtMaxWidth.Clear();
            for (int i = 0; i < this.def.columns.Count; i++)
            {
                this.columnAtMaxWidth.Add(this.cachedColumnWidths[i] >= this.GetMaxWidth(this.def.columns[i]));
            }
            int num = 0;
            for (; ; )
            {
                num++;
                if (num >= 10000)
                {
                    break;
                }
                float num2 = 0f;
                for (int j = 0; j < this.def.columns.Count; j++)
                {
                    if (!this.columnAtMaxWidth[j])
                    {
                        num2 += Mathf.Max(this.GetOptimalWidth(this.def.columns[j]), 1f);
                    }
                }
                float num3 = totalAvailableSpaceForColumns - usedWidth;
                bool flag = false;
                for (int k = 0; k < this.def.columns.Count; k++)
                {
                    if (!this.columnAtMaxWidth[k])
                    {
                        float num4 = num3 * Mathf.Max(this.GetOptimalWidth(this.def.columns[k]), 1f) / num2;
                        float num5 = this.GetMaxWidth(this.def.columns[k]) - this.cachedColumnWidths[k];
                        if (num4 >= num5)
                        {
                            num4 = num5;
                            this.columnAtMaxWidth[k] = true;
                        }
                        else
                        {
                            flag = true;
                        }
                        if (num4 > 0f)
                        {
                            List<float> list = this.cachedColumnWidths;
                            int index = k;
                            list[index] += num4;
                            usedWidth += num4;
                        }
                    }
                }
                if (usedWidth >= totalAvailableSpaceForColumns - 0.1f)
                {
                    return;
                }
                if (!flag)
                {
                    goto Block_10;
                }
            }
            Log.Error("Too many iterations.", false);
            return;
        Block_10:
            this.DistributeRemainingWidthProportionallyAboveMax(totalAvailableSpaceForColumns - usedWidth);
        }


        // Token: 0x0600627B RID: 25211 RVA: 0x002226C9 File Offset: 0x002208C9
        private float GetOptimalWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetOptimalWidth(table), 0f);
        }

        // Token: 0x0600627C RID: 25212 RVA: 0x002226E2 File Offset: 0x002208E2
        private float GetMinWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetMinWidth(table), 0f);
        }

        // Token: 0x0600627D RID: 25213 RVA: 0x002226FB File Offset: 0x002208FB
        private float GetMaxWidth(PawnColumnDef column)
        {
            return Mathf.Max((float)column.Worker.GetMaxWidth(table), 0f);
        }*/
    }


    public class PawnTable_WildlifeGrouped : PawnTable_Wildlife, IPawnTableGrouped
    {
        PawnTableGroupedImpl impl;

        public PawnTable_WildlifeGrouped(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            impl = new PawnTableGroupedImpl(this, def, pawnsGetter, uiWidth, uiHeight);
        }

        public void override_PawnTableOnGUI(Vector2 position)
        {
            impl.PawnTableOnGUI(position);
        }

        public void override_RecacheIfDirty()
        {
            impl.RecacheIfDirty();
        }
  
        public float override_CalculateTotalRequiredHeight()
        {
            return impl.CalculateTotalRequiredHeight();
        }
    }

    public class PawnTable_AnimalsGrouped : PawnTable_Animals, IPawnTableGrouped
    {
        PawnTableGroupedImpl impl;

        public PawnTable_AnimalsGrouped(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight) : base(def, pawnsGetter, uiWidth, uiHeight)
        {
            impl = new PawnTableGroupedImpl(this, def, pawnsGetter, uiWidth, uiHeight);
        }

        public void override_PawnTableOnGUI(Vector2 position)
        {
            impl.PawnTableOnGUI(position);
        }

        public void override_RecacheIfDirty()
        {
            impl.RecacheIfDirty();
        }

        public float override_CalculateTotalRequiredHeight()
        {
            return impl.CalculateTotalRequiredHeight();
        }
    }

}
