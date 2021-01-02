using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class PawnTableGroup
    {
        public List<Pawn> Pawns = null;
        public string Title = null;

        public List<GroupColumnWorker> ColumnResolvers = null;
        private List<bool> columnsIsUniform = new List<bool>();
        private List<object> columnValues = new List<object>();
        private List<bool> columnVisible = new List<bool>();

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
            columnsIsUniform.Clear();
            columnValues.Clear();
            columnVisible.Clear();

            for (int i = 0; i < ColumnResolvers.Count; i++)
            {
                var uniform = ColumnResolvers[i]?.IsUniform(Pawns) ?? true;
                var value = uniform ? ColumnResolvers[i]?.GetGroupValue(Pawns) : ColumnResolvers[i]?.DefaultValue(Pawns);
                var visible = ColumnResolvers[i]?.IsVisible(Pawns) ?? false;

                columnsIsUniform.Add(uniform);
                columnValues.Add(value);
                columnVisible.Add(visible);
            }
        }

        public bool IsUniform(int columnIndex) {
            return columnsIsUniform[columnIndex]; 
        }

        public void NotifyValueChanged(int columnIndex)
        {
            $"NotifyValueChanged in column {columnIndex}".Log();

            RecacheValues();
        }

        public object GetGroupValue(int columnIndex)
        {
            return columnValues[columnIndex];
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
            return ColumnResolvers[columnIndex]?.DefaultValue(Pawns);
        }

        public object GetValue(int columnIndex, Pawn pawn)
        {
            return ColumnResolvers[columnIndex]?.GetValue(pawn);
        }
    }
}
