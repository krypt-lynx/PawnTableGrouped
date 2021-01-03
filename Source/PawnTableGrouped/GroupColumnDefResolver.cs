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
		public bool isInteractive;
	}

	public class ClassMappingDef : Def
	{
		public List<ListGroupHeaderMapping> mapping;


		[Unsaved(false)]
		private Dictionary<Type, Type> mapping_ = null;

		public Dictionary<Type, Type> Mapping
		{
			get
			{
				if (mapping_ == null)
				{
					mapping_ = new Dictionary<Type, Type>();

					foreach (var def in mapping)
					{
						// reading types manually to suppress parser error if supported mod is not present
						var columnWorker = GenTypes.GetTypeInAnyAssembly(def.columnWorkerType);
						var groupWorker = GenTypes.GetTypeInAnyAssembly(def.groupWorkerType);
						if (columnWorker != null && groupWorker != null)
						{
							mapping_[columnWorker] = groupWorker;
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
		public static GroupColumnWorkerDef GetResolverSilentFail(PawnColumnDef column)
		{
			var resolverDef = DefDatabase<GroupColumnWorkerDef>.GetNamedSilentFail(column.defName);
			if (resolverDef == null)
			{
				var mapping = DefDatabase<ClassMappingDef>.GetNamed("GroupHeadersMapping");
				var workerClass = column.workerClass;
				Type headerType = null;

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
					resolverDef = CreateGroupColumnDef(column, typeof(GroupColumnWorker_Dummy));
				}

				$"Header for column {column.defName} with worker {column.workerClass.FullName}: {resolverDef.workerClass.FullName}".Log();
			}


			return resolverDef;
		}

		private static GroupColumnWorkerDef CreateGroupColumnDef(PawnColumnDef column, Type workerType)
		{
			GroupColumnWorkerDef resolverDef = new GroupColumnWorkerDef
			{
				defName = column.defName,
				workerClass = workerType,
				modContentPack = Mod.Instance.Content
			};
			DefGenerator.AddImpliedDef(resolverDef);
			return resolverDef;
		}
	}

}
