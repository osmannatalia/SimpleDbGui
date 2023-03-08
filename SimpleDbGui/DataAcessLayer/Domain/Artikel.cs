using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Artikel
{
    public int Artikelnr { get; set; }

    public string? Bezeichnung { get; set; }

    public int? Hersteller { get; set; }

    public decimal? Nettopreis { get; set; }

    public int? Mwst { get; set; }

    public int? Bestand { get; set; }

    public int? Mindestbestand { get; set; }

    public int? Kategorie { get; set; }

    public string? Bestellvorschlag { get; set; }

    public virtual Hersteller? HerstellerNavigation { get; set; }

    public virtual Kategorie? KategorieNavigation { get; set; }

    public virtual Mwstsatz? MwstNavigation { get; set; }
}
