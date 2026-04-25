using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Department : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = default!;
    public DepartmentType Type { get; set; }
    public string? Description { get; set; }
    public Guid? ManagerUserId { get; set; }
    public Guid? ParentDepartmentId { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();
}
