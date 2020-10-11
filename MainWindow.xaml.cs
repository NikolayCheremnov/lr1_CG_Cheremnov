using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lr1_CG_Cheremnov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Log> logs;
        public MainWindow()
        {
            InitializeComponent();
       
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            logs = new ObservableCollection<Log>();
            dg_logs.ItemsSource = logs;
            logs.Add(new Log() { Time = DateTime.Now.ToString(), Action = "App is running", Logs = "Successfully" }); // log_action
            lb_projection_type.SelectedIndex = 0;
        }

        private void lb_projection_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logs.Add(new Log() { Time = DateTime.Now.ToString(), Action = "Projection type is set", Logs = lb_projection_type.SelectedItem.ToString() }); // log_action
        }
    }
}
