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


        public SettingsView()
        {
            CTabsView tabs;

            tabs = this.AddElement(new CTabsView());
            this.Embed(tabs);

            tabs.AddTab(new GeneralSettingsTab());
            tabs.AddTab(new TablesSettingsTab());
            tabs.AddTab(new ColumnsSettingsTab());
        }

    }

}
