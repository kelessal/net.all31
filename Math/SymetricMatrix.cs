using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Math
{
    public class SymetricMatrix:Matrix
    {
        public int Rank => this.RowCount;

        public SymetricMatrix(int rank):base(rank,rank)
        {
        }
        public override void Set(int row, int col, bool value)
        {
            base.Set(row, col, value);
            base.Set(col, row, value);
        }

        public IEnumerable<int[]> FindBestUnitMatrixes()
        {
            HashSet<int> usedRanks = new HashSet<int>();
            for(var r=0;r<this.Rank;r++)
            {
                var possibles =Enumerable.Range(r + 1, this.Rank-r-1)
                     .Where(p => this.Get(r, p) == false).ToList();
                int foundRank = 0;
                for(var n = possibles.Count;n>0;n--)
                {
                    var combines = new Combinations<int>(possibles, n);
                    bool isFound = false;
                    foreach(var combine in combines)
                    {
                        var items = new[] { r }.Concat(combine).ToList();
                        var isUnitMatrix = this.IsCandidateToBeUnitMatrix(items);
                        if(isUnitMatrix)
                        {
                            isFound = true;
                            var founds= items.Select(p=>p%this.Rank).ToArray();
                            foreach (var found in founds)
                                usedRanks.Add(found);
                            yield return founds;
                        }
                    }
                    if (isFound)
                    {
                        foundRank = n;
                        break;
                    }
                }
                if (r!=this.Rank-1 && foundRank == this.Rank - r - 1)
                {
                    for (var i = r; i < this.Rank; i++)
                        usedRanks.Add(i);
                    break;
                }
            }
            foreach(var p in Enumerable.Range(0, this.Rank).Where(p => !usedRanks.Contains(p)))
            {
                yield return new[] { p };
            }
        }

        private bool IsCandidateToBeUnitMatrix(IList<int> combine)
        {
            for(var i = 0; i < combine.Count; i++)
            {
                var row = combine[i];
                if (this.Get(row, row) == false) return false;
                for (var j = i + 1; j < combine.Count; j++)
                {
                    var col = combine[j];
                    if (this.Get(row, col) == true) return false;
                    
                }
            }
            return true;
        }
       
    }
}
