using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Hersteller
{
    public int Herstellernr { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Artikel> Artikels { get; } = new List<Artikel>();
}
