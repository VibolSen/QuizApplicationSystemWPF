using QuizApplicationSystemWPF.View;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuizApplicationSystem.View
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text;
            string password = txtPass.Password;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally redirect to another view or main application window
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPass.Clear();
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "user.txt");
                if (File.Exists(filePath))
                {
                    string hashedPassword = HashPassword(password);
                    foreach (var line in File.ReadLines(filePath))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 2 && parts[0] == username && parts[1] == hashedPassword)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception details
                LogError(ex);
                MessageBox.Show($"Error reading user data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterView registerView = new RegisterView();
            registerView.Show();
            this.Close(); // Close the current LoginView
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Optionally add logic to handle text changes in the username field
            // For example, validate input or enable/disable the login button
        }

        private void LogError(Exception ex)
        {
            // Implement your logging logic here
            // Example: Write the error details to a log file
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {ex.ToString()}{Environment.NewLine}");
        }
    }
}
