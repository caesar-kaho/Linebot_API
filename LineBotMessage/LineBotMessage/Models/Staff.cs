using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 職員表
/// </summary>
public partial class Staff
{
    public uint Id { get; set; }

    /// <summary>
    /// 職員名稱
    /// </summary>
    public string StaffsName { get; set; } = null!;

    /// <summary>
    /// 所屬部門
    /// </summary>
    public uint StaffsDepartment { get; set; }

    /// <summary>
    /// 分機號碼
    /// </summary>
    public string StaffsExtentionnumber { get; set; } = null!;

    public virtual Department StaffsDepartmentNavigation { get; set; } = null!;
}
