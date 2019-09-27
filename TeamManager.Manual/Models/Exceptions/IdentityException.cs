using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models.Exceptions
{
    public class IdentityException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
