using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskFlowStudent.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;
using System.Windows.Media;


namespace TaskFlowStudent.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TaskItem> allTasks = new List<TaskItem>();
        private string connectionString = "Server=LAPTOP-352KBCVK\\SQLEXPRESS;Database=TaskFlow;Trusted_Connection=True;";

        public MainWindow()
        {
            InitializeComponent();
            LoadTasksFromDatabase();
            SetupChart();

        }

        private void LoadTasksFromDatabase()
        {
            allTasks.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Tasks";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string statusStr = reader["Status"].ToString().Replace(" ", "");
                    Models.TaskStatus status;
                    if (!Enum.TryParse(statusStr, true, out status))
                    {
                        status = Models.TaskStatus.ToDo;
                    }

                    allTasks.Add(new TaskItem
                    {
                        Id = (int)reader["Id"],
                        Title = reader["Title"].ToString(),
                        Subject = reader["Subject"].ToString(),
                        Deadline = Convert.ToDateTime(reader["Deadline"]),
                        Status = status
                    });
                }
            }

            SetupChart();
            ApplyFilters();
        }


        private void SetupChart()
        {
            int toDoCount = allTasks.Count(t => t.Status.ToString() == "ToDo");
            int inProgressCount = allTasks.Count(t => t.Status.ToString() == "InProgress");
            int doneCount = allTasks.Count(t => t.Status.ToString() == "Done");

            TaskStatusChart.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "ToDo",
                    Values = new ChartValues<int> { toDoCount },
                    Fill = new SolidColorBrush(Color.FromRgb(255, 193, 7)), // galben
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "InProgress",
                    Values = new ChartValues<int> { inProgressCount },
                    Fill = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // albastru
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Done",
                    Values = new ChartValues<int> { doneCount },
                    Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // verde
                    DataLabels = true
                }
            };

            TaskStatusChart.LegendLocation = LegendLocation.Right;
        }


        private void ApplyFilters()
        {
            string searchText = SearchBox.Text?.ToLower() ?? "";
            string selectedStatus = (StatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = allTasks.Where(task =>
                (string.IsNullOrWhiteSpace(searchText) || task.Title.ToLower().Contains(searchText)) &&
                (selectedStatus == "Toate" || task.Status.ToString() == selectedStatus)
            ).ToList();

            if (TaskGrid != null)
            {
                TaskGrid.ItemsSource = filtered;
            }

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
           this.Hide();
            var window = new AddTaskWindow();
            if (window.ShowDialog() == true)
            {
                LoadTasksFromDatabase();
            }
            this.Show();
            
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var task = (sender as FrameworkElement)?.DataContext as TaskItem;
            if (task != null)
            {
                var window = new EditTaskWindow(task.Id);
                if (window.ShowDialog() == true)
                {
                    LoadTasksFromDatabase();
                }
                this.Show();
                
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var task = (sender as FrameworkElement)?.DataContext as TaskItem;
            if (task != null && MessageBox.Show("Ești sigur că vrei să ștergi acest task?", "Confirmare", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", task.Id);
                    cmd.ExecuteNonQuery();
                }

                LoadTasksFromDatabase();
            }
        }
    }
}

