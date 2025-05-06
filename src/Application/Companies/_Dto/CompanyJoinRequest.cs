using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Companies._Dto
{
    public class CompanyJoinRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedDate { get; set; }
    }

}
