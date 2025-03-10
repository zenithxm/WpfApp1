using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Model;
using WpfApp1.View_Model;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //helper to make sure tabcontrol clean it cache when user close tab without change selection tab
        //because there is no direct event to call for this one
        //so use resize event that can trigger manually
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ((Main)this.DataContext).IsTabRefresh = false;

            double offset = elMainControl.ActualWidth;
            elMainControl.Width = offset + 1;
            elMainControl.UpdateLayout();
            elMainControl.Width = offset;
            elMainControl.UpdateLayout();
        }
    }
}
