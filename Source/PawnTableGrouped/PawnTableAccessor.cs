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

        public bool dirty
        {
            get
            {
                return (bool)typeof(PawnTable).GetField("dirty", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("dirty", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public bool hasFixedSize
        {
            get
            {
                return (bool)typeof(PawnTable).GetField("hasFixedSize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("hasFixedSize", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public float cachedHeaderHeight
        {
            get
            {
                return (float)typeof(PawnTable).GetField("cachedHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public float cachedHeightNoScrollbar
        {
            get
            {
                return (float)typeof(PawnTable).GetField("cachedHeightNoScrollbar", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedHeightNoScrollbar", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<Pawn> cachedPawns
        {
            get
            {
                return (List<Pawn>)typeof(PawnTable).GetField("cachedPawns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedPawns", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public Vector2 cachedSize
        {
            get
            {
                return (Vector2)typeof(PawnTable).GetField("cachedSize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedSize", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<float> cachedColumnWidths
        {
            get
            {
                return (List<float>)typeof(PawnTable).GetField("cachedColumnWidths", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("cachedColumnWidths", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<bool> columnAtOptimalWidth
        {
            get
            {
                return (List<bool>)typeof(PawnTable).GetField("columnAtOptimalWidth", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("columnAtOptimalWidth", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public List<bool> columnAtMaxWidth
        {
            get
            {
                return (List<bool>)typeof(PawnTable).GetField("columnAtMaxWidth", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("columnAtMaxWidth", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }


        public int minTableHeight
        {
            get
            {
                return (int)typeof(PawnTable).GetField("minTableHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("minTableHeight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public int maxTableHeight
        {
            get
            {
                return (int)typeof(PawnTable).GetField("maxTableHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("maxTableHeight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }


        public PawnColumnDef sortByColumn
        {
            get
            {
                return (PawnColumnDef)typeof(PawnTable).GetField("sortByColumn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("sortByColumn", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public bool sortDescending
        {
            get
            {
                return (bool)typeof(PawnTable).GetField("sortDescending", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Table);
            }
            set
            {
                typeof(PawnTable).GetField("sortDescending", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Table, value);
            }
        }

        public void RecachePawns()
        {
            typeof(PawnTable).GetMethod("RecachePawns", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheRowHeights()
        {
            typeof(PawnTable).GetMethod("RecacheRowHeights", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheSize()
        {
            typeof(PawnTable).GetMethod("RecacheSize", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheColumnWidths()
        {
            typeof(PawnTable).GetMethod("RecacheColumnWidths", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public void RecacheLookTargets()
        {
            typeof(PawnTable).GetMethod("RecacheLookTargets", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public float CalculateHeaderHeight()
        {
            return (float)typeof(PawnTable).GetMethod("CalculateHeaderHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public float CalculateTotalRequiredHeight()
        {
            return (float)typeof(PawnTable).GetMethod("CalculateTotalRequiredHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }
        public bool CanAssignPawn(Pawn p)
        {
            return (bool)typeof(PawnTable).GetMethod("CanAssignPawn", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { p });
        }
        public float CalculateRowHeight(Pawn p)
        {
            return (float)typeof(PawnTable).GetMethod("CalculateRowHeight", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { p });
        }
        public void RecacheIfDirty()
        {
            typeof(PawnTable).GetMethod("RecacheIfDirty", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { });
        }

        public IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
        {
           return (IEnumerable<Pawn>)typeof(PawnTable).GetMethod("LabelSortFunction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { input  });
        }

        public IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
        {
            return (IEnumerable<Pawn>)typeof(PawnTable).GetMethod("PrimarySortFunction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Table, new object[] { input });
        }

    }

}
