﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Email.Interfaces
{
    public interface ISendEmail
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
