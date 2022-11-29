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

        public bool IsVisibleCached()
        {
            return Group.IsVisibleCached(columnIndex);
        }

        public bool IsUniformCached()
        {
            return Group.IsUniformCached(columnIndex);
        }

        public void NotifyValueChanged()
        {
            Group.NotifyValueChanged();
        }

        public object GetGroupValueCached()
        {
            return Group.GetGroupValueCached(columnIndex);
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

        public void SetDirty()
        {
            Group.SetDirty();
        }
    }



    public class PawnTableGroup
    {
        public List<Pawn> Pawns = null;
        public Pawn KeyPawn = null;
        public TaggedString title = null;

        public List<GroupColumnWorker> ColumnResolvers = null;
        private bool[] columnsIsUniform;
        private bool[] columnIsCached;
        private object[] columnValues;
        private bool[] columnVisible;
        private bool[] columnIsStaticVisible;
        private List<PawnTableGroupColumn> columns;


        public TaggedString Title { get => title; }
        public string Key { get; set; }

        private Verse.WeakReference<PawnTable> table = null;
        public PawnTable Table => table.Target;

        public IReadOnlyList<PawnTableGroupColumn> Columns
        {
            get
            {
                return columns;
            }
        }

        public PawnTableGroup(PawnTable table, TaggedString title, Pawn keyPawn, IEnumerable<Pawn> pawns, List<GroupColumnWorker> columnResolvers, Func<TaggedString> countInfoGenerator = null)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            Key = title.RawText;
            KeyPawn = keyPawn;
            ColumnResolvers = columnResolvers;
            Pawns = pawns.ToList();
            columns = columnResolvers.Select((r, i) => new PawnTableGroupColumn(this, i)).ToList();

            this.title = title + $" ({(countInfoGenerator ?? PawnsCountString).Invoke()})";
            InitArrays();
            UpdateIsVisible();
        }

        private void UpdateIsVisible()
        {
            for (int i = 0; i < ColumnResolvers.Count; i++)
            {
                if (ColumnResolvers[i].IsStaticVisible())
                {
                    columnIsStaticVisible[i] = true;
                    columnVisible[i] = ColumnResolvers[i]?.IsGroupVisible(Pawns) ?? false;
                }
            }                
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

        public bool IsVisibleCached(int columnIndex)
        {
            if (!columnIsStaticVisible[columnIndex])
            {
                UpdateColumnIfNeeded(columnIndex);
            }                
            return columnVisible[columnIndex];
        }

        void InitArrays()
        {
            columnIsCached = new bool[ColumnResolvers.Count];
            columnsIsUniform = new bool[ColumnResolvers.Count];
            columnValues = new object[ColumnResolvers.Count];
            columnVisible = new bool[ColumnResolvers.Count];
            columnIsStaticVisible = new bool[ColumnResolvers.Count];
        }

        private void ResetCache()
        {
            Array.Fill(columnIsCached, false);
        }

        private void UpdateColumnIfNeeded(int columnIndex) // todo: optimize
        {
            if (!columnIsCached[columnIndex])
            {
                columnIsCached[columnIndex] = true;
                var uniform = ColumnResolvers[columnIndex]?.IsUniform(Pawns) ?? true;
                columnsIsUniform[columnIndex] = uniform;
                columnValues[columnIndex] = uniform ? ColumnResolvers[columnIndex]?.GetGroupValue(Pawns) : ColumnResolvers[columnIndex]?.DefaultValue(Pawns);
                columnVisible[columnIndex] = ColumnResolvers[columnIndex]?.IsGroupVisible(Pawns) ?? false;
            }
        }

        public bool IsUniformCached(int columnIndex)
        {
            UpdateColumnIfNeeded(columnIndex);
            return columnsIsUniform[columnIndex]; 
        }

        public void NotifyValueChanged()
        {
            ResetCache();
        }

        public object GetGroupValueCached(int columnIndex)
        {
            UpdateColumnIfNeeded(columnIndex);
            return columnValues[columnIndex];
        }

        public void SetGroupValue(int columnIndex, object value)
        {
            var resolver = ColumnResolvers[columnIndex];
            if (resolver == null || !resolver.CanSetValues())
            {
                return;
            }

            resolver.SetGroupValue(Pawns, value, Table);
            columnIsCached[columnIndex] = false;
        }

        public object GetDefaultValue(int columnIndex)
        {
            return ColumnResolvers[columnIndex]?.DefaultValue(Pawns);
        }

        public object GetValue(int columnIndex, Pawn pawn)
        {
            return ColumnResolvers[columnIndex]?.GetValue(pawn);
        }

        internal void SetDirty()
        {
            ResetCache();
            Table.SetDirty();
        }
    }
}
