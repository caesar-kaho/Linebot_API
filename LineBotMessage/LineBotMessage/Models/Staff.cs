using System;
using System.Collections.Generic;

namespace LineBotMessage.Models;

/// <summary>
/// 職員表
/// </summary>
public partial class Staff
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<ExtentionNumber> ExtentionNumbers { get; } = new List<ExtentionNumber>();
}
