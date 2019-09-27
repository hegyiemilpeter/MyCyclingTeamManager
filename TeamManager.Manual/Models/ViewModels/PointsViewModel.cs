using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.ViewModels
{
    public class PointsViewModel
    {
        public IList<ResultModel> CollectedPoints { get; set; }
        public IList<PointConsuption> ConsumedPoints { get; set; }
        public int UserId { get; set; }
    }
}
