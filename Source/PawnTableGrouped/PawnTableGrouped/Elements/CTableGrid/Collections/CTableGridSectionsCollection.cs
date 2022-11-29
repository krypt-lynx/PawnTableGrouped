using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    class CTableGridSectionsCollection : CTableGridDataSourceMetricsCache<CTableGridRowsCollection>
    {
        public CTableGridSectionsCollection(ICTableGridDataSource dataSource) : base(dataSource) { }

        protected override CTableGridRowsCollection itemAt(int index) => new CTableGridRowsCollection(DataSource, DataSource.SectionAt(index));

        protected override int itemsCount() => DataSource.NumberOfSections();

        protected override float getItemSize(int index) => this[index].TotalSize();
    }
}
