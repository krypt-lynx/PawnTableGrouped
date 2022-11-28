using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PawnTableGrouped
{
    public class GroupColumnWorker_Checkbox : GroupColumnWorker_CheckboxBase
    {
        static Func<PawnColumnWorker_Checkbox, Pawn, bool> getValue = Dynamic.InstanceRetMethod<PawnColumnWorker_Checkbox, Pawn, bool>(
            typeof(PawnColumnWorker_Checkbox).GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance));
#if rw_1_2_or_earlier
        static Action<PawnColumnWorker_Checkbox, Pawn, bool> setValue = Dynamic.InstanceVoidMethod<PawnColumnWorker_Checkbox, Pawn, bool>(
            typeof(PawnColumnWorker_Checkbox).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance));
#else
        static Action<PawnColumnWorker_Checkbox, Pawn, bool, PawnTable> setValue = Dynamic.InstanceVoidMethod<PawnColumnWorker_Checkbox, Pawn, bool, PawnTable>(
            typeof(PawnColumnWorker_Checkbox).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance));
#endif
        static Func<PawnColumnWorker_Checkbox, Pawn, bool> hasCheckbox = Dynamic.InstanceRetMethod<PawnColumnWorker_Checkbox, Pawn, bool>(
            typeof(PawnColumnWorker_Checkbox).GetMethod("HasCheckbox", BindingFlags.NonPublic | BindingFlags.Instance));
  


        public override object GetValue(Pawn pawn) => getValue((PawnColumnWorker_Checkbox)ColumnDef.Worker, pawn);

        public override void SetValue(Pawn pawn, object value, PawnTable table)
        {
            if (HasCheckbox(pawn))
            {
#if rw_1_2_or_earlier
                setValue((PawnColumnWorker_Checkbox)ColumnDef.Worker, pawn, (bool)value);
#else
                setValue((PawnColumnWorker_Checkbox)ColumnDef.Worker, pawn, (bool)value, table);
#endif
            }
        }

        public bool HasCheckbox(Pawn pawn) => hasCheckbox((PawnColumnWorker_Checkbox)ColumnDef.Worker, pawn);

        public override bool CanSetValue(Pawn pawn) => HasCheckbox(pawn);

        public override object DefaultValue(IEnumerable<Pawn> pawns) => true;

        public override bool IsVisible(Pawn pawn) => HasCheckbox(pawn);
    }
}
