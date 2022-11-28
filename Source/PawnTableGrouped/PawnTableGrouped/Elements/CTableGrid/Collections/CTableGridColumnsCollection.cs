using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    class CTableGridColumnsCollection : CTableGridDataSourceMetricsCache<object> // me being lazy
    {
        public CTableGridColumnsCollection(ICTableGridDataSource dataSource) : base(dataSource) { }

        protected override object itemAt(int index) => new object();

        protected override int itemsCount() => DataSource.numberOfColumns();

        protected override float getItemSize(int index) => DataSource.widthForColumn(index);
    }
}
