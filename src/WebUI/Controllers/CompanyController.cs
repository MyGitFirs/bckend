
using CleanArchitecture.Application.Companies.Commands.CreateCompany;
using CleanArchitecture.Application.Companies.Commands.DeleteCompany;
using CleanArchitecture.Application.Companies.Commands.JoinCompany;
using CleanArchitecture.Application.Company.Commands.UpdateCompany;
using CleanArchitecture.Application.Company.Queries.GetCompanies;
using CleanArchitecture.Application.Company.Queries.GetJoinRequests;
using CleanArchitecture.Application.Companies.Commands.ApproveJoinRequest;

using CleanArchitecture.Application.User.Queries.GetUsers;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CleanArchitecture.WebUI.Controllers
{
    public class CompanyController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCompanies([FromQuery] GetCompanyQuery request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            try
            {
                var query = new GetCompanyByIdQuery { Id = id };
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] CreateCompanyCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(new { companyId = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(string id, [FromBody] UpdateCompanyCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }
        [HttpPost("join-company")]
        public async Task<IActionResult> JoinCompany([FromBody] JoinUserCompanyCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(string id)
        {
            if (!Guid.TryParse(id, out Guid companyId))
            {
                return BadRequest("Invalid GUID format.");
            }

            var request = new DeleteCompanyCommand
            {
                Id = companyId
            };

            var result = await Mediator.Send(request);
            return Ok(result);
        }
        [HttpGet("pending-join-requests/{companyId}")]
        public async Task<IActionResult> GetPendingJoinRequests([FromRoute] Guid companyId)
        {
            var result = await Mediator.Send(new GetPendingJoinRequestsQuery { CompanyId = companyId });
            return Ok(result);
        }
        [HttpPost("approve-join-request")]
        public async Task<IActionResult> ApproveJoinRequest([FromBody] ApproveCompanyJoinRequestCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("by-code/{companyCode}")]
        public async Task<IActionResult> GetCompanyByCode(string companyCode)
        {
            try
            {
                var query = new GetCompanyByCodeQuery { CompanyCode = companyCode };
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
