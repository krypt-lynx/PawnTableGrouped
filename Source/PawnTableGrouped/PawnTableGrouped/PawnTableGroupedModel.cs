using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
    public interface IDataOwner
    {
        Dictionary<string, IExposable> SaveData(out string key);
    }

    public class PawnTableGroupedModel : IDataOwner
    {
        Verse.WeakReference<PawnTable> table; 
        public PawnTable Table => table.Target;

        public PawnTableAccessor accessor;
        public PawnTableDef def;

        List<GroupColumnWorker> columnResolvers;

        public GroupWorker ActiveGrouper;


        public Action<PawnTableGroupedModel> GroupsStateChanged;

        public int NumbersMagic = 0;

        public PawnTableGroupedModel(PawnTable table, PawnTableAccessor accessor, PawnTableDef def)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            this.accessor = accessor;
            this.def = def;

            columnResolvers = new List<GroupColumnWorker>();

            ReadTableConfig();

            ActiveGrouper = AllGroupers.First();

            LoadData(def);
        }

        public float TableExtendedArea = 0;
        public float FotterBtnOffset = 0;
        public bool AllowHScroll = false;

        private void ReadTableConfig()
        {
            TableInfo info = null;
            if (CompatibilityInfoDef.CurrentTables.TryGetValue(def.defName, out info))
            {
                TableExtendedArea = info.config?.expandedBottomSpace ?? TableExtendedArea;
                FotterBtnOffset = info.config?.fotterBtnOffset ?? FotterBtnOffset;
                AllowHScroll = info.config?.allowHScroll ?? AllowHScroll;
            }
        }


#region Save/Load

        private void LoadData(PawnTableDef def)
        {
            $"LoadData {def.defName}...".Log();
            
            // trying to access data saved in savegame
            var store = Current.Game.GetComponent<DataStore>();
            store.RegisterModel(this);
            var data = store.GetData(def.defName);
            if (data != null)
            {
                $"Restored".Log();
                // have data, reading it
                IExposable value = null;
                if (data.TryGetValue("groupsState", out value))
                {
                    ExpandedState = (value as DictionaryContainer<string, bool>)?.Value ?? ExpandedState;
                }
                if (data.TryGetValue("sortDecending", out value))
                {
                    SortDecending = (value as ValueContainer<bool>)?.Value ?? SortDecending;
                }
                if (data.TryGetValue("groupingBy", out value))
                {
                    var groupingKey = (value as ValueContainer<string>)?.Value;
                    if (groupingKey != null)
                    {
                        ActiveGrouper = AllGroupers.FirstOrDefault(x => groupingKey == x.Key()) ?? AllGroupers.First();
                    }
                }
            } 
            else
            {
                $"Defaults".Log();
                // have no data, using defaults
                TableInfo info = null;
                if (CompatibilityInfoDef.CurrentTables.TryGetValue(def.defName, out info))
                {
                    ActiveGrouper = AllGroupers.FirstOrDefault(x => info.defaultGrouping == x.Key()) ?? AllGroupers.First();
                } 
                else
                {
                    ActiveGrouper = AllGroupers.First();
                }
            }
        }

        public void SaveData()
        {
            var store = Current.Game.GetComponent<DataStore>();

            string key;
            var data = SaveData(out key);
            store.UpdateData(key, data);
        }

        public Dictionary<string, IExposable> SaveData(out string Key)
        {
            Key = def.defName;
            $"SaveData {Key}".Log();

            var data =  new Dictionary<string, IExposable>();
            data["groupsState"] = (DictionaryContainer<string, bool>)ExpandedState;
            data["sortDecending"] = (ValueContainer<bool>)SortDecending;
            data["groupingBy"] = (ValueContainer<string>)ActiveGrouper.Key();
            return data;
        }

#endregion

        public List<PawnTableGroup> Groups;
        public bool SortDecending = false;

        public float cachedHeaderHeight
        {
            get
            {
                return accessor.cachedHeaderHeight;
            }
        }

#region Group expanded state

        Dictionary<string, bool> ExpandedState = new Dictionary<string, bool>();

        public bool IsExpanded(PawnTableGroup group)
        {
            if (group?.Title.RawText == null)
            {
                $"trying to get expanded flag for group with 'null' title".Log(MessageType.Warning);
                return true;
            }

            if (ExpandedState.ContainsKey(group.Title))
            {
                return ExpandedState[group.Title];
            }
            else
            {
                return true;
            }
        }

        public void SwitchExpanded(PawnTableGroup g)
        {
            SetExpanded(g, !IsExpanded(g));
            Table.SetDirty();
        }

        void SetExpanded(PawnTableGroup group, bool expanded, bool updateBtnState = true)
        {
            if (group?.Title.RawText == null)
            {
                $"trying to set expanded flag for group with 'null' title".Log(MessageType.Warning);
                return;
            }

            ExpandedState[group.Title] = expanded;

            if (updateBtnState)
            {
                DoGroupsStateChanged();
            }
        }

        public bool ExpandCollapse()
        {
            bool needToExpand = true;
            foreach (var group in Groups)
            {
                if (IsExpanded(group))
                {
                    needToExpand = false;
                }
            }
            foreach (var group in Groups)
            {
                SetExpanded(group, needToExpand, false);
            }

            Table.SetDirty();

            return !needToExpand;
        }

        internal void DoGroupsStateChanged()
        {
            GroupsStateChanged?.Invoke(this);
        }

#endregion

#region Group management

        /// <summary>
        /// temporary sorting array
        /// </summary>
        List<Pawn> tmpSortList = new List<Pawn>();

        /// <summary>
        /// Sorts pawns using default-ish PawnTable sorting
        /// </summary>
        /// <param name="pawns"></param>
        /// <returns>sorted pawns</returns>
        /// <remarks>returned object must not be stored</remarks>
        public IEnumerable<Pawn> DefaultPawnSort(IEnumerable<Pawn> pawns)
        {
            tmpSortList.Clear();
            tmpSortList.AddRange(accessor.LabelSortFunction(pawns));
            if (accessor.sortByColumn != null)
            {
                if (accessor.sortDescending)
                {
                    tmpSortList.SortStable(new Func<Pawn, Pawn, int>(accessor.sortByColumn.Worker.Compare));
                }
                else
                {
                    tmpSortList.SortStable((Pawn a, Pawn b) => accessor.sortByColumn.Worker.Compare(b, a));
                }
            }
            if (Mod.Settings.usePrimarySortFunction)
            {
                return accessor.PrimarySortFunction(tmpSortList);
            }
            else
            {
                return tmpSortList;
            }
        }


        public void RecacheGroups()
        {
            Groups = ActiveGrouper.CreateGroups(Table.PawnsListForReading, DefaultPawnSort, columnResolvers).ToList();

            SortGroups();
        }

        private void SortGroups()
        {
            if (SortDecending)
            {
                Groups.Sort((a, b) => ActiveGrouper.GroupsSortingComparer.Compare(b, a));
            }
            else
            {
                Groups.Sort(ActiveGrouper.GroupsSortingComparer);
            }
        }

        public void SetGrouper(GroupWorker group)
        {
            ActiveGrouper = group;
            Table.SetDirty();
        }

        public void SetSortingDecending(bool value)
        {
            SortDecending = value;
            SortGroups();
        }

        public IEnumerable<GroupWorker> AllGroupers
        {
            get
            {
                if (Mod.Settings.groupByColumnExperimental)
                {
                    return Mod.MiscGroupWorkers.Concat(columnResolvers.Select(x => x.GroupWorker).Where(x => x != null));
                }
                else
                {
                    return Mod.MiscGroupWorkers;
                }
            }
        }

#endregion


        public void RecacheColumnResolvers()
        {
            columnResolvers.Clear();
            foreach (var column in Table.ColumnsListForReading)
            {
                var resolverDef = GroupColumnDefResolver.GetResolver(column);
                columnResolvers.Add(resolverDef?.Worker);


            }
        }



        public float CalculateRowHeight(Pawn pawn)
        {
            return accessor.CalculateRowHeight(pawn);
        }

    }

}
