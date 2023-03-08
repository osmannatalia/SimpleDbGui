using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using SimpleDbGui.Data;
using SimpleDbGui.DataAcessLayer;
using System.Data;

namespace SimpleDbGui.Data
{
    public class Customer
    {
        private static readonly Dictionary<RecordState, SaveStrategyFor> _saveStrategies = new Dictionary<RecordState, SaveStrategyFor>()
        {
            { RecordState.Initial,  SaveError },
            { RecordState.New,      SaveNewInstance },
            { RecordState.Hollow,   SaveNothing },
            { RecordState.Clean,    SaveNothing },
            { RecordState.Dirty,    SaveDirtyInstance },
            { RecordState.Deleted,  SaveDeletedInstance },
        };

        private RecordState RecordState { get; set; } = RecordState.Initial;

        public static readonly Customer NONE = new Customer()
        {
            Id = -1
        };

        public int Id { get; private set; } = -1;
        private string _name = "?";
        public string Name
        {
            get
            {
                LoadIfNecessary();
                return _name;
            }
            set
            {
                MarkDirtyIfNotNew();
                _name = value;
            }
        }
        private string _firstname = "?";
        public string Firstname
        {
            get
            {
                LoadIfNecessary();
                return _firstname;
            }
            set
            {
                MarkDirtyIfNotNew();
                _firstname = value;
            }
        }
        private string _street = "?";
        public string Street
        {
            get
            {
                LoadIfNecessary();
                return _street;
            }
            set
            {
                MarkDirtyIfNotNew();
                _street = value;
            }
        }
        private string _zipCode = "?";
        public string ZipCode
        {
            get
            {
                LoadIfNecessary();
                return _zipCode;
            }
            set
            {
                MarkDirtyIfNotNew();
                _zipCode = value;
            }
        }
        private string _city = "?";
        public string City
        {
            get
            {
                LoadIfNecessary();
                return _city;
            }
            set
            {
                MarkDirtyIfNotNew();
                _city = value;
            }
        }
        public List<Order> Orders { get; } = new List<Order>();

        public int OrderCount => Orders.Count;

        public bool IsNew => RecordState == RecordState.New;

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

        public Customer()
        {
            RecordState = RecordState.New;
        }

        public Customer(int id)
        {
            Id = id;
            RecordState = RecordState.Hollow;
        }

        public static IEnumerable<Customer> Load()
        {
            List<Customer> result = new List<Customer>();
            using (var connection = ConnectionProvider.ConnectionGet())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT KUNDENNR "
                                + "FROM kunde";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Customer(reader.GetInt32(0)));
                    }
                }
            }

            return result;
        }

        public void Revert()
        {
            using (var connection = ConnectionProvider.ConnectionGet())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT k.KUNDENNR, Name, Vorname, Strasse, PLZ, Ort, bestellnr "
                                + "FROM kunde k LEFT OUTER JOIN bestellung b ON k.KundenNr = b.KundenNr "
                                + "WHERE k.Kundennr = @id";
                cmd.Parameters.Add(new MySqlParameter("id", Id));

                using (var reader = cmd.ExecuteReader())
                {
                    bool isFirstRow = true;
                    while (reader.Read())
                    {
                        if (isFirstRow)
                        {
                            Id = reader.GetInt32(0);
                            Name = reader.GetString(1);
                            Firstname = reader.GetString(2);
                            Street = reader.GetString(3);
                            ZipCode = reader.GetString(4);
                            City = reader.GetString(5);

                            isFirstRow = false;
                        }

                        if (!reader.IsDBNull(6))
                        {
                            Orders.Add(new Order(reader.GetInt32(6)));
                        }
                    }
                }
            }

            RecordState = RecordState.Clean;
        }

        public void Delete()
        {
            RecordState = RecordState.Deleted;
        }

        private delegate bool SaveStrategyFor(Customer customer, IDbCommand cmd);


        public bool Save()
        {
            bool runQuery = false;

            using (var connection = ConnectionProvider.ConnectionGet())
            using (var cmd = connection.CreateCommand())
            {
                SaveStrategyFor _saveIt = GetSaveStrategyForState(RecordState);
                
                runQuery = _saveIt(this, cmd);

                cmd.Parameters.Add(new MySqlParameter("@id", Id));
                cmd.Parameters.Add(new MySqlParameter("@name", Name));
                cmd.Parameters.Add(new MySqlParameter("@vorname", Firstname));
                cmd.Parameters.Add(new MySqlParameter("@strasse", Street));
                cmd.Parameters.Add(new MySqlParameter("@plz", ZipCode));
                cmd.Parameters.Add(new MySqlParameter("@ort", City));

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

        private static SaveStrategyFor GetSaveStrategyForState(RecordState recordState)
        {
            SaveStrategyFor? _saveIt = SaveError;

            if (!_saveStrategies.TryGetValue(recordState, out _saveIt))
            {
                return SaveError;
            }

            return _saveIt;
        }

        private static bool SaveNothing(Customer customer, IDbCommand cmd)
        {
            return false;
        }

        private static bool SaveNewInstance(Customer customer, IDbCommand cmd)
        {
            cmd.CommandText = "INSERT INTO kunde (kundennr, name, vorname, strasse, plz, ort) "
                            + "  ("
                            + "    SELECT MAX(kundennr) + 1, @name, @vorname, @strasse, @plz, @ort "
                            + "    FROM kunde"
                            + "  )"
                            + ";";
            return true;
        }

        private static bool SaveDirtyInstance(Customer customer, IDbCommand cmd)
        {
            cmd.CommandText = "UPDATE kunde "
                + "SET "
                + "  Name = @name "
                + ", Vorname = @vorname "
                + ", Strasse = @strasse "
                + ", PLZ = @plz "
                + ", Ort = @ort "
                + "WHERE kundennr = @id";
            return true;
        }

        private static bool SaveDeletedInstance(Customer customer, IDbCommand cmd)
        {
            if (!customer.IsNew)
            {
                cmd.CommandText = "DELETE FROM kunde "
                                + "WHERE kundennr=@id";
                return true;
            }

            return false;
        }

        private static bool SaveError(Customer me, IDbCommand cmd)
        {
            throw new InvalidOperationException($"Object is not in an appropriate state for {nameof(Save)}: {me.RecordState}");
        }
    }
}