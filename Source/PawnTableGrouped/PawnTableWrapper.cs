using RimWorld;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class PawnTableWrapper
    {
        private Verse.WeakReference<PawnTable> table;
        public PawnTable Table => table?.Target;


        public PawnTableWrapper(PawnTable table)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
        }


        public static implicit operator PawnTable(PawnTableWrapper wrapper) => wrapper.Table;

        #region public api redirect 


#if rw_1_4_or_later
        public List<PawnColumnDef> Columns => Table.Columns;
#else
        public List<PawnColumnDef> Columns => Table.ColumnsListForReading;
#endif
        public PawnColumnDef SortingBy => Table.SortingBy;
        public bool SortingDescending => Table.SortingDescending;
        public Vector2 Size => Table.Size;
        public float HeightNoScrollbar => Table.HeightNoScrollbar;
        public float HeaderHeight => Table.HeaderHeight;
        public List<Pawn> PawnsListForReading => Table.PawnsListForReading;

        public void PawnTableOnGUI(Vector2 position) => Table.PawnTableOnGUI(position);
        public void SetDirty() => Table.SetDirty();
        public void SetFixedSize(Vector2 size) => Table.SetFixedSize(size);
        public void SetMinMaxSize(int minTableWidth, int maxTableWidth, int minTableHeight, int maxTableHeight) => Table.SetMinMaxSize(minTableWidth, maxTableWidth, minTableHeight, maxTableHeight);
        public void SortBy(PawnColumnDef column, bool descending) => Table.SortBy(column, descending);

        #endregion

        static Getter<PawnTable, bool> _get_dirty = Dynamic.InstanceGetField<PawnTable, bool>("dirty");
        static Setter<PawnTable, bool> _set_dirty = Dynamic.InstanceSetField<PawnTable, bool>("dirty");

        public bool dirty
        {
            get => _get_dirty(Table);
            set => _set_dirty(Table, value);
        }

        static Getter<PawnTable, bool> _get_hasFixedSize = Dynamic.InstanceGetField<PawnTable, bool>("hasFixedSize");
        static Setter<PawnTable, bool> _set_hasFixedSize = Dynamic.InstanceSetField<PawnTable, bool>("hasFixedSize");

        public bool hasFixedSize
        {
            get => _get_hasFixedSize(Table);
            set => _set_hasFixedSize(Table, value);
        }

        static Getter<PawnTable, float> _get_cachedHeaderHeight = Dynamic.InstanceGetField<PawnTable, float>("cachedHeaderHeight");
        static Setter<PawnTable, float> _set_cachedHeaderHeight = Dynamic.InstanceSetField<PawnTable, float>("cachedHeaderHeight");
        public float cachedHeaderHeight
        {
            get => _get_cachedHeaderHeight(Table);
            set => _set_cachedHeaderHeight(Table, value);
        }

        static Getter<PawnTable, float> _get_cachedHeightNoScrollbar = Dynamic.InstanceGetField<PawnTable, float>("cachedHeightNoScrollbar");
        static Setter<PawnTable, float> _set_cachedHeightNoScrollbar = Dynamic.InstanceSetField<PawnTable, float>("cachedHeightNoScrollbar");
        public float cachedHeightNoScrollbar
        {
            get => _get_cachedHeightNoScrollbar(Table);
            set => _set_cachedHeightNoScrollbar(Table, value);
        }

        static Getter<PawnTable, List<Pawn>> _get_cachedPawns = Dynamic.InstanceGetField<PawnTable, List<Pawn>>("cachedPawns");
        static Setter<PawnTable, List<Pawn>> _set_cachedPawns = Dynamic.InstanceSetField<PawnTable, List<Pawn>>("cachedPawns");
        public List<Pawn> cachedPawns
        {
            get => _get_cachedPawns(Table);
            set => _set_cachedPawns(Table, value);
        }

        static Getter<PawnTable, Vector2> _get_cachedSize = Dynamic.InstanceGetField<PawnTable, Vector2>("cachedSize");
        static Setter<PawnTable, Vector2> _set_cachedSize = Dynamic.InstanceSetField<PawnTable, Vector2>("cachedSize");
        public Vector2 cachedSize
        {
            get => _get_cachedSize(Table);
            set => _set_cachedSize(Table, value);
        }


        static Getter<PawnTable, List<float>> _get_cachedColumnWidths = Dynamic.InstanceGetField<PawnTable, List<float>>("cachedColumnWidths");
        static Setter<PawnTable, List<float>> _set_cachedColumnWidths = Dynamic.InstanceSetField<PawnTable, List<float>>("cachedColumnWidths");
        public List<float> cachedColumnWidths
        {
            get => _get_cachedColumnWidths(Table);
            set => _set_cachedColumnWidths(Table, value);
        }


        static Getter<PawnTable, List<bool>> _get_columnAtOptimalWidth = Dynamic.InstanceGetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        static Setter<PawnTable, List<bool>> _set_columnAtOptimalWidth = Dynamic.InstanceSetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        public List<bool> columnAtOptimalWidth
        {
            get => _get_columnAtOptimalWidth(Table);
            set => _set_columnAtOptimalWidth(Table, value);
        }


        static Getter<PawnTable, List<bool>> _get_columnAtMaxWidth = Dynamic.InstanceGetField<PawnTable, List<bool>>("columnAtMaxWidth");
        static Setter<PawnTable, List<bool>> _set_columnAtMaxWidth = Dynamic.InstanceSetField<PawnTable, List<bool>>("columnAtMaxWidth");
        public List<bool> columnAtMaxWidth
        {
            get => _get_columnAtMaxWidth(Table);
            set => _set_columnAtMaxWidth(Table, value);
        }



        static Getter<PawnTable, int> _get_minTableWidth = Dynamic.InstanceGetField<PawnTable, int>("minTableWidth");
        static Setter<PawnTable, int> _set_minTableWidth = Dynamic.InstanceSetField<PawnTable, int>("minTableWidth");
        public int minTableWidth
        {
            get => _get_minTableWidth(Table);
            set => _set_minTableWidth(Table, value);
        }


        static Getter<PawnTable, int> _get_maxTableWidth = Dynamic.InstanceGetField<PawnTable, int>("maxTableWidth");
        static Setter<PawnTable, int> _set_maxTableWidth = Dynamic.InstanceSetField<PawnTable, int>("maxTableWidth");
        public int maxTableWidth
        {
            get => _get_maxTableWidth(Table);
            set => _set_maxTableWidth(Table, value);
        }


        static Getter<PawnTable, int> _get_minTableHeight = Dynamic.InstanceGetField<PawnTable, int>("minTableHeight");
        static Setter<PawnTable, int> _set_minTableHeight = Dynamic.InstanceSetField<PawnTable, int>("minTableHeight");
        public int minTableHeight
        {
            get => _get_minTableHeight(Table);
            set => _set_minTableHeight(Table, value);
        }


        static Getter<PawnTable, int> _get_maxTableHeight = Dynamic.InstanceGetField<PawnTable, int>("maxTableHeight");
        static Setter<PawnTable, int> _set_maxTableHeight = Dynamic.InstanceSetField<PawnTable, int>("maxTableHeight");
        public int maxTableHeight
        {
            get => _get_maxTableHeight(Table);
            set => _set_maxTableHeight(Table, value);
        }


        static Getter<PawnTable, PawnColumnDef> _get_sortByColumn = Dynamic.InstanceGetField<PawnTable, PawnColumnDef>("sortByColumn");
        static Setter<PawnTable, PawnColumnDef> _set_sortByColumn = Dynamic.InstanceSetField<PawnTable, PawnColumnDef>("sortByColumn");
        public PawnColumnDef sortByColumn
        {
            get => _get_sortByColumn(Table);
            set => _set_sortByColumn(Table, value);
        }


        static Getter<PawnTable, bool> _get_sortDescending = Dynamic.InstanceGetField<PawnTable, bool>("sortDescending");
        static Setter<PawnTable, bool> _set_sortDescending = Dynamic.InstanceSetField<PawnTable, bool>("sortDescending");
        public bool sortDescending
        {
            get => _get_sortDescending(Table);
            set => _set_sortDescending(Table, value);
        }

        static Action<PawnTable> _RecachePawns = Dynamic.InstanceVoidMethod<PawnTable>("RecachePawns");
        public void RecachePawns()
        {
            _RecachePawns(Table);
        }

        static Action<PawnTable> _RecacheRowHeights = Dynamic.InstanceVoidMethod<PawnTable>("RecacheRowHeights");
        public void RecacheRowHeights()
        {
            _RecacheRowHeights(Table);
        }

        static Action<PawnTable> _RecacheSize = Dynamic.InstanceVoidMethod<PawnTable>("RecacheSize");
        public void RecacheSize()
        {
            _RecacheSize(Table);
        }

        static Action<PawnTable> _RecacheColumnWidths = Dynamic.InstanceVoidMethod<PawnTable>("RecacheColumnWidths");
        public void RecacheColumnWidths()
        {
            _RecacheColumnWidths(Table);
        }

        static Action<PawnTable> _RecacheLookTargets = Dynamic.InstanceVoidMethod<PawnTable>("RecacheLookTargets");
        public void RecacheLookTargets()
        {
            _RecacheLookTargets(Table);
        }

        static Func<PawnTable, float> _CalculateHeaderHeight = Dynamic.InstanceRetMethod<PawnTable, float>("CalculateHeaderHeight");
        public float CalculateHeaderHeight()
        {
            return _CalculateHeaderHeight(Table);
        }

        static Func<PawnTable, float> _CalculateTotalRequiredHeight = Dynamic.InstanceRetMethod<PawnTable, float>("CalculateTotalRequiredHeight");
        public float CalculateTotalRequiredHeight()
        {
            return _CalculateTotalRequiredHeight(Table);
        }

#if rw_1_3_or_earlier
        static Func<PawnTable, Pawn, bool> _CanAssignPawn = Dynamic.InstanceRetMethod<PawnTable, Pawn, bool>("CanAssignPawn");
        public bool CanAssignPawn(Pawn p)
        {
            return _CanAssignPawn(Table, p);
        }
#endif

        static Func<PawnTable, Pawn, float> _CalculateRowHeight = Dynamic.InstanceRetMethod<PawnTable, Pawn, float>("CalculateRowHeight");
        public float CalculateRowHeight(Pawn p)
        {
            return _CalculateRowHeight(Table, p);
        }

        static Action<PawnTable> _RecacheIfDirty = Dynamic.InstanceVoidMethod<PawnTable>("RecacheIfDirty");
        public void RecacheIfDirty()
        {
            _RecacheIfDirty(Table);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _LabelSortFunction = Dynamic.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("LabelSortFunction");
        public IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
        {
            return _LabelSortFunction(Table, input);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _PrimarySortFunction = Dynamic.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("PrimarySortFunction");
        public IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
        {
            return _PrimarySortFunction(Table, input);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetOptimalWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetOptimalWidth");
        public float GetOptimalWidth(PawnColumnDef column)
        {
            return _GetOptimalWidth(Table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMinWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMinWidth");
        public float GetMinWidth(PawnColumnDef column)
        {
            return _GetMinWidth(Table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMaxWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMaxWidth");
        public float GetMaxWidth(PawnColumnDef column)
        {
            return _GetMaxWidth(Table, column);
        }
    }

}
