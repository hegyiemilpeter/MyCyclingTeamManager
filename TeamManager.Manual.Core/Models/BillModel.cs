using System.Collections.Generic;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Models
{
    public class BillModel
    {
        public IList<Bill> Bills { get; set; }
        public int Points { get; set; }
    }
}
