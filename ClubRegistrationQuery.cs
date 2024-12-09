using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms; 

namespace ClubRegistration
{
    
    internal class ClubRegistrationQuery
    {
        
        private SqlConnection sqlConnect ;
        private SqlCommand sqlCommand;
        private SqlDataAdapter sqlDataAdapter;
        

        public DataTable dataTable;
        public BindingSource bindingSource;

        private string connectionString;



        public  ClubRegistrationQuery()
        {
            connectionString = @"Data Source=GHGK\SQLEXPRESS;Initial Catalog=ClubDB;Integrated Security=True;Encrypt=False";

            sqlConnect = new SqlConnection(connectionString);
            
            dataTable = new DataTable();
            bindingSource = new BindingSource();
        }

        public void DisplayList()
        {
            string ViewClubMembers = "SELECT ID, StudentId, FirstName, MiddleName, LastName, Age, Gender, Program FROM ClubMembers";
            sqlConnect.Open();

            sqlDataAdapter = new SqlDataAdapter(ViewClubMembers, sqlConnect);
            dataTable.Clear();
            sqlDataAdapter.Fill(dataTable);
            bindingSource.DataSource = dataTable;

            sqlConnect.Close();
        }



        public bool RegisterStudent(int ID, long StudentID, string FirstName, string MiddleName, string LastName, int Age, string Gender, string Program)
        {
            sqlCommand = new SqlCommand("INSERT INTO ClubMembers (ID, StudentID, FirstName, MiddleName, LastName, Age, Gender, Program) " +
                                       "VALUES (@ID, @StudentID, @FirstName, @MiddleName, @LastName, @Age, @Gender, @Program)", sqlConnect);

            sqlCommand.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
            sqlCommand.Parameters.Add("@StudentID", SqlDbType.BigInt).Value = StudentID;
            sqlCommand.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = FirstName;
            sqlCommand.Parameters.Add("@MiddleName", SqlDbType.VarChar).Value = MiddleName;
            sqlCommand.Parameters.Add("@LastName", SqlDbType.VarChar).Value = LastName;
            sqlCommand.Parameters.Add("@Age", SqlDbType.Int).Value = Age;
            sqlCommand.Parameters.Add("@Gender", SqlDbType.VarChar).Value = Gender;
            sqlCommand.Parameters.Add("@Program", SqlDbType.VarChar).Value = Program;
     
                sqlConnect.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnect.Close() ;  

            return true;
        }



    }
}
