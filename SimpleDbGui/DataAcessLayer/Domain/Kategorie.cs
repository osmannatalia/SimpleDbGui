using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Kategorie
{
    public int Kategorienr { get; set; }

    public string? Bezeichnung { get; set; }

    public virtual ICollection<Artikel> Artikels { get; } = new List<Artikel>();
}
