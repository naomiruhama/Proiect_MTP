using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskFlowStudent.Views
{
    /// <summary>
    /// Interaction logic for EditTaskWindow.xaml
    /// </summary>
    public partial class EditTaskWindow : Window
    {
        private string connectionString = "Server=LAPTOP-352KBCVK\\SQLEXPRESS;Database=TaskFlow;Trusted_Connection=True;";
        private int taskId;

        public EditTaskWindow(int taskId)
        {
            InitializeComponent();
            this.taskId = taskId;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Title, Subject, Deadline, Status FROM Tasks WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", taskId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        TitleBox.Text = reader["Title"].ToString();
                        SubjectBox.Text = reader["Subject"].ToString();
                        DeadlinePicker.SelectedDate = Convert.ToDateTime(reader["Deadline"]);
                        string status = reader["Status"].ToString();

                        if (status == "ToDo") StatusCombo.SelectedIndex = 0;
                        else if (status == "InProgress") StatusCombo.SelectedIndex = 1;
                        else if (status == "Done") StatusCombo.SelectedIndex = 2;
                        else StatusCombo.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la încărcarea taskului: " + ex.Message);
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string subject = SubjectBox.Text.Trim();
            DateTime? deadline = DeadlinePicker.SelectedDate;
            string status = (StatusCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(subject) || !deadline.HasValue || string.IsNullOrEmpty(status))
            {
                ErrorText.Text = "Completează toate câmpurile!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Tasks SET Title = @title, Subject = @subject, Deadline = @deadline, Status = @status WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@subject", subject);
                    cmd.Parameters.AddWithValue("@deadline", deadline);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = "Eroare la salvare: " + ex.Message;
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}

