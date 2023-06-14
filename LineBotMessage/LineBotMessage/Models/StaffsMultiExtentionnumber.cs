using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 職員有多個分機號碼
/// </summary>
public partial class StaffsMultiExtentionnumber
{
    public uint Id { get; set; }

    /// <summary>
    /// 職員名稱
    /// </summary>
    public string StaffsName { get; set; } = null!;

    /// <summary>
    /// 所屬單位
    /// </summary>
    public uint StaffsDepartment { get; set; }

    /// <summary>
    /// 分機號碼1
    /// </summary>
    public string? StaffsExtentionnumber1 { get; set; }

    /// <summary>
    /// 分機號碼2
    /// </summary>
    public string? StaffsExtentionnumber2 { get; set; }

    /// <summary>
    /// 分機號碼3
    /// </summary>
    public string? StaffsExtentionnumber3 { get; set; }

    public virtual Department StaffsDepartmentNavigation { get; set; } = null!;
}
