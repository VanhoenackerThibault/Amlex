namespace Amlex.Models.Models.LegalActs;

public class LegalAct
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CellarId { get; set; } = string.Empty;

    public string Celex { get; set; } = string.Empty;

    public string? TitleNl { get; set; }

    public string? TitleEn { get; set; }

    public string? TitleFr { get; set; }

    public bool? InForce { get; set; }

    public DateTime? DocumentDate { get; set; }

    public DateTime LastModifiedAt { get; set; }



    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}