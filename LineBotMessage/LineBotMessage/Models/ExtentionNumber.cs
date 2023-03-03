using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 分機表
/// 
/// </summary>
public partial class ExtentionNumber
{
    public int Id { get; set; }

    public int StaffsId { get; set; }

    public string? ExtentionNumbers { get; set; }

    public virtual Staff Staffs { get; set; } = null!;
}
