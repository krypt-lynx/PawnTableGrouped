using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public class PawnTableAccessor // Accessors
    {
        private Verse.WeakReference<PawnTable> table;
        private PawnTable Table => table?.Target;

        public PawnTableAccessor(PawnTable table)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
        }

        static Func<PawnTable, bool> _get_dirty = FastAccess.InstanceGetField<PawnTable, bool>("dirty");
        static Action<PawnTable, bool> _set_dirty = FastAccess.InstanceSetField<PawnTable, bool>("dirty");

        public bool dirty
        {
            get => _get_dirty(Table);
            set => _set_dirty(Table, value);
        }

        static Func<PawnTable, bool> _get_hasFixedSize = FastAccess.InstanceGetField<PawnTable, bool>("hasFixedSize");
        static Action<PawnTable, bool> _set_hasFixedSize = FastAccess.InstanceSetField<PawnTable, bool>("hasFixedSize");

        public bool hasFixedSize
        {
            get => _get_hasFixedSize(Table);
            set => _set_hasFixedSize(Table, value);
        }

        static Func<PawnTable, float> _get_cachedHeaderHeight = FastAccess.InstanceGetField<PawnTable, float>("cachedHeaderHeight");
        static Action<PawnTable, float> _set_cachedHeaderHeight = FastAccess.InstanceSetField<PawnTable, float>("cachedHeaderHeight");
        public float cachedHeaderHeight
        {
            get => _get_cachedHeaderHeight(Table);
            set => _set_cachedHeaderHeight(Table, value);
        }

        static Func<PawnTable, float> _get_cachedHeightNoScrollbar = FastAccess.InstanceGetField<PawnTable, float>("cachedHeightNoScrollbar");
        static Action<PawnTable, float> _set_cachedHeightNoScrollbar = FastAccess.InstanceSetField<PawnTable, float>("cachedHeightNoScrollbar");
        public float cachedHeightNoScrollbar
        {
            get => _get_cachedHeightNoScrollbar(Table);
            set => _set_cachedHeightNoScrollbar(Table, value);
        }

        static Func<PawnTable, List<Pawn>> _get_cachedPawns = FastAccess.InstanceGetField<PawnTable, List<Pawn>>("cachedPawns");
        static Action<PawnTable, List<Pawn>> _set_cachedPawns = FastAccess.InstanceSetField<PawnTable, List<Pawn>>("cachedPawns");
        public List<Pawn> cachedPawns
        {
            get => _get_cachedPawns(Table);
            set => _set_cachedPawns(Table, value);
        }

        static Func<PawnTable, Vector2> _get_cachedSize = FastAccess.InstanceGetField<PawnTable, Vector2>("cachedSize");
        static Action<PawnTable, Vector2> _set_cachedSize = FastAccess.InstanceSetField<PawnTable, Vector2>("cachedSize");
        public Vector2 cachedSize
        {
            get => _get_cachedSize(Table);
            set => _set_cachedSize(Table, value);
        }


        static Func<PawnTable, List<float>> _get_cachedColumnWidths = FastAccess.InstanceGetField<PawnTable, List<float>>("cachedColumnWidths");
        static Action<PawnTable, List<float>> _set_cachedColumnWidths = FastAccess.InstanceSetField<PawnTable, List<float>>("cachedColumnWidths");
        public List<float> cachedColumnWidths
        {
            get => _get_cachedColumnWidths(Table);
            set => _set_cachedColumnWidths(Table, value);
        }


        static Func<PawnTable, List<bool>> _get_columnAtOptimalWidth = FastAccess.InstanceGetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        static Action<PawnTable, List<bool>> _set_columnAtOptimalWidth = FastAccess.InstanceSetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        public List<bool> columnAtOptimalWidth
        {
            get => _get_columnAtOptimalWidth(Table);
            set => _set_columnAtOptimalWidth(Table, value);
        }


        static Func<PawnTable, List<bool>> _get_columnAtMaxWidth = FastAccess.InstanceGetField<PawnTable, List<bool>>("columnAtMaxWidth");
        static Action<PawnTable, List<bool>> _set_columnAtMaxWidth = FastAccess.InstanceSetField<PawnTable, List<bool>>("columnAtMaxWidth");
        public List<bool> columnAtMaxWidth
        {
            get => _get_columnAtMaxWidth(Table);
            set => _set_columnAtMaxWidth(Table, value);
        }



        static Func<PawnTable, int> _get_minTableWidth = FastAccess.InstanceGetField<PawnTable, int>("minTableWidth");
        static Action<PawnTable, int> _set_minTableWidth = FastAccess.InstanceSetField<PawnTable, int>("minTableWidth");
        public int minTableWidth
        {
            get => _get_minTableWidth(Table);
            set => _set_minTableWidth(Table, value);
        }


        static Func<PawnTable, int> _get_maxTableWidth = FastAccess.InstanceGetField<PawnTable, int>("maxTableWidth");
        static Action<PawnTable, int> _set_maxTableWidth = FastAccess.InstanceSetField<PawnTable, int>("maxTableWidth");
        public int maxTableWidth
        {
            get => _get_maxTableWidth(Table);
            set => _set_maxTableWidth(Table, value);
        }


        static Func<PawnTable, int> _get_minTableHeight = FastAccess.InstanceGetField<PawnTable, int>("minTableHeight");
        static Action<PawnTable, int> _set_minTableHeight = FastAccess.InstanceSetField<PawnTable, int>("minTableHeight");
        public int minTableHeight
        {
            get => _get_minTableHeight(Table);
            set => _set_minTableHeight(Table, value);
        }


        static Func<PawnTable, int> _get_maxTableHeight = FastAccess.InstanceGetField<PawnTable, int>("maxTableHeight");
        static Action<PawnTable, int> _set_maxTableHeight = FastAccess.InstanceSetField<PawnTable, int>("maxTableHeight");
        public int maxTableHeight
        {
            get => _get_maxTableHeight(Table);
            set => _set_maxTableHeight(Table, value);
        }


        static Func<PawnTable, PawnColumnDef> _get_sortByColumn = FastAccess.InstanceGetField<PawnTable, PawnColumnDef>("sortByColumn");
        static Action<PawnTable, PawnColumnDef> _set_sortByColumn = FastAccess.InstanceSetField<PawnTable, PawnColumnDef>("sortByColumn");
        public PawnColumnDef sortByColumn
        {
            get => _get_sortByColumn(Table);
            set => _set_sortByColumn(Table, value);
        }


        static Func<PawnTable, bool> _get_sortDescending = FastAccess.InstanceGetField<PawnTable, bool>("sortDescending");
        static Action<PawnTable, bool> _set_sortDescending = FastAccess.InstanceSetField<PawnTable, bool>("sortDescending");
        public bool sortDescending
        {
            get => _get_sortDescending(Table);
            set => _set_sortDescending(Table, value);
        }

        static Action<PawnTable> _RecachePawns = FastAccess.InstanceVoidMethod<PawnTable>("RecachePawns");
        public void RecachePawns()
        {
            _RecachePawns(Table);
        }

        static Action<PawnTable> _RecacheRowHeights = FastAccess.InstanceVoidMethod<PawnTable>("RecacheRowHeights");
        public void RecacheRowHeights()
        {
            _RecacheRowHeights(Table);
        }

        static Action<PawnTable> _RecacheSize = FastAccess.InstanceVoidMethod<PawnTable>("RecacheSize");
        public void RecacheSize()
        {
            _RecacheSize(Table);
        }

        static Action<PawnTable> _RecacheColumnWidths = FastAccess.InstanceVoidMethod<PawnTable>("RecacheColumnWidths");
        public void RecacheColumnWidths()
        {
            _RecacheColumnWidths(Table);
        }

        static Action<PawnTable> _RecacheLookTargets = FastAccess.InstanceVoidMethod<PawnTable>("RecacheLookTargets");
        public void RecacheLookTargets()
        {
            _RecacheLookTargets(Table);
        }

        static Func<PawnTable, float> _CalculateHeaderHeight = FastAccess.InstanceRetMethod<PawnTable, float>("CalculateHeaderHeight");
        public float CalculateHeaderHeight()
        {
            return _CalculateHeaderHeight(Table);
        }

        static Func<PawnTable, float> _CalculateTotalRequiredHeight = FastAccess.InstanceRetMethod<PawnTable, float>("CalculateTotalRequiredHeight");
        public float CalculateTotalRequiredHeight()
        {
            return _CalculateTotalRequiredHeight(Table);
        }

        static Func<PawnTable, Pawn, bool> _CanAssignPawn = FastAccess.InstanceRetMethod<PawnTable, Pawn, bool>("CanAssignPawn");
        public bool CanAssignPawn(Pawn p)
        {
            return _CanAssignPawn(Table, p);
        }

        static Func<PawnTable, Pawn, float> _CalculateRowHeight = FastAccess.InstanceRetMethod<PawnTable, Pawn, float>("CalculateRowHeight");
        public float CalculateRowHeight(Pawn p)
        {
            return _CalculateRowHeight(Table, p);
        }

        static Action<PawnTable> _RecacheIfDirty = FastAccess.InstanceVoidMethod<PawnTable>("RecacheIfDirty");
        public void RecacheIfDirty()
        {
            _RecacheIfDirty(Table);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _LabelSortFunction = FastAccess.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("LabelSortFunction");
        public IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
        {
            return _LabelSortFunction(Table, input);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _PrimarySortFunction = FastAccess.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("PrimarySortFunction");
        public IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
        {
            return _PrimarySortFunction(Table, input);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetOptimalWidth = FastAccess.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetOptimalWidth");
        public float GetOptimalWidth(PawnColumnDef column)
        {
            return _GetOptimalWidth(Table,  column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMinWidth = FastAccess.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMinWidth");
        public float GetMinWidth(PawnColumnDef column)
        {
            return _GetMinWidth(Table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMaxWidth = FastAccess.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMaxWidth");
        public float GetMaxWidth(PawnColumnDef column)
        {
            return _GetMaxWidth(Table, column);
        }
    }

}
