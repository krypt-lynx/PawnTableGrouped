using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    class CTableGridRowsCollection : CTableGridDataSourceMetricsCache<ICTableGridRow>
    {
        private readonly int sectionIndex;

        public CTableGridRowsCollection(ICTableGridDataSource dataSource, int sectionIndex) : base(dataSource)
        {
            this.sectionIndex = sectionIndex;
        }

        protected override ICTableGridRow itemAt(int index) => DataSource.rowAt(sectionIndex, index);

        protected override int itemsCount() => DataSource.numberOfRowsInSection(sectionIndex);

        protected override float getItemSize(int index) => this[index].GetRowHeight();
    }
}
