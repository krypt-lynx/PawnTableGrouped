using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WildlifeTabAlt
{
    public abstract class MainTabWindow_GroupedPawnTable : MainTabWindow_PawnTable
    {
        protected abstract PawnTableGroupDef GroupDef { get; }

        private PawnTableGrouped groupedTable;

        private PawnTable table 
        {
            get
            {
                return (PawnTable)typeof(MainTabWindow_PawnTable).GetField("table", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            }
        }

        bool needToInject = true;
        public override void PostOpen()
        {
            base.PostOpen();
            if (needToInject)
            {
                needToInject = false;
                groupedTable = new PawnTableGrouped(table, GroupDef);

            }
        }
    }
}
