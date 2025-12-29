using System;
using System.Collections.Generic;

namespace Demo.Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
