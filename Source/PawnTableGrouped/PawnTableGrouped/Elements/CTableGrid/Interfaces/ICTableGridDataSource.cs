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
        int numberOfColumns();
        float widthForColumn(int column);
        bool canMergeRows(int column);

        int numberOfSections();
        int numberOfRowsInSection(int section);
        ICTableGridRow rowAt(int section, int row);
    }
}
