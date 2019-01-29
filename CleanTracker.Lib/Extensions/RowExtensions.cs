using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTracker.Lib.Models;

namespace CleanTracker.Lib.Extensions
{
    public static class RowExtensions
    {
        /// <summary>
        /// Check whether the media ID is that of an image
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsImageStimulus(this Row row)
        {
            var targetMediaIds = new int[] { 445, 443 };
            return targetMediaIds.Contains(row.MEDIA_ID);
        }
        /// <summary>
        /// Check whether the CS (event ID) is a left or right click
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsAClick(this Row row)
        {
            return row.CS == 1 || row.CS == 2;
        }

        /*
        public static int getCorrectMediaId(this Row row)
        {

        }
        */

        /// <summary>
        /// Check whether the media ID has a mismatch with the NewMedia name
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsFubar(this Row row)
        {
            var newMediaStr = "NewMedia";
            if (row.MEDIA_NAME.Contains(newMediaStr))
            {
                var sanitized = row.MEDIA_NAME.Replace(newMediaStr, "").Trim();
                int num = -1;
                int.TryParse(sanitized, out num);
                if(num != -1)
                {
                    return num != row.MEDIA_ID;
                }
                else
                {
                    return false;
                }
                // return targetMediaIds.Contains(row.MEDIA_ID);
            }
            else
            {
                return false;
            }
            
            
        }
    }
}
