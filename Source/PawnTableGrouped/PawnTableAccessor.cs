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

        static Func<PawnTable, bool> _get_dirty = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, bool>("dirty");
        static Action<PawnTable, bool> _set_dirty = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, bool>("dirty");

        public bool dirty
        {
            get => _get_dirty(Table);
            set => _set_dirty(Table, value);
        }

        static Func<PawnTable, bool> _get_hasFixedSize = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, bool>("hasFixedSize");
        static Action<PawnTable, bool> _set_hasFixedSize = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, bool>("hasFixedSize");

        public bool hasFixedSize
        {
            get => _get_hasFixedSize(Table);
            set => _set_hasFixedSize(Table, value);
        }

        static Func<PawnTable, float> _get_cachedHeaderHeight = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, float>("cachedHeaderHeight");
        static Action<PawnTable, float> _set_cachedHeaderHeight = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, float>("cachedHeaderHeight");
        public float cachedHeaderHeight
        {
            get => _get_cachedHeaderHeight(Table);
            set => _set_cachedHeaderHeight(Table, value);
        }

        static Func<PawnTable, float> _get_cachedHeightNoScrollbar = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, float>("cachedHeightNoScrollbar");
        static Action<PawnTable, float> _set_cachedHeightNoScrollbar = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, float>("cachedHeightNoScrollbar");
        public float cachedHeightNoScrollbar
        {
            get => _get_cachedHeightNoScrollbar(Table);
            set => _set_cachedHeightNoScrollbar(Table, value);
        }

        static Func<PawnTable, List<Pawn>> _get_cachedPawns = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, List<Pawn>>("cachedPawns");
        static Action<PawnTable, List<Pawn>> _set_cachedPawns = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, List<Pawn>>("cachedPawns");
        public List<Pawn> cachedPawns
        {
            get => _get_cachedPawns(Table);
            set => _set_cachedPawns(Table, value);
        }

        static Func<PawnTable, Vector2> _get_cachedSize = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, Vector2>("cachedSize");
        static Action<PawnTable, Vector2> _set_cachedSize = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, Vector2>("cachedSize");
        public Vector2 cachedSize
        {
            get => _get_cachedSize(Table);
            set => _set_cachedSize(Table, value);
        }


        static Func<PawnTable, List<float>> _get_cachedColumnWidths = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, List<float>>("cachedColumnWidths");
        static Action<PawnTable, List<float>> _set_cachedColumnWidths = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, List<float>>("cachedColumnWidths");
        public List<float> cachedColumnWidths
        {
            get => _get_cachedColumnWidths(Table);
            set => _set_cachedColumnWidths(Table, value);
        }


        static Func<PawnTable, List<bool>> _get_columnAtOptimalWidth = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, List<bool>>("columnAtOptimalWidth");
        static Action<PawnTable, List<bool>> _set_columnAtOptimalWidth = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, List<bool>>("columnAtOptimalWidth");
        public List<bool> columnAtOptimalWidth
        {
            get => _get_columnAtOptimalWidth(Table);
            set => _set_columnAtOptimalWidth(Table, value);
        }


        static Func<PawnTable, List<bool>> _get_columnAtMaxWidth = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, List<bool>>("columnAtMaxWidth");
        static Action<PawnTable, List<bool>> _set_columnAtMaxWidth = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, List<bool>>("columnAtMaxWidth");
        public List<bool> columnAtMaxWidth
        {
            get => _get_columnAtMaxWidth(Table);
            set => _set_columnAtMaxWidth(Table, value);
        }



        static Func<PawnTable, int> _get_minTableWidth = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, int>("minTableWidth");
        static Action<PawnTable, int> _set_minTableWidth = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, int>("minTableWidth");
        public int minTableWidth
        {
            get => _get_minTableWidth(Table);
            set => _set_minTableWidth(Table, value);
        }


        static Func<PawnTable, int> _get_maxTableWidth = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, int>("maxTableWidth");
        static Action<PawnTable, int> _set_maxTableWidth = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, int>("maxTableWidth");
        public int maxTableWidth
        {
            get => _get_maxTableWidth(Table);
            set => _set_maxTableWidth(Table, value);
        }


        static Func<PawnTable, int> _get_minTableHeight = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, int>("minTableHeight");
        static Action<PawnTable, int> _set_minTableHeight = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, int>("minTableHeight");
        public int minTableHeight
        {
            get => _get_minTableHeight(Table);
            set => _set_minTableHeight(Table, value);
        }


        static Func<PawnTable, int> _get_maxTableHeight = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, int>("maxTableHeight");
        static Action<PawnTable, int> _set_maxTableHeight = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, int>("maxTableHeight");
        public int maxTableHeight
        {
            get => _get_maxTableHeight(Table);
            set => _set_maxTableHeight(Table, value);
        }


        static Func<PawnTable, PawnColumnDef> _get_sortByColumn = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, PawnColumnDef>("sortByColumn");
        static Action<PawnTable, PawnColumnDef> _set_sortByColumn = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, PawnColumnDef>("sortByColumn");
        public PawnColumnDef sortByColumn
        {
            get => _get_sortByColumn(Table);
            set => _set_sortByColumn(Table, value);
        }


        static Func<PawnTable, bool> _get_sortDescending = FastAccess.CreateGetInstanceFieldWrapper<PawnTable, bool>("sortDescending");
        static Action<PawnTable, bool> _set_sortDescending = FastAccess.CreateSetInstanceFieldWrapper<PawnTable, bool>("sortDescending");
        public bool sortDescending
        {
            get => _get_sortDescending(Table);
            set => _set_sortDescending(Table, value);
        }

        static Action<PawnTable> _RecachePawns = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecachePawns");
        public void RecachePawns()
        {
            _RecachePawns(Table);
        }

        static Action<PawnTable> _RecacheRowHeights = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecacheRowHeights");
        public void RecacheRowHeights()
        {
            _RecacheRowHeights(Table);
        }

        static Action<PawnTable> _RecacheSize = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecacheSize");
        public void RecacheSize()
        {
            _RecacheSize(Table);
        }

        static Action<PawnTable> _RecacheColumnWidths = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecacheColumnWidths");
        public void RecacheColumnWidths()
        {
            _RecacheColumnWidths(Table);
        }

        static Action<PawnTable> _RecacheLookTargets = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecacheLookTargets");
        public void RecacheLookTargets()
        {
            _RecacheLookTargets(Table);
        }

        static Func<PawnTable, float> _CalculateHeaderHeight = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, float>("CalculateHeaderHeight");
        public float CalculateHeaderHeight()
        {
            return _CalculateHeaderHeight(Table);
        }

        static Func<PawnTable, float> _CalculateTotalRequiredHeight = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, float>("CalculateTotalRequiredHeight");
        public float CalculateTotalRequiredHeight()
        {
            return _CalculateTotalRequiredHeight(Table);
        }

        static Func<PawnTable, Pawn, bool> _CanAssignPawn = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, Pawn, bool>("CanAssignPawn");
        public bool CanAssignPawn(Pawn p)
        {
            return _CanAssignPawn(Table, p);
        }

        static Func<PawnTable, Pawn, float> _CalculateRowHeight = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, Pawn, float>("CalculateRowHeight");
        public float CalculateRowHeight(Pawn p)
        {
            return _CalculateRowHeight(Table, p);
        }

        static Action<PawnTable> _RecacheIfDirty = FastAccess.CreateInstanceVoidMethodWrapper<PawnTable>("RecacheIfDirty");
        public void RecacheIfDirty()
        {
            _RecacheIfDirty(Table);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _LabelSortFunction = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("LabelSortFunction");
        public IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
        {
            return _LabelSortFunction(Table, input);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _PrimarySortFunction = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("PrimarySortFunction");
        public IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
        {
            return _PrimarySortFunction(Table, input);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetOptimalWidth = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, PawnColumnDef, float>("GetOptimalWidth");
        public float GetOptimalWidth(PawnColumnDef column)
        {
            return _GetOptimalWidth(Table,  column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMinWidth = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, PawnColumnDef, float>("GetMinWidth");
        public float GetMinWidth(PawnColumnDef column)
        {
            return _GetMinWidth(Table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMaxWidth = FastAccess.CreateInstanceRetMethodWrapper<PawnTable, PawnColumnDef, float>("GetMaxWidth");
        public float GetMaxWidth(PawnColumnDef column)
        {
            return _GetMaxWidth(Table, column);
        }
    }

}
