using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NevronskaMreža
{
    using Point = Tuple<double, double>;
    class LearningInput
    {
        public double[] coordinates;
        public int represents;
        public LearningInput(double[] coordinates, int represents)
        {
            this.coordinates = coordinates;
            this.represents = represents;
        }
    }
}
