using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    class CTableGridRowsCollection : CTableGridDataSourceMetricsCache<ICTableGridRow>
    {
        private readonly ICTableGridSection section;

        public CTableGridRowsCollection(ICTableGridDataSource dataSource, ICTableGridSection iCTableGridSection) : base(dataSource)
        {
            this.section = iCTableGridSection;
        }

        protected override ICTableGridRow itemAt(int index) => section.RowAt(index);

        protected override int itemsCount() => section.NumberOfRows();

        protected override float getItemSize(int index) => this[index].GetRowHeight();
        public ICTableGridSection Section => section;
    }
}
