using Infrastructure.Context;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly MyAppContext _context;
        public EmailRepository(MyAppContext context)
        {
            _context = context;
        }
        public async Task<OperationResult> EditEmailStatus(ContactInfo contactobject)
        {
            try
            {
                var oldemail = await _context.ReceivedEmails.FirstOrDefaultAsync(e => e.Id == contactobject.Id);
                if (oldemail != null)
                {
                    oldemail.EmailStatus = contactobject.EmailStatus;
                    await _context.SaveChangesAsync();
                    return new OperationResult { Success = true, Message = "Updated successfully" };
                }
                return new OperationResult { Success = true, Message = "Email Not Found" };
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }

        public async Task<OperationResult> GetAllReceivedEmails(EmailPagination emailmodel)
        {
            try
            {
                var query = _context.ReceivedEmails.AsQueryable();
                if (!string.IsNullOrWhiteSpace(emailmodel.SearchParam))
                {
                    query = query.Where(e => e.Email.Contains(emailmodel.SearchParam) || e.Name.Contains(emailmodel.SearchParam));
                }
                if (emailmodel.EmailStatus > 0)
                {
                    query = query.Where(e => e.EmailStatus == (ReceviedEmailEnum)emailmodel.EmailStatus);
                }
                var totalRecords = await query.CountAsync();
                var emails = await query.Skip((emailmodel.PageNumber - 1) * emailmodel.PageSize).Take(emailmodel.PageSize).ToListAsync();
                if (emails.Any())
                {
                    return new OperationResult
                    {
                        Success = true,Message = "Data retrieved successfully.",Data = new {Emails = emails,TotalRecords = totalRecords,PageNumber = emailmodel.PageNumber,PageSize = emailmodel.PageSize,TotalPages = (int)Math.Ceiling((double)totalRecords / emailmodel.PageSize)}
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Success = true,Message = "No data found!",Data = new{Emails = new List<ReceivedEmail>(),TotalRecords = 0,PageNumber = emailmodel.PageNumber,PageSize = emailmodel.PageSize,TotalPages = 0}
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,Message = "Something went wrong. Please try again later.",DevelopMessage = ex.Message
                };
            }
        }

    }
}
