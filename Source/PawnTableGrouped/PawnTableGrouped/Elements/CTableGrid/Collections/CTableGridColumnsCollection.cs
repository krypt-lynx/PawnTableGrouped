using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    class CTableGridColumnsCollection : CTableGridDataSourceMetricsCache<ICTableGridColumn>
    {
        public CTableGridColumnsCollection(ICTableGridDataSource dataSource) : base(dataSource) { }

        protected override ICTableGridColumn itemAt(int index) => DataSource.ColumnAt(index);

        protected override int itemsCount() => DataSource.NumberOfColumns();

        protected override float getItemSize(int index) => DataSource.WidthForColumn(index);
    }
}
