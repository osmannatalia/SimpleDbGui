using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Bestellung
{
    public int Bestellnr { get; set; }

    public int? Kundennr { get; set; }

    public DateTime? Bestelldatum { get; set; }

    public DateTime? Lieferdatum { get; set; }

    public decimal? Rechnungsbetrag { get; set; }

    public virtual Kunde? KundennrNavigation { get; set; }
}
