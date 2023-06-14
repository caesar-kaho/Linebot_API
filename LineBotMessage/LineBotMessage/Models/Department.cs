using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 部門表
/// </summary>
public partial class Department
{
    public uint Id { get; set; }

    /// <summary>
    /// 部門名稱
    /// </summary>
    public string DepartmentsName { get; set; } = null!;

    public virtual ICollection<Staff> Staff { get; } = new List<Staff>();

    public virtual ICollection<StaffsMultiExtentionnumber> StaffsMultiExtentionnumbers { get; } = new List<StaffsMultiExtentionnumber>();
}
