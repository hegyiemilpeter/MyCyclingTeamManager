using System;
using System.Collections.Generic;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.ViewModels
{
    public class ConsumedPointsViewModel
    {
        public int UserId { get; set; }
        public IList<PointConsuption> ConsumedPoints { get; set; }
    }
}
