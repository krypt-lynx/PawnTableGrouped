using RimWorld;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    public static class PawnTableWrapper
    {
        static Getter<PawnTable, bool> _get_dirty = Dynamic.InstanceGetField<PawnTable, bool>("dirty");
        static Setter<PawnTable, bool> _set_dirty = Dynamic.InstanceSetField<PawnTable, bool>("dirty");
        public static bool GetDirty(this PawnTable table) => _get_dirty(table);
        public static void SetDirty(this PawnTable table, bool value) => _set_dirty(table, value);

        static Getter<PawnTable, bool> _get_hasFixedSize = Dynamic.InstanceGetField<PawnTable, bool>("hasFixedSize");
        static Setter<PawnTable, bool> _set_hasFixedSize = Dynamic.InstanceSetField<PawnTable, bool>("hasFixedSize");
        public static bool GetHasFixedSize(this PawnTable table) => _get_hasFixedSize(table);
        public static void SetHasFixedSize(this PawnTable table, bool value) => _set_hasFixedSize(table, value);

        static Getter<PawnTable, float> _get_cachedHeaderHeight = Dynamic.InstanceGetField<PawnTable, float>("cachedHeaderHeight");
        static Setter<PawnTable, float> _set_cachedHeaderHeight = Dynamic.InstanceSetField<PawnTable, float>("cachedHeaderHeight");
        public static float GetCachedHeaderHeight(this PawnTable table) => _get_cachedHeaderHeight(table);
        public static void SetCachedHeaderHeight(this PawnTable table, float value) => _set_cachedHeaderHeight(table, value);


        static Getter<PawnTable, float> _get_cachedHeightNoScrollbar = Dynamic.InstanceGetField<PawnTable, float>("cachedHeightNoScrollbar");
        static Setter<PawnTable, float> _set_cachedHeightNoScrollbar = Dynamic.InstanceSetField<PawnTable, float>("cachedHeightNoScrollbar");
        public static float GetCachedHeightNoScrollbar(this PawnTable table) => _get_cachedHeightNoScrollbar(table);
        public static void SetCachedHeightNoScrollbar(this PawnTable table, float value) => _set_cachedHeightNoScrollbar(table, value);

        static Getter<PawnTable, List<Pawn>> _get_cachedPawns = Dynamic.InstanceGetField<PawnTable, List<Pawn>>("cachedPawns");
        static Setter<PawnTable, List<Pawn>> _set_cachedPawns = Dynamic.InstanceSetField<PawnTable, List<Pawn>>("cachedPawns");
        public static List<Pawn> GetCachedPawns(this PawnTable table) => _get_cachedPawns(table);
        public static void SetCachedPawns(this PawnTable table, List<Pawn> value) => _set_cachedPawns(table, value);

        static Getter<PawnTable, Vector2> _get_cachedSize = Dynamic.InstanceGetField<PawnTable, Vector2>("cachedSize");
        static Setter<PawnTable, Vector2> _set_cachedSize = Dynamic.InstanceSetField<PawnTable, Vector2>("cachedSize");
        public static Vector2 GetCachedSize(this PawnTable table) => _get_cachedSize(table);
        public static void SetCachedSize(this PawnTable table, Vector2 value) => _set_cachedSize(table, value);


        static Getter<PawnTable, List<float>> _get_cachedColumnWidths = Dynamic.InstanceGetField<PawnTable, List<float>>("cachedColumnWidths");
        static Setter<PawnTable, List<float>> _set_cachedColumnWidths = Dynamic.InstanceSetField<PawnTable, List<float>>("cachedColumnWidths");
        public static List<float> GetCachedColumnWidths(this PawnTable table) => _get_cachedColumnWidths(table);
        public static void SetCachedColumnWidths(this PawnTable table, List<float> value) => _set_cachedColumnWidths(table, value);

        static Getter<PawnTable, List<bool>> _get_columnAtOptimalWidth = Dynamic.InstanceGetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        static Setter<PawnTable, List<bool>> _set_columnAtOptimalWidth = Dynamic.InstanceSetField<PawnTable, List<bool>>("columnAtOptimalWidth");
        public static List<bool> GetColumnAtOptimalWidth(this PawnTable table) => _get_columnAtOptimalWidth(table);
        public static void SetColumnAtOptimalWidth(this PawnTable table, List<bool> value) => _set_columnAtOptimalWidth(table, value);


        static Getter<PawnTable, List<bool>> _get_columnAtMaxWidth = Dynamic.InstanceGetField<PawnTable, List<bool>>("columnAtMaxWidth");
        static Setter<PawnTable, List<bool>> _set_columnAtMaxWidth = Dynamic.InstanceSetField<PawnTable, List<bool>>("columnAtMaxWidth");
        public static List<bool> GetColumnAtMaxWidth(this PawnTable table) => _get_columnAtMaxWidth(table);
        public static void SetColumnAtMaxWidth(this PawnTable table, List<bool> value) => _set_columnAtMaxWidth(table, value);



        static Getter<PawnTable, int> _get_minTableWidth = Dynamic.InstanceGetField<PawnTable, int>("minTableWidth");
        static Setter<PawnTable, int> _set_minTableWidth = Dynamic.InstanceSetField<PawnTable, int>("minTableWidth");
        public static int GetMinTableWidth(this PawnTable table) => _get_minTableWidth(table);
        public static void SetMinTableWidth(this PawnTable table, int value) => _set_minTableWidth(table, value);


        static Getter<PawnTable, int> _get_maxTableWidth = Dynamic.InstanceGetField<PawnTable, int>("maxTableWidth");
        static Setter<PawnTable, int> _set_maxTableWidth = Dynamic.InstanceSetField<PawnTable, int>("maxTableWidth");
        public static int GetMaxTableWidth(this PawnTable table) => _get_maxTableWidth(table);
        public static void SetMaxTableWidth(this PawnTable table, int value) => _set_maxTableWidth(table, value);


        static Getter<PawnTable, int> _get_minTableHeight = Dynamic.InstanceGetField<PawnTable, int>("minTableHeight");
        static Setter<PawnTable, int> _set_minTableHeight = Dynamic.InstanceSetField<PawnTable, int>("minTableHeight");
        public static int GetMinTableHeight(this PawnTable table) => _get_minTableHeight(table);
        public static void SetMinTableHeight(this PawnTable table, int value) => _set_minTableHeight(table, value);


        static Getter<PawnTable, int> _get_maxTableHeight = Dynamic.InstanceGetField<PawnTable, int>("maxTableHeight");
        static Setter<PawnTable, int> _set_maxTableHeight = Dynamic.InstanceSetField<PawnTable, int>("maxTableHeight");
        public static int GetMaxTableHeight(this PawnTable table) => _get_maxTableHeight(table);
        public static void SetMaxTableHeight(this PawnTable table, int value) => _set_maxTableHeight(table, value);


        static Getter<PawnTable, PawnColumnDef> _get_sortByColumn = Dynamic.InstanceGetField<PawnTable, PawnColumnDef>("sortByColumn");
        static Setter<PawnTable, PawnColumnDef> _set_sortByColumn = Dynamic.InstanceSetField<PawnTable, PawnColumnDef>("sortByColumn");
        public static PawnColumnDef GetSortByColumn(this PawnTable table) => _get_sortByColumn(table);
        public static void SetSortByColumn(this PawnTable table, PawnColumnDef value) => _set_sortByColumn(table, value);


        static Getter<PawnTable, bool> _get_sortDescending = Dynamic.InstanceGetField<PawnTable, bool>("sortDescending");
        static Setter<PawnTable, bool> _set_sortDescending = Dynamic.InstanceSetField<PawnTable, bool>("sortDescending");
        public static bool GetSortDescending(this PawnTable table) => _get_sortDescending(table);
        public static void SetSortDescending(this PawnTable table, bool value) => _set_sortDescending(table, value);

        static Action<PawnTable> _RecachePawns = Dynamic.InstanceVoidMethod<PawnTable>("RecachePawns");
        public static void RecachePawns(this PawnTable table)
        {
            _RecachePawns(table);
        }

        static Action<PawnTable> _RecacheRowHeights = Dynamic.InstanceVoidMethod<PawnTable>("RecacheRowHeights");
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RecacheRowHeights(this PawnTable table)
        {
            _RecacheRowHeights(table);
        }

        static Action<PawnTable> _RecacheSize = Dynamic.InstanceVoidMethod<PawnTable>("RecacheSize");
        public static void RecacheSize(this PawnTable table)
        {
            _RecacheSize(table);
        }

        static Action<PawnTable> _RecacheColumnWidths = Dynamic.InstanceVoidMethod<PawnTable>("RecacheColumnWidths");
        public static void RecacheColumnWidths(this PawnTable table)
        {
            _RecacheColumnWidths(table);
        }

        static Action<PawnTable> _RecacheLookTargets = Dynamic.InstanceVoidMethod<PawnTable>("RecacheLookTargets");
        public static void RecacheLookTargets(this PawnTable table)
        {
            _RecacheLookTargets(table);
        }

        static Func<PawnTable, float> _CalculateHeaderHeight = Dynamic.InstanceRetMethod<PawnTable, float>("CalculateHeaderHeight");
        public static float CalculateHeaderHeight(this PawnTable table)
        {
            return _CalculateHeaderHeight(table);
        }

        static Func<PawnTable, float> _CalculateTotalRequiredHeight = Dynamic.InstanceRetMethod<PawnTable, float>("CalculateTotalRequiredHeight");
        public static float CalculateTotalRequiredHeight(this PawnTable table)
        {
            return _CalculateTotalRequiredHeight(table);
        }

#if rw_1_3_or_earlier
        static Func<PawnTable, Pawn, bool> _CanAssignPawn = Dynamic.InstanceRetMethod<PawnTable, Pawn, bool>("CanAssignPawn");
        public static bool CanAssignPawn(this PawnTable table, Pawn p)
        {
            return _CanAssignPawn(table, p);
        }
#endif

        static Func<PawnTable, Pawn, float> _CalculateRowHeight = Dynamic.InstanceRetMethod<PawnTable, Pawn, float>("CalculateRowHeight");
        public static float CalculateRowHeight(this PawnTable table, Pawn p)
        {
            return _CalculateRowHeight(table, p);
        }

        static Action<PawnTable> _RecacheIfDirty = Dynamic.InstanceVoidMethod<PawnTable>("RecacheIfDirty");
        public static void RecacheIfDirty(this PawnTable table)
        {
            _RecacheIfDirty(table);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _LabelSortFunction = Dynamic.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("LabelSortFunction");
        public static IEnumerable<Pawn> LabelSortFunction(this PawnTable table, IEnumerable<Pawn> input)
        {
            return _LabelSortFunction(table, input);
        }

        static Func<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>> _PrimarySortFunction = Dynamic.InstanceRetMethod<PawnTable, IEnumerable<Pawn>, IEnumerable<Pawn>>("PrimarySortFunction");
        public static IEnumerable<Pawn> PrimarySortFunction(this PawnTable table, IEnumerable<Pawn> input)
        {
            return _PrimarySortFunction(table, input);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetOptimalWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetOptimalWidth");
        public static float GetOptimalWidth(this PawnTable table, PawnColumnDef column)
        {
            return _GetOptimalWidth(table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMinWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMinWidth");
        public static float GetMinWidth(this PawnTable table, PawnColumnDef column)
        {
            return _GetMinWidth(table, column);
        }

        static Func<PawnTable, PawnColumnDef, float> _GetMaxWidth = Dynamic.InstanceRetMethod<PawnTable, PawnColumnDef, float>("GetMaxWidth");
        public static float GetMaxWidth(this PawnTable table, PawnColumnDef column)
        {
            return _GetMaxWidth(table, column);
        }

        public static List<PawnColumnDef> Columns(this PawnTable table)
        {
#if rw_1_4_or_later
            return table.Columns;
#else
            return table.ColumnsListForReading;
#endif
        }
    }

}
