using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Kunde
{
    public int Kundennr { get; set; }

    public string? Name { get; set; }

    public string? Vorname { get; set; }

    public string? Strasse { get; set; }

    public string? Plz { get; set; }

    public string? Ort { get; set; }

    public string? TelefonGesch { get; set; }

    public string? TelefonPrivat { get; set; }

    public string? Email { get; set; }

    public string? Zahlungsart { get; set; }

    public virtual ICollection<Bestellung> Bestellungs { get; } = new List<Bestellung>();
}
