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
    public DateTime? EntryIntoForce { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? EndOfValidity { get; set; }
    public DateTime LastModifiedAt { get; set; }

    public string? ResourceType { get; set; }

    // Stored as JSON or separate tables later
    public List<string> EuroVocUris { get; set; } = new();
    public List<string> Amends { get; set; } = new();
    public List<string> Repeals { get; set; } = new();
    public List<string> ProposesToAmend { get; set; } = new();


    // Derived later
    public List<string> Topics { get; set; } = new();          // e.g. FOOD_SAFETY, HYGIENE
    public int? RelevanceScore { get; set; }
    public string? RelevanceCategory { get; set; }             // URGENT / IMPORTANT / MONITOR / ...
    public string? RelevanceRationale { get; set; }
    public Guid? AssignedTeamId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}