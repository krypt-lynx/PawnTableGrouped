using Cassowary;
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

    class SettingsView : CElement
    {


        public SettingsView() : base()
        {
            CTabsView tabs;

            tabs = this.AddElement(new CTabsView());
            this.Embed(tabs);

            tabs.AddTab(new GeneralSettingsTab());
            tabs.AddTab(new TablesSettingsTab());
            tabs.AddTab(new ColumnsSettingsTab());

            CElement footer;

            footer = AddElement(new CLabel
            {
                Title = $"Version: {Mod.CommitInfo}",
                TextAlignment = TextAnchor.LowerRight,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny
            });

            this.AddConstraints(
                footer.top ^ this.bottom + 3,
                footer.width ^ footer.intrinsicWidth,
                footer.right ^ this.right,
                footer.height ^ footer.intrinsicHeight);

        }

    }

}
