using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdoNetDisconnectedMode
{
    public partial class MainWindow : Window
    {

        private Command chooseShowUsersCommand;
        private Command updateCommand;
        private Command fillCommand;
        public ICommand ChooseShowUsersCommand => chooseShowUsersCommand;

        private string commandText;
        public string CommandText
        {
            get { return commandText; }
            set
            {
                if (value != commandText)
                {
                    commandText = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand UpdateCommand => updateCommand;
        public ICommand FillCommand => fillCommand;
        private bool isAdmins = false;
        public bool IsAdmins
        {
            get { return isAdmins; }
            set
            {
                if (value != isAdmins)
                {

                    isAdmins = value;
                    OnPropertyChanged();
                }
            }
        }

        private SqlConnection conn = null;

        private SqlDataAdapter da = null;

        private DataSet set = null;


        private void Fill()
        {
            try
            {

                string sql = commandText;

                da = new SqlDataAdapter(sql, conn);

                new SqlCommandBuilder(da);
                set = new DataSet();

                da.Fill(set, "Users");
                DataView view = set.Tables[0].AsDataView();
                if (isAdmins)
                {
                    view.RowFilter = "IsAdmin=1";
                }
                dataGrid.ItemsSource = view;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Update()
        {
            try
            {
                da.Update(set, "Users");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            ;
        }
        public void ChooseShowUsers()
        {
            try
            {
                if (set != null)
                {
                    DataView view = set.Tables[0].AsDataView();
                    if (isAdmins)
                    {
                        view.RowFilter = "IsAdmin=1";
                    }
                    dataGrid.ItemsSource = view;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            string cs = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            chooseShowUsersCommand = new DelegateCommand(ChooseShowUsers);
            updateCommand = new DelegateCommand(Update);
            fillCommand = new DelegateCommand(Fill);
            conn = new SqlConnection(cs);
        }

    }
}
