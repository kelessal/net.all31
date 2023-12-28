using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Math
{
    public class UnitMatrix : SymetricMatrix
    {
        public UnitMatrix(int rank) : base(rank)
        {
            for (var i = 0; i < this.Rank; i++)
                base.Set(i, i, true);
        }
        public override void Set(int row, int col, bool value)
        {
            throw new InvalidOperationException();
        }
    }
}
