using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PawnTableGrouped
{
	public class ListGroupHeaderMapping
	{
		public string columnWorkerType;
		public string groupWorkerType;
		public object workerConfig;
	}

	public class WorkerTypeWithConfig
    {
		public Type type;
		public object config;
    }

	public class ClassMappingDef : Def
	{
		public List<ListGroupHeaderMapping> mapping;


		[Unsaved(false)]
		private Dictionary<Type, WorkerTypeWithConfig> mapping_ = null;

		public Dictionary<Type, WorkerTypeWithConfig> Mapping
		{
			get
			{
				if (mapping_ == null)
				{
					$"Loading mappings...".Log();

					mapping_ = new Dictionary<Type, WorkerTypeWithConfig>();

					foreach (var def in mapping)
					{
						if (def.columnWorkerType == null ||
							def.groupWorkerType == null)
                        {
							continue;
                        }

						// reading types manually to suppress parser error if supported mod is not present
						var columnWorker = GenTypes.GetTypeInAnyAssembly(def.columnWorkerType);
						var groupWorker = GenTypes.GetTypeInAnyAssembly(def.groupWorkerType);
						if (columnWorker != null && groupWorker != null)
						{
							$"{def.columnWorkerType} => {def.groupWorkerType} - Ok".Log();
							mapping_[columnWorker] = new WorkerTypeWithConfig
							{
								type = groupWorker,
								config = def.workerConfig,
							};							
						} 
						else
                        {
							$"{def.columnWorkerType} => {def.groupWorkerType} - Failed".Log();
						}
					}

					$"Loaded {mapping_.Count} of {mapping.Count} table column mappings".Log();
				}
				return mapping_;
			}
		}
	}

	public static class GroupColumnDefResolver
	{
		public static GroupColumnWorkerDef GetResolver(PawnColumnDef column, bool generate = true)
		{
			var resolverDef = DefDatabase<GroupColumnWorkerDef>.GetNamedSilentFail(column.defName);
			if (generate && resolverDef == null)
			{
				var mapping = DefDatabase<ClassMappingDef>.GetNamed("GroupHeadersMapping");
				var workerClass = column.workerClass;
				WorkerTypeWithConfig headerType = null;

				while (workerClass != null)
				{
					if (mapping.Mapping.TryGetValue(workerClass, out headerType) && headerType != null)
					{
						resolverDef = CreateGroupColumnDef(column, headerType);
					}

					if (resolverDef != null)
					{
						break;
					}
					workerClass = workerClass.BaseType;
				}

				if (resolverDef == null)
				{
					resolverDef = CreateGroupColumnDef(column, new WorkerTypeWithConfig
					{
						type = typeof(GroupColumnWorker_Dummy),
						config = null
					});
				}

				$"Generated: column: {column.defName}; worker {column.workerClass.FullName} => {resolverDef.workerClass.FullName}".Log();
			}


			return resolverDef;
		}

		private static GroupColumnWorkerDef CreateGroupColumnDef(PawnColumnDef column, WorkerTypeWithConfig workerType)
		{
			GroupColumnWorkerDef resolverDef = new GroupColumnWorkerDef
			{
				defName = column.defName,
				workerClass = workerType.type,
				workerConfig = workerType.config,
				modContentPack = Mod.Instance.Content,
				generated = true,
			};
			DefGenerator.AddImpliedDef(resolverDef);
			return resolverDef;
		}
	}

}
