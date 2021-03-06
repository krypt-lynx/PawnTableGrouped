using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public class PawnTableGroupColumn
    {
        Verse.WeakReference<PawnTableGroup> owner;
        private int columnIndex;

        public PawnTableGroupColumn(PawnTableGroup owner, int columnIndex)
        {
            this.owner = new Verse.WeakReference<PawnTableGroup>(owner);
            this.columnIndex = columnIndex;
        }

        public PawnTableGroup Group
        {
            get {
                return owner?.Target;
            }
        }

        public GroupColumnWorker Resolver()
        {
            return Group.ColumnResolvers[columnIndex];
        }

        public PawnColumnWorker ColumnWorker()
        {
            return Resolver()?.ColumnDef?.Worker;
        }

        public bool HasResolver()
        {
            return Group.HasResolver(columnIndex);
        }

        public bool IsInteractive()
        {
            return Group.IsInteractive(columnIndex);
        }

        public bool IsVisible()
        {
            return Group.IsVisible(columnIndex);
        }

        public bool IsUniform()
        {
            return Group.IsUniform(columnIndex);
        }

        public void NotifyValueChanged()
        {
            Group.NotifyValueChanged();
        }

        public object GetGroupValue()
        {
            return Group.GetGroupValue(columnIndex);
        }

        public void SetGroupValue(object value)
        {
            Group.SetGroupValue(columnIndex, value);
        }

        public object GetDefaultValue()
        {
            return Group.GetDefaultValue(columnIndex);
        }

        public object GetValue(Pawn pawn)
        {
            return Group.GetValue(columnIndex, pawn);
        }
    }



    public class PawnTableGroup
    {
        public List<Pawn> Pawns = null;
        public Pawn KeyPawn = null;
        public TaggedString title = null;

        public List<GroupColumnWorker> ColumnResolvers = null;
        private List<bool> columnsIsUniform = new List<bool>();
        private List<object> columnValues = new List<object>();
        private List<bool> columnVisible = new List<bool>();
        private List<PawnTableGroupColumn> columns;

        public TaggedString Title { get => title; }
        public string Key { get; set; }

        private Verse.WeakReference<PawnTable> table = null;

        public IReadOnlyList<PawnTableGroupColumn> Columns
        {
            get
            {
                return columns;
            }
        }

        public PawnTableGroup(Verse.WeakReference<PawnTable> table, TaggedString title, Pawn keyPawn, IEnumerable<Pawn> pawns, List<GroupColumnWorker> columnResolvers, Func<TaggedString> countInfoGenerator = null)
        {
            this.table = table;
            Key = title.RawText;
            KeyPawn = keyPawn;
            ColumnResolvers = columnResolvers;
            Pawns = pawns.ToList();
            columns = columnResolvers.Select((r, i) => new PawnTableGroupColumn(this, i)).ToList();
            this.title = title + $" ({(countInfoGenerator ?? PawnsCountString).Invoke()})";
            RecacheValues();
        }

        TaggedString PawnsCountString()
        {
            return $"{Pawns?.Count ?? 0}";
        }

        public bool HasResolver(int columnIndex)
        {
            return ColumnResolvers[columnIndex] != null;
        }

        public bool IsInteractive(int columnIndex)
        {
            return ColumnResolvers[columnIndex]?.CanSetValues() ?? false;
        }

        public bool IsVisible(int columnIndex)
        {
            return columnVisible[columnIndex];
        }

        private void RecacheValues()
        {
            if (Mod.Settings.disableGroupCells)
            {
                return;
            }

            columnsIsUniform.Clear();
            columnValues.Clear();
            columnVisible.Clear();

            for (int i = 0; i < ColumnResolvers.Count; i++)
            {
                var uniform = ColumnResolvers[i]?.IsUniform(Pawns) ?? true;
                var value = uniform ? ColumnResolvers[i]?.GetGroupValue(Pawns) : ColumnResolvers[i]?.DefaultValue(Pawns);
                var visible = ColumnResolvers[i]?.IsGroupVisible(Pawns) ?? false;

                columnsIsUniform.Add(uniform);
                columnValues.Add(value);
                columnVisible.Add(visible);
            }
        }

        public bool IsUniform(int columnIndex) {
            return columnsIsUniform[columnIndex]; 
        }

        public void NotifyValueChanged()
        {
            RecacheValues();
        }

        public object GetGroupValue(int columnIndex)
        {
            return columnValues[columnIndex];
        }

        public void SetGroupValue(int columnIndex, object value)
        {
            var resolver = ColumnResolvers[columnIndex];
            if (resolver == null || !resolver.CanSetValues())
            {
                return;
            }

            resolver.SetGroupValue(Pawns, value, table?.Target);
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
