using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Posten
{
    public int Bestellnr { get; set; }

    public int? Artikelnr { get; set; }

    public int? Bestellmenge { get; set; }

    public int? Liefermenge { get; set; }

    public virtual Artikel? ArtikelnrNavigation { get; set; }

    public virtual Bestellung BestellnrNavigation { get; set; } = null!;
}
