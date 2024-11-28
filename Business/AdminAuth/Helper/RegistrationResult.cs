using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAuth.Helper
{
    public class RegistrationResult
    {
        public RegistrationStatus Status { get; set; }
        public string Message { get; set; }
    }
}
