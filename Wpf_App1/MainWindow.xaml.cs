using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data;
using DBConnection;

namespace Wpf_App1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string dbConnectionString = @"Data Source=database.db;version=3;";
        string dbConnectionString = new GetAConnectionNow().getADBString();
        bool blnLoggedIn = false;

        static Dictionary<int, string> sqlCmdTypes = new Dictionary<int, string>
    {
	{0, "select"},
	{1, "insert"},
	{2, "update"},
	{3, "delete"}
    };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_1_Click(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("Test Message");            
            try
            {
                string Query = "select eid,roles from employeeinfo where username = '" + this.txt_username.Text + "' and password = '" + this.txt_password.Password + "';";
                SQLiteConnection sqliteCon = getDBConnection();
                SQLiteDataReader dataReader = runQuery(Query, sqliteCon);
                
                int count = 0;
                string userRole = "";
                while (dataReader.Read())
                {
                    if (dataReader.HasRows)
                    {
                        userRole = dataReader["roles"].ToString();
                    }
                    count++;
                }
                if(count == 1)
                {
                    MessageBox.Show("Username and password is correct");
                    if(blnLoggedIn)
                    {
                        logoutUser();     
                    }
                    ActivateTabs(userRole);
                    blnLoggedIn = true;
                }
                if (count > 1)
                {
                    if (blnLoggedIn)
                    {
                        logoutUser();
                    }
                    MessageBox.Show("Duplicate Username and password...Try Again");
                }
                if (count < 1)
                {
                    if (blnLoggedIn)
                    {
                        logoutUser();
                    }
                    MessageBox.Show("Username and password is not correct");
                }
                closeDBConnection(sqliteCon);
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void logoutUser()
        {
            blnLoggedIn = false;
            foreach (TabItem item in tabWindow.Items)
            {
                if (item.Name != tab_login.Name)
                {
                    item.IsEnabled = false;
                }
            }   
        }
        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void btn_logOut_Click(object sender, RoutedEventArgs e)
        {
            blnLoggedIn = false;
            foreach (TabItem item in tabWindow.Items)
            {
                if (item.Name != tab_login.Name)
                {
                    item.IsEnabled = false;
                }
            }              
            this.tab_search.IsEnabled = false;
            MessageBox.Show("You have logged out");
        }

        class Employee
        {
            public string Name { get; set; }
            public string surName { get; set; }
            public string userName { get; set; }
            public int age { get; set; }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try{
                string Query = "";
                if (this.ckbox_ageOnly.IsChecked == true)
                {
                    Query = "select eid,name,surname,age,username from employeeinfo where age = " + this.txt_search.Text + ";";
                }
                else
                {
                    Query = "select eid,name,surname,age,username from employeeinfo where username like '%" + this.txt_search.Text + "%' or name like '%" + this.txt_search.Text + "%' or surname like '%" + this.txt_search.Text + "%';";
                }
            SQLiteConnection sqliteCon = getDBConnection();
            SQLiteDataReader dataReader = runQuery(Query, sqliteCon);

            DataTable dt = new DataTable();
            dt.Load(dataReader);
            this.grd_employeeSearch.ItemsSource = dt.DefaultView;
            
            closeDBConnection(sqliteCon);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private SQLiteDataReader runQuery(string Query, SQLiteConnection sqliteCon)
        {
            SQLiteCommand createCommand = new SQLiteCommand(Query, sqliteCon);
            createCommand.ExecuteNonQuery();
            SQLiteDataReader dr = createCommand.ExecuteReader();
            return dr;
        }

        private void runQuery(string Query, SQLiteConnection sqliteCon, bool blnNotSelect = true)
        {
            SQLiteCommand createCommand = new SQLiteCommand(Query, sqliteCon);
            createCommand.ExecuteNonQuery();           
        }

        private SQLiteConnection getDBConnection()
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(dbConnectionString);
            sqliteCon.Open();
            return sqliteCon;
        }

        private void closeDBConnection(SQLiteConnection sqliteCon)
        {
            sqliteCon.Close();
        }

        private void grd_employeeSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Query = "";
                
                    Query = "insert into employeeinfo (name,surname,age,username,password) values('Becky','Randolph',41,'brandolph','nut');";
                
                SQLiteConnection sqliteCon = getDBConnection();
                runQuery(Query, sqliteCon,true);
                closeDBConnection(sqliteCon);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ActivateTabs(string role)
        {
            try
            {
                string Query = "";
                string formIdList = "";
                Query = "select FormId from forms where (cast(roles as bit) & " + Convert.ToInt32(role) + ") <> 0;";
                
                SQLiteConnection sqliteCon = getDBConnection();
                SQLiteDataReader dataReader2 = runQuery(Query, sqliteCon);

                while (dataReader2.Read())
                {
                    if (dataReader2.HasRows)
                    {
                        formIdList = formIdList + "," + dataReader2["FormId"];
                    }
                }

                    foreach (TabItem item in tabWindow.Items)
                    {
                        if(formIdList.Contains(item.Name))
                        {
                            item.IsEnabled = true;
                            if (item.Name.ToString() == "tab_search")
                            {
                                item.IsSelected = true;
                            }
                        }
                    }              

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
