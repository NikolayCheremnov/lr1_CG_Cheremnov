using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        WireObject axes;
        WireObject wo_object;

        public MainWindow()
        {
            InitializeComponent();

            axes = new WireObject(@"axes", @"Resources\axes.wo");
            wo_object = null;
            
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
            SceneRedrawing();
        }

        // methods for drawing on canvas
        private void DrawingWO(WireObject wo, bool isClone)
        {
            switch (lb_projection_type.SelectedIndex)
            {
                case 0:
                    // test mode
                    foreach(((double x, double y, double z) p1, (double x, double y, double z) p2, string colorStr) in wo.Ridge )
                    {
                        CanvasArea.Children.Add(SceneProcessor.PreparatingLine(colorStr, p1.x, p1.y, p2.x, p2.y, isClone));
                    }
                    logs.Add(new Log() { Time = DateTime.Now.ToString(), Action = "Drawing WO", Logs = wo.Name + " are drawn in test mode" }); // log_action
                    break;
                case 1:
                    // first mode - "free projection"
                    WireObject projected_wo = SceneProcessor.PreparingFreeProjection(wo);
                    foreach (((double x, double y, double z) p1, (double x, double y, double z) p2, string colorStr) in projected_wo.Ridge)
                    {
                        CanvasArea.Children.Add(SceneProcessor.PreparatingLine(colorStr, p1.x, p1.y, p2.x, p2.y, isClone));
                    }
                    logs.Add(new Log() { Time = DateTime.Now.ToString(), Action = "Drawing WO", Logs = projected_wo.Name + " are drawn in free projection mode" }); // log_action
                    break;
                default:
                    logs.Add(new Log() { Time = DateTime.Now.ToString(), Action = "Drawing WO", Logs = "drawing skipped" }); // log action
                    break;
            }
        }

        private void mi_load_object_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadObjectFileDialog = new OpenFileDialog();
            loadObjectFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            loadObjectFileDialog.Filter = "Wired Object files|*.wo";
            loadObjectFileDialog.ShowDialog();
            wo_object = new WireObject(loadObjectFileDialog.SafeFileName, loadObjectFileDialog.FileName);
            SceneRedrawing();
        }

        private void mi_remove_from_scene_Click(object sender, RoutedEventArgs e)
        {
            wo_object = null;
            SceneRedrawing();
        }

        private void tb_scalling_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (wo_object != null)
            {
                wo_object.Scalling(tb_ox_scalling.Text, tb_oy_scalling.Text, tb_oz_scalling.Text);
                SceneRedrawing();
            }
        }

        private void s_rotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (wo_object != null)
            {
                wo_object.Rotate(s_ox_rotate.Value, s_oy_rotate.Value, s_oz_rotate.Value);
                SceneRedrawing();
            }
        }

        private void mi_reset_Click(object sender, RoutedEventArgs e)
        {
            s_ox_rotate.Value = s_oy_rotate.Value = s_oz_rotate.Value = 0;
            tb_ox_scalling.Text = tb_oy_scalling.Text = tb_oz_scalling.Text = "100";
            tb_ox_shift.Text = tb_oy_shift.Text = tb_oz_shift.Text = "0";
            wo_object = new WireObject(wo_object.Name, wo_object.Path);
            SceneRedrawing();
        }

        private void tb_shift_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (wo_object != null)
            {
                wo_object.Shift(tb_ox_shift.Text, tb_oy_shift.Text, tb_oz_shift.Text);
                SceneRedrawing();
            }
        }

        private void tb_ox_shift_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                tb_ox_shift.Text = Convert.ToString(Convert.ToInt32(tb_ox_shift.Text) + 1);
            else
                tb_ox_shift.Text = Convert.ToString(Convert.ToInt32(tb_ox_shift.Text) - 1);
        }

        private void tb_oy_shift_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                tb_oy_shift.Text = Convert.ToString(Convert.ToInt32(tb_oy_shift.Text) + 1);
            else
                tb_oy_shift.Text = Convert.ToString(Convert.ToInt32(tb_oy_shift.Text) - 1);
        }

        private void tb_oz_shift_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                tb_oz_shift.Text = Convert.ToString(Convert.ToInt32(tb_oz_shift.Text) + 1);
            else
                tb_oz_shift.Text = Convert.ToString(Convert.ToInt32(tb_oz_shift.Text) - 1);
        }

        private void XY_Button_Click(object sender, RoutedEventArgs e)
        {
            if (wo_object != null)
            {
                wo_object.XY_Mirror();
                SceneRedrawing();
            }
        }

        private void YZ_Button_Click(object sender, RoutedEventArgs e)
        {
            if (wo_object != null)
            {
                wo_object.YZ_Mirror();
                SceneRedrawing();
            }
        }

        private void ZX_Button_Click(object sender, RoutedEventArgs e)
        {
            if (wo_object != null)
            {
                wo_object.ZX_Mirror();
                SceneRedrawing();
            }
        }

        private void SceneRedrawing()
        {
            CanvasArea.Children.Clear();
            DrawingWO(axes, false);
            if (wo_object != null)
            {
                DrawingWO(wo_object, false);
                if ((bool)rb_two_wo_xy.IsChecked) // the second is relative to xy
                {
                    wo_object.XY_Mirror();
                    DrawingWO(wo_object, true);
                    wo_object.XY_Mirror();
                } else if ((bool)rb_two_wo_yz.IsChecked)
                {
                    wo_object.YZ_Mirror();
                    DrawingWO(wo_object, true);
                    wo_object.YZ_Mirror();
                } else if ((bool)rb_two_wo_zx.IsChecked)
                {
                    wo_object.ZX_Mirror();
                    DrawingWO(wo_object, true);
                    wo_object.ZX_Mirror();
                }
            }
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            SceneRedrawing();
        }
    }
}
