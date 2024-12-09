using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClubRegistration
{
    public partial class FrmClubRegistration : Form
    {
        private ClubRegistrationQuery clubRegistrationQuery;
        private int ID, Age, count;
        private string FirstName, LastName, MiddleName, Program, Gender;
        private long StudentId;

        private SqlConnection sqlConnect;
        private SqlCommand sqlCommand;

        public FrmClubRegistration()
        {
            InitializeComponent();
            clubRegistrationQuery = new ClubRegistrationQuery();

            
            sqlConnect = new SqlConnection(@"Data Source=GHGK\SQLEXPRESS;Initial Catalog=ClubDB;Integrated Security=True;Encrypt=False");
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            FrmUpdateMember frmUpdateMember = new FrmUpdateMember();
            frmUpdateMember.ShowDialog();
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshListOfClubMembers();
        }


        private void RefreshListOfClubMembers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=GHGK\SQLEXPRESS;Initial Catalog=ClubDB;Integrated Security=True;Encrypt=False"))
                {
                    string query = "SELECT ID, StudentID, FirstName, MiddleName, LastName, Age, Gender, Program FROM ClubMembers ORDER BY ID ASC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private int RegistrationID()
        {
            return count++;
        }


        private void FrmClubRegistration_Load(object sender, EventArgs e)
        {
            RefreshListOfClubMembers(); 
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {

            if (!ValidateInputs()) return;

            StudentId = long.Parse(txtStudentId.Text);

            
            if (IsStudentIdExists(StudentId))
            {
                MessageBox.Show("Your ID already exists.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FirstName = txtFirstName.Text;
            LastName = txtLastName.Text;
            MiddleName = txtMiddleName.Text;
            Program = txtProgram.Text;
            Gender = cmbGender.Text;
            Age = int.Parse(txtAge.Text);

            
            ResetIDs();

            
            bool success = clubRegistrationQuery.RegisterStudent(ID, StudentId, FirstName, MiddleName, LastName, Age, Gender, Program);

            if (success)
            {
                MessageBox.Show("Student successfully registered!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshListOfClubMembers(); 
            }
            else
            {
                MessageBox.Show("Failed to register the student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ClearInputs();
        }

        private void ResetIDs()
        {
            try
            {
                sqlCommand = new SqlCommand(
                    @"WITH CTE AS (
                SELECT ROW_NUMBER() OVER (ORDER BY ID) AS NewID, ID 
                FROM ClubMembers
            )
            UPDATE ClubMembers
            SET ID = CTE.NewID
            FROM ClubMembers
            INNER JOIN CTE ON ClubMembers.ID = CTE.ID;"
                ,
                    sqlConnect);

                
                if (sqlConnect.State != ConnectionState.Open)
                {
                    sqlConnect.Open();
                }

                sqlCommand.ExecuteNonQuery();

            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error during ID reset: " + sqlEx.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
                if (sqlConnect.State == ConnectionState.Open)
                {
                    sqlConnect.Close();
                }
            }
        }


        private bool IsStudentIdExists(long studentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=GHGK\SQLEXPRESS;Initial Catalog=ClubDB;Integrated Security=True;Encrypt=False"))
                {
                    string query = "SELECT COUNT(*) FROM ClubMembers WHERE StudentID = @StudentID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@StudentID", SqlDbType.BigInt).Value = studentId;

                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count > 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking Student ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtMiddleName.Clear();
            txtAge.Clear();
            txtStudentId.Clear();
            cmbGender.SelectedIndex = -1;
            txtProgram.SelectedIndex = -1;
        }


        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAge.Text) ||
                string.IsNullOrWhiteSpace(txtProgram.Text) ||
                cmbGender.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            if (!ValidateName(txtFirstName.Text) ||
                !ValidateName(txtLastName.Text) ||
                (!string.IsNullOrWhiteSpace(txtMiddleName.Text) && !ValidateName(txtMiddleName.Text)))
            {
                MessageBox.Show("Enter a proper name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            if (int.TryParse(txtAge.Text, out int age))
            {
                if (age >= 35)
                {
                    MessageBox.Show("Hindi kana pwede sa Club boss!.", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (age <= 12)
                {
                    MessageBox.Show("Bata kapa lods!", "Age Restriction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Enter a valid Age.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            if (!long.TryParse(txtStudentId.Text.Trim(), out _))
            {
                MessageBox.Show("Please enter a valid Student ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ValidateName(string name)
        {
            
            if (!char.IsUpper(name[0])) return false;

            
            return name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-' || c == '\'');
        }




    }
}
