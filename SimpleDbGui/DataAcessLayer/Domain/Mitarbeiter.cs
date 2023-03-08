using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Mitarbeiter
{
    public int Mitarbeiternr { get; set; }

    public string? Name { get; set; }

    public string? Vorname { get; set; }

    public string? Strasse { get; set; }

    public string? Plz { get; set; }

    public string? Ort { get; set; }

    public decimal? Gehalt { get; set; }

    public int? Abteilung { get; set; }

    public string? Telefonnummer { get; set; }

    public string? Email { get; set; }

    public DateTime? Eintrittsdatum { get; set; }

    public virtual Abteilung? AbteilungNavigation { get; set; }
}
