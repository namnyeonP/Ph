using Righthand.RealtimeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_BASE.클래스
{
    public class GraphItem : IGraphItem
    {
        public int Time { get; set; }
        public double Value { get; set; }
    }
}
