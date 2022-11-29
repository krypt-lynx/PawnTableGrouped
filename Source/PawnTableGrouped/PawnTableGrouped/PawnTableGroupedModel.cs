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
        private Verse.WeakReference<PawnTable> table;
        public PawnTable Table => table?.Target;
        
        private Verse.WeakReference<MainTabWindow_PawnTable> window;
        public MainTabWindow_PawnTable Window
        {
            get => window?.Target;
            set => window = new Verse.WeakReference<MainTabWindow_PawnTable>(value);
        }


        public PawnTableDef def;

        List<GroupColumnWorker> columnResolvers;

        public GroupWorker ActiveGrouper;

        public Action<PawnTableGroupedModel> GroupsStateChanged;

        public int NumbersMagic = 0;

        public PawnTableGroupedModel(PawnTable table, PawnTableDef def)
        {
            this.table = new Verse.WeakReference<PawnTable>(table);
            this.def = def;

            columnResolvers = new List<GroupColumnWorker>();

            ReadTableConfig();

            ActiveGrouper = PredefinedGroupWorkers.First();

            LoadData(def);

            new EventBusListener<PawnTableGroupedModel, PawnTableSaveDataMessage>(this, (x, sender, args) =>
            {
                if (x.def.defName == args.Message.DefName)
                {
                    x.SaveData();
                }
            });
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
                FotterBtnOffset = info.config?.footerBtnOffset ?? FotterBtnOffset;
                AllowHScroll = info.config?.allowHScroll ?? AllowHScroll;
            }
        }


#region Save/Load

        private void LoadData(PawnTableDef def)
        {
            EventBus<PawnTableSaveDataMessage>.SendMessage(this, new PawnTableSaveDataMessage
            {
                DefName = def.defName,
            });

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
                        ActiveGrouper = PredefinedGroupWorkers.FirstOrDefault(x => groupingKey == x.Key()) ?? PredefinedGroupWorkers.First();
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
                    ActiveGrouper = PredefinedGroupWorkers.FirstOrDefault(x => info.defaultGrouping == x.Key()) ?? PredefinedGroupWorkers.First();
                } 
                else
                {
                    ActiveGrouper = PredefinedGroupWorkers.First();
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
                return Table.GetCachedHeaderHeight();
            }
        }

#region Group expanded state

        Dictionary<string, bool> ExpandedState = new Dictionary<string, bool>();

        public bool IsExpanded(PawnTableGroup group)
        {
            if (group?.Key == null)
            {
                $"trying to get expanded flag for group with 'null' title".Log(MessageType.Warning);
                return true;
            }

            if (ExpandedState.ContainsKey(group.Key))
            {
                return ExpandedState[group.Key];
            }
            else
            {
                return true;
            }
        }

        public void SwitchExpanded(PawnTableGroup g)
        {
            SetExpanded(g, !IsExpanded(g));
            SetGUIDirty();
        }

        void SetExpanded(PawnTableGroup group, bool expanded, bool updateBtnState = true)
        {
            if (group?.Key == null)
            {
                $"trying to set expanded flag for group with 'null' title".Log(MessageType.Warning);
                return;
            }

            ExpandedState[group.Key] = expanded;

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

            SetGUIDirty();

            return !needToExpand;
        }

        internal void DoGroupsStateChanged()
        {
            //OnChanged?.Invoke();
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
            tmpSortList.AddRange(Table.LabelSortFunction(pawns));
            if (Table.GetSortByColumn() != null)
            {
                if (Table.GetSortDescending())
                {
                    tmpSortList.SortStable(new Func<Pawn, Pawn, int>(Table.GetSortByColumn().Worker.Compare));
                }
                else
                {
                    tmpSortList.SortStable((Pawn a, Pawn b) => Table.GetSortByColumn().Worker.Compare(b, a));
                }
            }
            if (Mod.Settings.usePrimarySortFunction)
            {
                return Table.PrimarySortFunction(tmpSortList);
            }
            else
            {
                return tmpSortList;
            }
        }


        public void RecacheGroups()
        {            
            Groups = ActiveGrouper.CreateGroups(Table, Table.PawnsListForReading, DefaultPawnSort, columnResolvers).ToList();

            SortGroups();
        }

        private void SortGroups()
        {
            var comparer = ActiveGrouper.GroupsSortingComparer;
            if (SortDecending)
            {
                Groups.Sort((a, b) => comparer.Compare(b, a));
            }
            else
            {
                Groups.Sort(comparer);
            }
        }

        public void SetGrouper(GroupWorker group)
        {
            ActiveGrouper = group;
            SetGUIDirty();
        }

        public void SetSortingDecending(bool value)
        {
            SortDecending = value;
            SortGroups();
        }

        public IEnumerable<GroupWorker> PredefinedGroupWorkers
        {
            get
            {
                return Mod.GroupWorkers;
            }
        }

        public IEnumerable<GroupWorker> ColumnGroupWorkers
        {
            get
            {
                return columnResolvers.Select(x => x.GroupWorker).Where(x => x != null);
            }
        }



        #endregion

        public void SetGUIDirty()
        {
            if (Window != null)
            {
                Window.SetDirty();
            }
            else
            {
                Table.SetDirty();
            }
        }

        public void RecacheColumnResolvers()
        {
            columnResolvers.Clear();
            foreach (var column in Table.Columns())
            {
                var resolverDef = GroupColumnDefResolver.GetResolver(column);
                columnResolvers.Add(resolverDef?.Worker);
            }
        }



        public float CalculateRowHeight(Pawn pawn)
        {
            return Table.CalculateRowHeight(pawn);
        }

    }

}
