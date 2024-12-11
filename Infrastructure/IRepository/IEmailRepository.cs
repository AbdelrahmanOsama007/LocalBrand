using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IEmailRepository
    {
        Task<OperationResult> GetAllReceivedEmails(EmailPagination emailmodel);
        Task<OperationResult> EditEmailStatus(ContactInfo contactobject);
    }
}
