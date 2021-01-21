using Cassowary;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    public class ColumnGuide : CLayoutGuide
    {
        public ColumnGuide(int columnsCount)
        {
            columns_ = new Anchor[columnsCount];
            for (int i = 0; i < columnsCount; i++)
            {
                columns_[i] = new Anchor();
            }
        }

        private Anchor[] columns_;

        private ClVariable[] columnsCached = null;
        public ClVariable[] columns => columnsCached ??= columns_.Select((c, i) => Parent.GetVariable(ref columns_[i], $"column{i}")).ToArray();

        public void UpdateColumnWidth(int index, double width)
        {
            Parent.UpdateStayConstrait(ref columns_[index], width);
        }

        public override void AddImpliedConstraints()
        {
            for (int i = 0; i < columns_.Length; i++)
            {
                Parent.CreateConstraintIfNeeded(ref columns_[i], () => new ClStayConstraint(columns[i]));
            }
        }

        public override void RemoveImpliedConstraints()
        {
            for (int i = 0; i < columns_.Length; i++)
            {
                Parent.RemoveVariableIfNeeded(ref columns_[i]);
            }
        }

        public override IEnumerable<ClVariable> enumerateAnchors()
        {
            return columns;
        }
    }

}
