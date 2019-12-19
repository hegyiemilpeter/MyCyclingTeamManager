using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class BillModel
    {
        public IList<Bill> Bills { get; set; }
        public int Points { get; set; }
    }
}
