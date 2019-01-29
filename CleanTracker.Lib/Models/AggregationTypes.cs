using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTracker.Lib.Models
{
    /// <summary>
    /// Summary data row, which will ultimately be collapsed
    /// </summary>
    public class LpdRpd
    {
        public double LPD { get; set; }
        public double RPD { get; set; }
        public int CS { get; set; }
        public double time { get; set; }
        public int cnt { get; set; }
        public LpdRpd(double lpd, double rpd, int cs, double _time, int _cnt)
        {
            LPD = lpd;
            RPD = rpd;
            CS = cs;
            time = _time;
            cnt = _cnt;
        }
    }

    /// <summary>
    /// Just average LPD/RPD data
    /// </summary>
    public class AvgLpdRpd
    {
        public double avgLPD { get; set; }
        public double avgRPD { get; set; }
        public AvgLpdRpd(double lpd, double rpd)
        {
            avgLPD = lpd;
            avgRPD = rpd;
        }
    }

    /// <summary>
    /// Get only the media ID and name from a row of data
    /// </summary>
    public class MediaNameAndId
    {
        public string mediaName { get; set; }
        public int mediaId { get; set; }
        public MediaNameAndId(Row row)
        {
            mediaName = row.MEDIA_NAME;
            mediaId = row.MEDIA_ID;
        }
    }

        /// <summary>
        /// Flattened data output
        /// </summary>
        public class AveragesWithClickData
    {
        public double avgLPD { get; set; }
        public double avgRPD { get; set; }
        public int mediaId { get; set; }

        // in-sample data at targeted record
        public double LPD { get; set; }
        public double RPD { get; set; }
        public int CS { get; set; }
        public double time { get; set; }
        public int cnt { get; set; }
        public string participantId { get; set; }

        private void setClickData(LpdRpd click)
        {
            LPD = click.LPD;
            RPD = click.RPD;
            CS = click.CS;
            time = click.time;
            cnt = click.cnt;
        }

        public AveragesWithClickData(double _avgLpd, double _avgRpd, List<LpdRpd> _clicks, int _mediaId, string _participantId)
        {


            avgLPD = _avgLpd;
            avgRPD = _avgRpd;
            mediaId = _mediaId;
            participantId = _participantId;

            // if we have multiple responses, we need to check if they're multiple types
            if (_clicks.Count > 1)
            {
                
                var responseTypes = _clicks.GroupBy(click => { return click.CS; }).ToList();
                // handle multiple response types
                if (responseTypes.Count > 1)
                {
                    // get CS state of final answer
                    var lastVal = _clicks.OrderBy(x => { return x.cnt; }).Last().CS;
                    // get first value in last group
                    responseTypes.ForEach(group => {
                        if (group.Key == lastVal)
                        {
                            setClickData(group.OrderBy(x => { return x.cnt; }).First());
                        }
                    });
                }
                else
                {
                    setClickData(_clicks.OrderBy(x => { return x.cnt; }).First());
                }
            }
            else if (_clicks.Count == 1)
            {
                setClickData(_clicks[0]);
            }
        }
    }
}
