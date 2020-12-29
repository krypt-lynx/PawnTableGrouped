using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WildlifeTabAlt
{
    public class PawnTableGroup
    {
        public List<Pawn> Pawns = null;
        public string Title = null;

        public List<GroupColumnWorker> ColumnResolvers = null;
        private List<bool> columnsIsUniform = null;
        private List<object> columnValies = null;
        private List<bool> columnVisible = null;

        public PawnTableGroup()
        {

        }

        public PawnTableGroup(ThingDef race, IEnumerable<Pawn> pawns, List<GroupColumnWorker> columnResolvers)
        {
            this.ColumnResolvers = columnResolvers;
            this.Pawns = pawns.ToList();
            Title = race.label.CapitalizeFirst() ?? "<unknown race>";
            RecacheValues();
        }

        public bool HasResolver(int columnIndex)
        {
            return ColumnResolvers[columnIndex] != null;
        }

        public bool IsInteractive(int columnIndex)
        {
            return ColumnResolvers[columnIndex]?.CanSetValues() ?? false;
        }

        public bool IsVisible(int columnHeader)
        {
            return columnVisible[columnHeader];
        }

        private void RecacheValues()
        {

            columnsIsUniform = ColumnResolvers.Select(r => r?.IsUniform(Pawns) ?? true).ToList();
            columnValies = ColumnResolvers.Select(r => r?.GetGroupValue(Pawns)).ToList();
            columnVisible = ColumnResolvers.Select(r => r?.IsVisible(Pawns) ?? false).ToList();
        }

        public bool InUniform(int columnIndex) {
            return columnsIsUniform[columnIndex]; 
        }

        public void NotifyValueChanged(int columnIndex)
        {
            $"NotifyValueChanged in column {columnIndex}".Log();

            RecacheValues();
        }

        public object GetGroupValue(int columnIndex)
        {
            return columnValies[columnIndex];
        }

        public void SetGroupValue(int columnIndex, object value)
        {

            $"SetGroupValue in column {columnIndex}".Log();

            var resolver = ColumnResolvers[columnIndex];
            if (resolver == null || !resolver.CanSetValues())
            {
                return;
            }


            resolver.SetGroupValue(Pawns, value);
            RecacheValues();
        }

        public object GetDefaultValue(int columnIndex)
        {
            return ColumnResolvers[columnIndex]?.DefaultValue();
        }

        public object GetValue(int columnIndex, Pawn pawn)
        {
            return ColumnResolvers[columnIndex]?.GetValue(pawn);
        }
    }
}
