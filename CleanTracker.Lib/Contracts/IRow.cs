using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTracker.Lib.Contracts
{
    public interface IRow
    {
        int MEDIA_ID { get; set; }
        string MEDIA_NAME { get; set; }
        int CNT{ get; set; }
        // TIME(2018/04/25 13:42:05.688)
        // TIMETICK(f=3328124)

        double FPOGX { get; set; }
        double FPOGY { get; set; }
        double FPOGS { get; set; }
        double FPOGD { get; set; }

        int FPOGID { get; set; }
        int FPOGV { get; set; }

        double BPOGX { get; set; }
        double BPOGY { get; set; }
        int BPOGV { get; set; }
        double CX{ get; set; }
        double CY{ get; set; }
        int CS { get; set; }
        // USER

        double LPCX { get; set; }
        double LPCY{ get; set; }
        double LPD { get; set; }
        double LPS{ get; set; }
        int LPV { get; set; }

        double RPCX { get; set; }
        double RPCY { get; set; }
        double RPD { get; set; }
        double RPS { get; set; }
        int RPV { get; set; }

        int BKID { get; set; }
        double BKDUR{ get; set; }
        int BKPMIN { get; set; }
        string AOI { get; set; }
    }
}
