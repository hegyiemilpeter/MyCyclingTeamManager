using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.ViewModels
{
    public class PointsViewModel
    {
        public IList<ResultModel> Results { get; set; }
        public IList<PointConsuption> ConsumedPoints { get; set; }
        public BillModel Bills { get; set; }
        public int UserId { get; set; }
    }
}
