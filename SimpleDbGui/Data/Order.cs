using MySql.Data.MySqlClient;
using SimpleDbGui.DataAcessLayer;

namespace SimpleDbGui.Data
{
    public class Order
    {
        private RecordState RecordState { get; set; } = RecordState.Initial;

        public static readonly Order NONE = new Order()
        {
            Id = -1
        };

        public bool IsNew => RecordState == RecordState.New;

        private static int _id = 0;
        public int Id { get; private set; } = ++_id;

        private DateTime _orderDate = DateTime.MinValue;
        public DateTime OrderDate
        {
            get
            {
                LoadIfNecessary();
                return _orderDate;
            }
            set
            {
                MarkDirtyIfNotNew();
                _orderDate = value;
            }
        }
        private DateTime? _deliveryDate = null;
        public DateTime? DeliveryDate
        {
            get
            {
                LoadIfNecessary();
                return _deliveryDate;
            }
            set
            {
                MarkDirtyIfNotNew();
                _deliveryDate = value;
            }
        }
        private double _balance = 0;
        public double Balance
        {
            get
            {
                LoadIfNecessary();
                return _balance;
            }

            set
            {
                MarkDirtyIfNotNew();
                _balance = value;
            }
        }

        public Order()
        {
            RecordState = RecordState.New;
        }

        public Order(int id)
        {
            RecordState = RecordState.Hollow;
            Id = id;
        }

        private void LoadIfNecessary()
        {
            if (RecordState == RecordState.Hollow)
            {
                Revert();
            }
        }

        private void MarkDirtyIfNotNew()
        {
            if (RecordState != RecordState.New)
            {
                RecordState = RecordState.Dirty;
            }
        }

        public void Revert()
        {
            using (var connection = ConnectionProvider.ConnectionGet())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT BestellNr, Bestelldatum, Lieferdatum, Rechnungsbetrag "
                                + "FROM bestellung "
                                + "WHERE BestellNr = @id";
                cmd.Parameters.Add(new MySqlParameter("id", Id));

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Id = reader.GetInt32(0);
                        OrderDate = reader.GetDateTime(1);
                        DeliveryDate = reader.GetDateTime(2);
                        Balance = reader.GetDouble(3);
                    }
                }
            }

            RecordState = RecordState.Clean;
        }

        public void Delete()
        {
            RecordState = RecordState.Deleted;
        }

        public bool Save()
        {
            bool runQuery = false;

            using (var connection = ConnectionProvider.ConnectionGet())
            using (var cmd = connection.CreateCommand())
            {
                switch (RecordState)
                {
                    case RecordState.Deleted:
                        if (!IsNew)
                        {
                            cmd.CommandText = "DELETE FROM bestellung "
                                            + "WHERE bestellnr=@id";
                            cmd.Parameters.Add(new MySqlParameter("@id", Id));
                            runQuery = true;
                        }
                        break;

                    case RecordState.Dirty:
                        cmd.CommandText = "UPDATE bestellung "
                                        + "SET "
                                        + "  Bestelldatum = @name"
                                        + ", Lieferdatum = @vorname"
                                        + ", Rechnungsbetrag = @strasse"
                                        + "WHERE BestellNr = @id";
                        runQuery = true;
                        break;

                    case RecordState.New:
                        cmd.CommandText = "INSERT INTO bestellung (bestellnr, Bestelldatum, Lieferdatum, Rechnungsbetrag) "
                                        + "  ("
                                        + "    SELECT MAX(bestellnr) + 1, @bestelldatum, @lieferdatum, @rechnungsbetrag "
                                        + "    FROM kunde"
                                        + "  )"
                                        + ";";
                        runQuery = true;
                        break;

                    case RecordState.Hollow:
                    case RecordState.Clean:
                        runQuery = false;
                        break;

                    case RecordState.Initial:
                    default:
                        throw new InvalidOperationException($"Object is not in an appropriate state for {nameof(Save)}: {RecordState}");
                }

                cmd.Parameters.Add(new MySqlParameter("@id", Id));
                cmd.Parameters.Add(new MySqlParameter("@bestelldatum", OrderDate));
                cmd.Parameters.Add(new MySqlParameter("@lieferdatum", DeliveryDate));
                cmd.Parameters.Add(new MySqlParameter("@rechnungsbetrag", Balance));

                if (runQuery)
                {
                    var querySuccessful = cmd.ExecuteNonQuery() > 0;
                    if (querySuccessful)
                    {
                        RecordState = RecordState.Clean;
                    }
                    return querySuccessful;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
