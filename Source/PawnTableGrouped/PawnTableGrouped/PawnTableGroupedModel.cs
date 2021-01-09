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

        List<GroupWorker> miscGroupers;
        public GroupWorker ActiveGrouper;


        public Action<PawnTableGroupedModel> GroupsStateChanged;

        public int NumbersMagic = 0;

        public PawnTableGroupedModel(PawnTable table, PawnTableAccessor accessor, PawnTableDef def)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            this.accessor = accessor;
            this.def = def;

            columnResolvers = new List<GroupColumnWorker>();
            miscGroupers = new List<GroupWorker>();

            miscGroupers.Add(new GroupWorker_AllInOne());
            miscGroupers.Add(new GroupWorker_ByRace());
            miscGroupers.Add(new GroupWorker_ByGender());
            miscGroupers.Add(new GroupWorker_ByFaction());

            ActiveGrouper = AllGroupers.First();

            LoadData(def);
        }

        ~PawnTableGroupedModel() 
        {
            $"~PawnTableGroupedModel {def.defName}".Log();
            ForceSaveData();
        }

        #region Save/Load

        private void LoadData(PawnTableDef def)
        {
            // trying to access data saved in savegame
            var store = Current.Game.GetComponent<DataStore>();
            store.RegisterModel(this);
            var data = store.GetData(def.defName);
            if (data != null)
            {
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
                // have no data, using defaults
                string groupingKey = null;
                if (Mod.DefaultTableConfig.TryGetValue(def.defName, out groupingKey))
                {
                    ActiveGrouper = AllGroupers.FirstOrDefault(x => groupingKey == x.Key()) ?? AllGroupers.First();
                } 
                else
                {
                    ActiveGrouper = AllGroupers.First();
                }
            }
        }

        private void ForceSaveData()
        {
            var store = Current.Game.GetComponent<DataStore>();

            string key;
            var data = SaveData(out key);
            store.UpdateData(key, data);
        }

        public Dictionary<string, IExposable> SaveData(out string Key)
        {
            Key = def.defName;

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
            if (group?.Title == null)
            {
                $"trying to get expanded flag for group with 'null' title".Log(LogHelper.MessageType.Warning);
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
            if (group?.Title == null)
            {
                $"trying to set expanded flag for group with 'null' title".Log(LogHelper.MessageType.Warning);
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

        public void RecacheGroups()
        {
            var pawnGroups = Table.PawnsListForReading
                .GroupBy(p => p, ActiveGrouper.GroupingEqualityComparer);

            Groups = new List<PawnTableGroup>();
            foreach (var group in pawnGroups)
            {
                var pawns = accessor.LabelSortFunction(group).ToList();
                if (accessor.sortByColumn != null)
                {
                    if (accessor.sortDescending)
                    {
                        pawns.SortStable(new Func<Pawn, Pawn, int>(accessor.sortByColumn.Worker.Compare));
                    }
                    else
                    {
                        pawns.SortStable((Pawn a, Pawn b) => accessor.sortByColumn.Worker.Compare(b, a));
                    }
                }
                //var pawns2 = accessor.PrimarySortFunction(pawns);
                Groups.Add(new PawnTableGroup(ActiveGrouper.TitleForGroup(group, group.Key), group.Key, pawns, columnResolvers));
            }

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
                    return miscGroupers.Concat(columnResolvers.Select(x => x.GroupWorker).Where(x => x != null));
                }
                else
                {
                    return miscGroupers;
                }
            }
        }

        #endregion


        public void RecacheColumnResolvers()
        {
            columnResolvers.Clear();
            foreach (var column in Table.ColumnsListForReading)
            {
                var resolverDef = GroupColumnDefResolver.GetResolverSilentFail(column);
                columnResolvers.Add(resolverDef?.Worker);


            }
        }



        public float CalculateRowHeight(Pawn pawn)
        {
            return accessor.CalculateRowHeight(pawn);
        }

    }

}
