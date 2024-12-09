using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClubRegistration
{
    public partial class FrmUpdateMember : Form
    {
        
        private string connectionString;
        private SqlConnection sqlConnect ;
        private SqlCommand sqlCommand;
        private SqlDataAdapter sqlDataAdapter;
        public DataTable dataTable;
        public BindingSource bindingSource;

        public FrmUpdateMember()
        {
            InitializeComponent();
            connectionString = @"Data Source=GHGK\SQLEXPRESS;Initial Catalog=ClubDB;Integrated Security=True;Encrypt=False";
            sqlConnect = new SqlConnection(connectionString);
            dataTable = new DataTable();
            bindingSource = new BindingSource();
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null) return;

            long selectedStudentId = (long)comboBox1.SelectedValue;

            sqlCommand = new SqlCommand("UPDATE ClubMembers SET FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, Age = @Age, Gender = @Gender, Program = @Program WHERE StudentID = @StudentID", sqlConnect);
            sqlCommand.Parameters.Add("StudentID", SqlDbType.BigInt).Value = selectedStudentId;
            sqlCommand.Parameters.Add("FirstName", SqlDbType.VarChar).Value = textBox1.Text;
            sqlCommand.Parameters.Add("MiddleName", SqlDbType.VarChar).Value = textBox2.Text;
            sqlCommand.Parameters.Add("LastName", SqlDbType.VarChar).Value = textBox3.Text;
            sqlCommand.Parameters.Add("Age", SqlDbType.Int).Value = int.Parse(textBox4.Text);
            sqlCommand.Parameters.Add("Gender", SqlDbType.VarChar).Value = comboBox2.SelectedItem.ToString();
            sqlCommand.Parameters.Add("Program", SqlDbType.VarChar).Value = comboBox3.SelectedItem.ToString();

            sqlConnect.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnect.Close();

            MessageBox.Show("Member information updated successfully.", "Update Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue == null)
                {
                    MessageBox.Show("No Student ID selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                long selectedStudentId;
                if (comboBox1.SelectedValue is DataRowView rowView)
                {
                    selectedStudentId = Convert.ToInt64(rowView["StudentID"]);
                }
                else
                {
                    selectedStudentId = Convert.ToInt64(comboBox1.SelectedValue);
                }

                
                sqlCommand = new SqlCommand("SELECT FirstName, MiddleName, LastName, Age, Gender, Program FROM ClubMembers WHERE StudentID = @StudentID", sqlConnect);
                sqlCommand.Parameters.Add("@StudentID", SqlDbType.BigInt).Value = selectedStudentId;

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable studentInfo = new DataTable();
                sqlDataAdapter.Fill(studentInfo);

                if (studentInfo.Rows.Count > 0)
                {
                    DataRow row = studentInfo.Rows[0];

                    
                    textBox1.Text = row["FirstName"].ToString();
                    textBox2.Text = row["MiddleName"].ToString();
                    textBox3.Text = row["LastName"].ToString();
                    textBox4.Text = row["Age"].ToString();
                    comboBox2.SelectedItem = row["Gender"].ToString();
                    comboBox3.SelectedItem = row["Program"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching student details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateGenderComboBox()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Male");
            comboBox2.Items.Add("Female");
            comboBox2.Items.Add("Other");
        }



        private void FrmUpdateMember_Load(object sender, EventArgs e)
        {
            try
            {
                
                PopulateGenderComboBox();
                PopulateProgramComboBox();

                
                sqlCommand = new SqlCommand("SELECT StudentID FROM ClubMembers", sqlConnect);
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable studentIds = new DataTable();
                sqlDataAdapter.Fill(studentIds);

                comboBox1.DataSource = studentIds;
                comboBox1.DisplayMember = "StudentID";
                comboBox1.ValueMember = "StudentID";
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void PopulateProgramComboBox()
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("BS in Information Technology");
            comboBox3.Items.Add("BS in Tourism Management");
            comboBox3.Items.Add("BS in Hospitality Management");
            comboBox3.Items.Add("BS in Accountancy");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a valid Student ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                long selectedStudentId;
                if (comboBox1.SelectedValue is DataRowView rowView)
                {
                    selectedStudentId = Convert.ToInt64(rowView["StudentID"]);
                }
                else
                {
                    selectedStudentId = Convert.ToInt64(comboBox1.SelectedValue);
                }

                var confirmResult = MessageBox.Show(
                    "Sure ka boss??",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                    sqlCommand = new SqlCommand("DELETE FROM ClubMembers WHERE StudentID = @StudentID", sqlConnect);
                    sqlCommand.Parameters.Add("StudentID", SqlDbType.BigInt).Value = selectedStudentId;

                    sqlConnect.Open();
                    int rowsAffected = sqlCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Member deleted successfully.", "Deletion Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Reset IDs after deletion
                        ResetIDs();
                    }
                    else
                    {
                        MessageBox.Show("No record found with the selected Student ID.", "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    sqlConnect.Close();
                    this.Close();
                }
                
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting member: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlConnect.State == ConnectionState.Open)
                {
                    sqlConnect.Close();
                }
            }
        }

        // Method to reset IDs in the database
        private void ResetIDs()
        {
            try
            {
                sqlCommand = new SqlCommand(
                    @"WITH CTE AS (
                SELECT ROW_NUMBER() OVER (ORDER BY ID) - 1 AS NewID, ID 
                FROM ClubMembers
            )
            UPDATE ClubMembers
            SET ID = CTE.NewID
            FROM ClubMembers
            INNER JOIN CTE ON ClubMembers.ID = CTE.ID;",
                    sqlConnect);

                // Open the connection only if it is not already open
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
                // Close the connection only if it is still open
                if (sqlConnect.State == ConnectionState.Open)
                {
                    sqlConnect.Close();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FrmUpdateMember frmUpdateMember = new FrmUpdateMember();
            this.Close();
        }
    }
}
