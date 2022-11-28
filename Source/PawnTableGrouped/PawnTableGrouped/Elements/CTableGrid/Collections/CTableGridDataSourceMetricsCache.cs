using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid.Collections
{
    abstract class CTableGridDataSourceMetricsCache<T> : CTableGridDataSourceCache<T> where T : class
    {
        public CTableGridDataSourceMetricsCache(ICTableGridDataSource dataSource) : base(dataSource)
        {
            integralSizes[0] = -1;
        }

        protected abstract float getItemSize(int index);

        float[] integralSizes = new float[33];

        protected override void ResizeArrays()
        {
            base.ResizeArrays();
            Array.Resize(ref integralSizes, Count + 1);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            integralSizes[0] = -1;
        }

        private void BuildIntegralSizesIfNeeded()
        {
            if (integralSizes[0] == 0)
            {
                return;
            }

            integralSizes[0] = 0;
            float height = 0;
            for (int i = 0; i < Count; i++)
            {
                height += getItemSize(i);
                integralSizes[i + 1] = height;
            }
        }

        public float IntegralSizeOf(int index)
        {
            BuildIntegralSizesIfNeeded();
            return integralSizes[index];
        }

        public float SizeOf(int index)
        {
            BuildIntegralSizesIfNeeded();
            return integralSizes[index + 1] - integralSizes[index];
        }

        public float TotalSize()
        {
            BuildIntegralSizesIfNeeded();
            return integralSizes[Count];
        }
    }

}
