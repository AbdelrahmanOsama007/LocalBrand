using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enums
{
    public enum RegistrationStatus
    {
        Success,
        UserAlreadyExists,
        PasswordValidationFailed,
        OtherError
    }
}
