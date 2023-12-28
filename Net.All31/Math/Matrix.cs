using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Math
{ 
    public class Matrix
    {
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        protected readonly bool[,] _matrixArray;
        public Matrix(int row,int col)
        {
            this.RowCount = row;
            this.ColumnCount = col;
            this._matrixArray = new bool[row, col];
        }
        public bool Get(int row,int col)
        {
            row = row % this.RowCount;
            col = col % this.ColumnCount;
            return this._matrixArray[row, col];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(var r = 0; r < this.RowCount; r++)
            {
                sb.Append("|");
                for(var c = 0; c < this.ColumnCount; c++)
                {
                    sb.Append(this._matrixArray[r, c] ? "1 " : "0 ");
                }
                sb.Append("|\n");

            }
            return sb.ToString();


        }
        public virtual void Set(int row,int col,bool value)
        {
            this._matrixArray[row, col] = value;
        }
        public virtual Matrix Transpose()
        {
            var matrix = new Matrix(this.ColumnCount, this.RowCount);
            for (var r = 0; r < this.RowCount; r++)
                for (var c = 0; c < this.ColumnCount; c++)
                    matrix._matrixArray[c, r] = this._matrixArray[r, c];
            return matrix;
        }

        public Matrix Multiply(Matrix m)
        {
            var result = new Matrix(this.RowCount, m.ColumnCount);
            for(var r = 0;r<this.RowCount;r++)
            {
                for(var mcol=0;mcol<m.ColumnCount;mcol++)
                {
                    for (var c = 0; c < this.ColumnCount; c++)
                    {
                        if (this._matrixArray[r, c] && m._matrixArray[c, mcol])
                        {
                            result._matrixArray[r, mcol] = true;
                            break;
                        }
                    }
                }
                
            }
            return result;
        }
        public SymetricMatrix MultiplyByTranspose()
        {
            var result = new SymetricMatrix(this.RowCount);
            for (var row = 0; row < this.RowCount; row++)
                for(var col=0;col<this.RowCount;col++)
                    for (var i = 0; i < this.ColumnCount; i++)
                    {
                        if (this._matrixArray[row, i] && this._matrixArray[col,i])
                        {
                            result.Set(row, col, true);
                            break;
                        }
                    }
            return result;
        }
    }
}
