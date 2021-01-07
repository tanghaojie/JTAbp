using Abp.Application.Services.Dto;

namespace JT.RoleService
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

