using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped.TableGrid
{
    public interface ICTableGridDataSource
    {
        int NumberOfColumns();
        float WidthForColumn(int column);
        ICTableGridColumn ColumnAt(int column);

        int NumberOfSections();
        ICTableGridSection SectionAt(int section);
    }
}
