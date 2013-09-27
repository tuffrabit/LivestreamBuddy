using System;
using System.Collections.Generic;
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

namespace LivestreamBuddyNew
{
    /// <summary>
    /// Interaction logic for SingleTextBoxWindow.xaml
    /// </summary>
    public partial class SingleTextBoxWindow : Window
    {
        public SingleTextBoxWindow()
        {
            InitializeComponent();
        }

        public SingleTextBoxWindow(string windowTitle, string labelText, string buttonText)
            : this()
        {
            this.Title = windowTitle;
            lblLabel.Text = labelText;

            txtTextBox.Width = 350 - 26 - (labelText.Length * 8);

            btnButton.Content = buttonText;
            btnButton.Width = buttonText.Length * 13;
        }

        public string Value { get; private set; }

        # region Events

        private void btnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = txtTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }

        # endregion
    }
}
