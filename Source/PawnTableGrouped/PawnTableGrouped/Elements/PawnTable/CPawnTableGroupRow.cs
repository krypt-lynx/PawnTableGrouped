using Cassowary;
using PawnTableGrouped.TableGrid;
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
    class CPawnTableGroupRow : ICTableGridRow
    {
        public Action<PawnTableGroup> Action { get; set; }

        private PawnTableGroup group;
        //private readonly Verse.WeakReference<ICTableGridDataSource> dataSource;
        private bool expanded;

        CGuiRoot host = new CGuiRoot();
        private ClVariable fadeOurEdgeAnchor;
        float fixedWidth;

        public CPawnTableGroupRow(PawnTableGroup group, float fixedWidth, bool expanded)
        {
            this.group = group;
            //this.dataSource = new Verse.WeakReference<ICTableGridDataSource>(dataSource);
            this.expanded = expanded;

            this.fixedWidth = fixedWidth;

            ConstructGui();
        }

        public bool CanCombineWith(ICTableGridRow other, int columnIndex)
        {
            return false;
        }


        public void DoBackground(Rect rect) {
            host.InRect = rect;
            host.UpdateLayoutIfNeeded();
            group.NotifyValueChanged();
        }

        public void DoCell(Rect rect, int columnIndex, bool hightlighted, bool combined)
        {
            if (Mod.Settings.disableGroupCells || columnIndex == 0 || rect.xMax < fadeOurEdgeAnchor.Value - fixedWidth)
            {
                return;
            }

            var resolver = group.ColumnResolvers[columnIndex];

            if (resolver != null && group.IsVisibleCached(columnIndex))
            {
                if (!resolver.IsDummy() && resolver.HealthCheck)
                {
                    try
                    {
                        resolver.DoCell(rect, group.Columns[columnIndex], group.Table); // todo: table is null
                    } 
                    catch (Exception e)
                    {
                        LogHelper.LogException($"Failed to draw group header cell {resolver.GetType().Name}, disabling it until game restart", e);
                        resolver.HealthCheck = false;
                    }
                }
            }

            if (Mod.Settings.showDummyColumns)
            {
                TooltipHandler.TipRegion(rect, $"{resolver.ColumnDef.defName}: {resolver.ColumnDef.workerClass.FullName}");
            }

            //GuiTools.PushColor(Color.magenta);
            //GuiTools.Box(rect, EdgeInsets.One);
            //Widgets.Label(rect, columnIndex.ToString());
            //GuiTools.PopColor();
        }

        static readonly Resource<Texture2D> TexCollapse = new Resource<Texture2D>("UI/Buttons/Dev/Collapse");
        static readonly Resource<Texture2D> TexRevial = new Resource<Texture2D>("UI/Buttons/Dev/Reveal");

        private void ConstructGui()
        {
            var bg = host.AddElement(new CImage
            {
                Texture = BaseContent.WhiteTex,
                TintColor = Widgets.WindowBGFillColor,
            });

            var indicator = host.AddElement(new CImage
            {
                Texture = expanded ? TexCollapse : TexRevial
            });

            var label = host.AddElement(new CLabel
            {
                TaggedTitle = group.Title,
                Font = GameFont.Small,
                Color = new Color(1, 1, 1, 0.6f),
                TextAlignment = TextAnchor.MiddleLeft,
            });



            var fadeIn = host.AddElement(new CImage
            {
                Texture = Textures.LinearGradient,
                TintColor = Widgets.WindowBGFillColor,
            });

            host.StackLeft(StackOptions.Create(constrainSides: false, constrainEnd: false), indicator, (label, label.intrinsicWidth), 8, (fadeIn, 60));
            indicator.MakeSizeIntristic();

            host.AddConstraints(
                indicator.centerY ^ host.centerY,
                label.centerY ^ host.centerY,

                label.height ^ label.intrinsicHeight,

                fadeIn.top ^ host.top,
                fadeIn.bottom ^ host.bottom,

                bg.left ^ host.left,
                bg.top ^ host.top,
                bg.bottom ^ host.bottom,
                bg.right ^ fadeIn.left
                );

            fadeOurEdgeAnchor = fadeIn.left;
        }

        public void DoOverlay(Rect rect)
        {
            host.DoElementContent();

            if (Widgets.ButtonInvisible(rect))
            {
                if (Event.current.button == 0)
                {
                    Action?.Invoke(group);
                } 
                else
                {
                    if (!Event.current.shift)
                    {
                        Find.Selector.ClearSelection();
                    }

                    foreach (var pawn in group.Pawns)
                    {
                        Find.Selector.Select(pawn, true, false);
                    }
                }                
            }
        }

        public float GetRowHeight()
        {
            return Metrics.GroupHeaderHeight;
        }
    }

}
