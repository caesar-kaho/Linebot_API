using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 部門表
/// </summary>
public partial class Department
{
    public int Id { get; set; }

    /// <summary>
    /// 部門名稱
    /// </summary>
    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<Staff> Staff { get; } = new List<Staff>();
}
