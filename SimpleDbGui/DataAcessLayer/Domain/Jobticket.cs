using System;
using System.Collections.Generic;

namespace SimpleDbGui.DataAcessLayer.Domain;

public partial class Jobticket
{
    public int Id { get; set; }

    public int? Mitarbeiternr { get; set; }

    public DateTime? GueltigBis { get; set; }

    public virtual Mitarbeiter? MitarbeiternrNavigation { get; set; }
}
