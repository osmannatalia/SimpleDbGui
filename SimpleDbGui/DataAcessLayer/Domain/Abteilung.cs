using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Abteilung
{
    public int Abteilungsnr { get; set; }

    public string? Bezeichnung { get; set; }

    public virtual ICollection<Mitarbeiter> Mitarbeiters { get; } = new List<Mitarbeiter>();
}
