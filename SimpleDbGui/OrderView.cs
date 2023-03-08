using SimpleDbGui.Data;

namespace SimpleDbGui
{
    public partial class OrderView : Form
    {
        private Customer? _customerSelected;

        public Customer? CustomerSelected
        {
            get
            {
                return _customerSelected;
            }
            set
            {
                if (value != _customerSelected)
                {
                    _customerSelected = value;
                    RefreshData();
                }
            }
        }

        public OrderView()
        {
            InitializeComponent();
        }

        private void Orders_Shown(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            if (CustomerSelected != null)
            {
                gridOrders.DataSource = CustomerSelected.Orders;
            }
            else
            {
                gridOrders.DataSource = null;
            }
        }
    }
}
