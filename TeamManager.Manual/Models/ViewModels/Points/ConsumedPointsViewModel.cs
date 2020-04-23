using System;
using System.Collections.Generic;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.ViewModels
{
    public class ConsumedPointsViewModel
    {
        public int UserId { get; set; }
        public IList<PointConsuption> ConsumedPoints { get; set; }
    }
}
