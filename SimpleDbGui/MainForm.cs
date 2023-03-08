using MySql.Data.MySqlClient;
using SimpleDbGui.Data;
using SimpleDbGui.DataAcessLayer;
using System.Configuration;
using System.Data;

namespace SimpleDbGui
{
    public partial class MainForm : Form
    {
        private object _lock = new object();

        private OrderView _orderDialog = new OrderView();

        public bool IsConnected => !string.IsNullOrWhiteSpace(ConnectionProvider.ConnectionString);

        protected Customer? CurrentCustomer => (gridCustomers.CurrentRow != null)  && IsConnected
                                               ? gridCustomers.CurrentRow.DataBoundItem as Customer 
                                               : null;

        #region Prepared framework stuff
        public MainForm()
        {
            InitializeComponent();

            txtHostname.Text = LoadFromSettingsOrDefault("Host", "localhost");
            txtUsername.Text = LoadFromSettingsOrDefault("User", "mysqladmin");
            txtPassword.Text = LoadFromSettingsOrDefault("Pass", "Riethuesli>12345");

            UpdateConnectionStatus();
        }

        private string LoadFromSettingsOrDefault(string key, string defaultValue)
        {
            var result = defaultValue;

            try
            {
                var valueRead = ConfigurationManager.AppSettings[key];
                result = string.IsNullOrWhiteSpace(valueRead) ? defaultValue : valueRead;
            }
            catch
            {
            }

            return result;
        }

        private string BuildConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = txtHostname.Text,
                UserID = txtUsername.Text,
                Password = txtPassword.Text,
                Database = "SqlTeacherDb"
            };

            var additions = LoadFromSettingsOrDefault("AdditionalConnectionString", "");
            if (!string.IsNullOrEmpty(additions))
            {
                return builder.ConnectionString + $";{additions}";
            }

            return builder.ConnectionString;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectionProvider.ConnectionString = BuildConnectionString();

            DisplayCustomers();

            UpdateConnectionStatus();
        }

        private void DisplayCustomers()
        {
            if (!IsConnected)
            {
                gridCustomers.DataSource = null;
                return;
            }

            gridCustomers.DataSource = GetCustomers();
            gridCustomers.Columns.Clear();
            gridCustomers.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Name",
                HeaderText = "Name",
            });
            gridCustomers.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Firstname",
                HeaderText = "Firstname",
            });
            gridCustomers.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "OrderCount",
                HeaderText = "Order Count",
            });
            FillUiFromCustomer(CurrentCustomer);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                ConnectionProvider.ConnectionString = string.Empty;

                UpdateConnectionStatus();
                DisplayCustomers();
            }
        }

        private void UpdateConnectionStatus()
        {
            btnConnect.Enabled = !IsConnected;
            btnDisconnect.Enabled = IsConnected;
            grpCustomer.Enabled = IsConnected;

            if (!IsConnected)
            {
                EmptyTextboxes(grpCustomer.Controls);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AddOrUpdateCustomer();
            DisplayCustomers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (CurrentCustomer != null && IsConnected)
            {
                var customerToDelete = CurrentCustomer;
                // Avoid updating issues with the selection
                SelectPreviousRecordIfLastSelected();
                RemoveCustomer(customerToDelete);
                DisplayCustomers();
            }
        }

        private void SelectPreviousRecordIfLastSelected()
        {
            var rowIndex = gridCustomers.CurrentRow.Index;
            if (rowIndex == gridCustomers.Rows.Count - 1)
            {
                gridCustomers.ClearSelection();
                if (rowIndex > 0)
                {
                    gridCustomers.CurrentCell = gridCustomers.Rows[rowIndex - 1].Cells[0];
                }
            }
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            FillUiFromCustomer(CurrentCustomer);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                gridCustomers.ClearSelection();
                gridCustomers.CurrentCell = null;
                EmptyTextboxes(grpCustomer.Controls);
            }
        }

        private void btnShowOrders_Click(object sender, EventArgs e)
        {
            if (CurrentCustomer != null)
            {
                _orderDialog.CustomerSelected = CurrentCustomer;
                _orderDialog.ShowDialog();
            }
        }

        private void gridCustomers_SelectionChanged(object sender, EventArgs e)
        {
            FillUiFromCustomer(CurrentCustomer);
        }
        #endregion

        private void FillCustomerFromUi(Customer customer)
        {
            customer.Name = txtName.Text;
            customer.Firstname = txtFirstname.Text;
            customer.Street = txtStreet.Text;
            customer.ZipCode = txtZip.Text;
            customer.City = txtCity.Text;
        }

        private void FillUiFromCustomer(Customer? customer)
        {
            if (customer != null)
            {
                txtID.Text = customer.Id.ToString();
                txtName.Text = customer.Name;
                txtFirstname.Text = customer.Firstname;
                txtStreet.Text = customer.Street;
                txtZip.Text = customer.ZipCode;
                txtCity.Text = customer.City;
            }
            else
            {
                EmptyTextboxes(grpCustomer.Controls);
            }
        }

        private void EmptyTextboxes(Control.ControlCollection controls)
        {
            foreach (var control in controls)
            {
                var box = control as TextBox;
                if (box != null)
                {
                    box.Text = "";
                }
            }
        }

        private IEnumerable<Customer>? GetCustomers()
        {
            using (var db = new SqlTeacherDbContext()) {

                var result = db.Kundes.Select(kunde => new Customer()
                {
                    Id = kunde.Kundennr,
                    Name = kunde.Name ?? "?",
                    Firstname = kunde.Vorname ?? "?",
                    Street = kunde.Strasse ?? "?",
                    ZipCode = kunde.Plz ?? "?",
                    City = kunde.Ort ?? "?",
                    OrderCount = kunde.Bestellungs.Count()

                });
            return Customer.Load();
            }    
        }

        private void AddOrUpdateCustomer()
        {
            var customer = (CurrentCustomer != null) ? CurrentCustomer : new Customer();

            FillCustomerFromUi(customer);

            customer.Save();
        }

        private void RemoveCustomer(Customer customer)
        {
            customer.Delete();
            customer.Save();
        }
    }
}