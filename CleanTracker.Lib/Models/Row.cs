using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTracker.Lib.Contracts;

namespace CleanTracker.Lib.Models
{
    public class Row : IRow
    {
        public int MEDIA_ID { get; set; }
        public string MEDIA_NAME { get; set; }
        public int CNT { get; set; }

        public double?  time { get; set; }
        public long? timetick { get; set; }
        // TIME(2018/04/25 13:42:05.688)
        // TIMETICK(f=3328124)

        public double FPOGX { get; set; }
        public double FPOGY { get; set; }
        public double FPOGS { get; set; }
        public double FPOGD { get; set; }

        public int FPOGID { get; set; }
        public int FPOGV { get; set; }

        public double BPOGX { get; set; }
        public double BPOGY { get; set; }
        public int BPOGV { get; set; }
        public double CX { get; set; }
        public double CY { get; set; }
        public int CS { get; set; }
        // USER

        public double LPCX { get; set; }
        public double LPCY { get; set; }
        public double LPD { get; set; }
        public double LPS { get; set; }
        public int LPV { get; set; }

        public double RPCX { get; set; }
        public double RPCY { get; set; }
        public double RPD { get; set; }
        public double RPS { get; set; }
        public int RPV { get; set; }

        public int BKID { get; set; }
        public double BKDUR { get; set; }
        public int BKPMIN { get; set; }
        public string AOI { get; set; }
    }
}
