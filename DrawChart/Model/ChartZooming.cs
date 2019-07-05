using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawChart.Model
{
    /// <summary>
    /// 控制图表缩放
    /// </summary>
    public class ChartZooming
    {
        private int _start { get; set; }
        private int _end { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        /// <summary>
        /// 图表显示起始值
        /// </summary>
        public int Start
        {
            get { return _start; }
            set
            {
                if (value >= _end) return;
                if (value < Min) return;
                _start = value;
            }
        }

        /// <summary>
        /// 图表显示终止值
        /// </summary>
        public int End
        {
            get { return _end; }
            set
            {
                if (value <= _start) return;
                if (value > Max) return;
                _end = value;
            }
        }

        //显示的间隔
        public int Duration
        {
            get { return _end - _start; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public ChartZooming(int min=0,int max = 1)
        {
            Min = min;
            Max = max;
            _start = Min;
            _end = Max;
        }
    }
}
