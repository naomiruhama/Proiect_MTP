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
    /// Interaction logic for AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        private string connectionString = "Server=LAPTOP-352KBCVK\\SQLEXPRESS;Database=TaskFlow;Trusted_Connection=True;";

        public AddTaskWindow()
        {
            InitializeComponent();
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

            // Validare
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(subject) || !deadline.HasValue || string.IsNullOrEmpty(status))
            {
                ErrorText.Text = "Completează toate câmpurile!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (deadline.Value.Date < DateTime.Today)
            {
                ErrorText.Text = "Data limită nu poate fi în trecut.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }


            // Inserare în baza de date
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Tasks (Title, Subject, Deadline, Status) VALUES (@title, @subject, @deadline, @status)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@subject", subject);
                    cmd.Parameters.AddWithValue("@deadline", deadline);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }

                this.DialogResult = true; // Închide fereastra și reîncarcă grila în MainWindow
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

