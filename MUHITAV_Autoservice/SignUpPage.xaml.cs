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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MUHITAV_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        private ClientService _currentClientService = new ClientService();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = МУХИТАОАОВ_автосервисEntities.GetContext().Clients.ToList();
            ComboClient.ItemsSource = _currentClient;
        }
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();


            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");


            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                МУХИТАОАОВ_автосервисEntities.GetContext().ClientService.Add(_currentClientService);
            try
            {
                МУХИТАОАОВ_автосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 4 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32((start[1].ToString()));

                if (startHour / 60 > 23 || startMin > 59)
                {
                    MessageBox.Show("Введите действительное время");
                    TBStart.Clear();
                    return;
                }

                int sum = startHour + startMin + _currentService.DurationInSeconds;

                int EndHour = sum / 60;
                int EndMin = sum % 60;

                if (EndHour > 23)
                    EndHour -= 24;
                s = EndHour.ToString() + ":";

                if (EndMin == 0)
                    s += "00";
                else if (EndMin > 0 && EndMin < 10)
                    s = s + '0' + EndMin;
                else
                    s += EndMin.ToString();

                TBEnd.Text = s;
            }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //              0                       0                       1                           0
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ":") && (!TBStart.Text.Contains(":") && TBStart.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
    }
}
