using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Mwstsatz
{
    public int Mwstnr { get; set; }

    public decimal? Prozent { get; set; }

    public virtual ICollection<Artikel> Artikels { get; } = new List<Artikel>();
}
