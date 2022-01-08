using Model;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Proiect_DucaAndreea
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        EntitiesModel ctx = new EntitiesModel();
        CollectionViewSource utilizatorVSource;
        CollectionViewSource serviciiVSource;
        CollectionViewSource programariVSource;
        CollectionViewSource fidelitateVSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            utilizatorVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("utilizatorViewSource")));
            utilizatorVSource.Source = ctx.Utilizator.Local;
            ctx.Utilizator.Load();
            System.Windows.Data.CollectionViewSource serviciiViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("serviciiViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // serviciiViewSource.Source = [generic data source]
            serviciiVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("serviciiViewSource")));
            serviciiVSource.Source = ctx.Servicii.Local;
            ctx.Servicii.Load();
            System.Windows.Data.CollectionViewSource programariViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("programariViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // programariViewSource.Source = [generic data source]
            programariVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("programariViewSource")));
            BindDataGrid();
            ctx.Programari.Load();
            System.Windows.Data.CollectionViewSource fidelitateViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fidelitateViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // fidelitateViewSource.Source = [generic data source]
            fidelitateVSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fidelitateViewSource")));
            BindFidelitateDataGrid();
            ctx.Fidelitate.Load();

            cmbUserValues.ItemsSource = ctx.Utilizator.Local;
            cmbUserValues.SelectedValuePath = "Id";

            cmbProductValues.ItemsSource = ctx.Servicii.Local;
            cmbProductValues.SelectedValuePath = "Id";
        }
      
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
        }
        private void SaveUser()
        {
            Utilizator user = null;
            try
            {

            switch (action)
            {
                case ActionState.New:
                        user = new Utilizator()
                        {
                            Nume = tbNume.Text.Trim(),
                            Prenume = tbPrenume.Text.Trim(),
                            Email = tbEmail.Text.Trim(),
                            NumarTelefon = tbTel.Text.Trim()
                        };
                        ctx.Utilizator.Add(user);
                        utilizatorVSource.View.Refresh();
                        ctx.SaveChanges();
                break;
                case ActionState.Edit: 
                        user = (Utilizator)utilizatorDataGrid.SelectedItem;
                        user.Nume = tbNume.Text.Trim();
                        user.Prenume = tbPrenume.Text.Trim();
                        user.Email = tbEmail.Text.Trim();
                        user.NumarTelefon = tbTel.Text.Trim();
                        ctx.SaveChanges();
                    break;
                    case ActionState.Delete:
                        dynamic selectedUser = utilizatorDataGrid.SelectedItem;                  // luam user-ul selectat din interfata
                        int currentId = selectedUser.Id;                                         // luam id-ul (cheia primara) user-ului selectat
                        var deletedUser = ctx.Utilizator.FirstOrDefault(u => u.Id == currentId); // validam ca in DB exsita user-ul selectat din interfata
                        if (deletedUser != null)
                        {
                            ctx.Utilizator.Remove(deletedUser);                                   // stergem din DB user-ul selectat din interfata
                            ctx.SaveChanges();
                            utilizatorVSource.View.Refresh();
                            BindDataGrid();
                            BindFidelitateDataGrid();
                        }
                        break;
            }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveProduct()
        {
            Servicii product = null;
            decimal price;
            decimal.TryParse(tbPret.Text, out price);

            try
            {

            
            switch (action)
            {
                case ActionState.New:
                        product = new Servicii()
                        {
                            Nume = tbNumeServiciu.Text.Trim(),
                            Pret = price
                        };
                        ctx.Servicii.Add(product);
                        serviciiVSource.View.Refresh();
                        ctx.SaveChanges();
                    
                    break;
                case ActionState.Edit:
                        product = (Servicii)serviciiDataGrid.SelectedItem;
                        product.Nume = tbNume.Text.Trim();
                        product.Pret = price;
                        ctx.SaveChanges();
                    break;
                case ActionState.Delete:
                        dynamic selectedProduct = serviciiDataGrid.SelectedItem;                  // luam serviciul selectat din interfata
                        int currentId = selectedProduct.Id;                                       // luam id-ul (cheia primara) serviciului selectat
                        var deletedProduct = ctx.Servicii.FirstOrDefault(s => s.Id == currentId); // validam ca in DB exsita serviciul selectat din interfata
                        if (deletedProduct != null)
                        {
                            ctx.Servicii.Remove(deletedProduct);                                   // stergem din DB serviciul selectat din interfata
                            ctx.SaveChanges();
                            serviciiVSource.View.Refresh();
                            BindDataGrid();
                        }
                        break;
            }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveAppointment()
        {
            Programari appointment = null;

            switch (action)
            {
                case ActionState.New:
                    try
                    {
                        Utilizator user = (Utilizator)cmbUserValues.SelectedItem;
                        Servicii product = (Servicii)cmbProductValues.SelectedItem;

                        appointment = new Programari()
                        {
                            Serviciu = product.Id,
                            UtilizatorId = user.Id,
                            DataProgramarii = dpDate.SelectedDate.Value
                        };
                        ctx.Programari.Add(appointment);
                        ctx.SaveChanges();

                        int count = ctx.Programari.Count(p => p.UtilizatorId == user.Id);   // verificam cate programari are user-ul pt care am adugat programarea
                        if (count >= 2) // daca are >= 2 prog adaugam/updatam tabela fidelitate
                        {
                            var existedUser = ctx.Fidelitate.FirstOrDefault(f => f.UtilizatorId == user.Id); // verificam daca userul pt care am adaugat programarea exista in tabela fidelitate
                            if (existedUser != null) // daca user-ul exista atunci facem update la prop NrProgramari
                            {
                                existedUser.NrProgramari = count;
                            } 
                            else // daca user-ul nu exista in tabela Fidelitate adaugam o noua inregistare
                            {
                                Fidelitate f = new Fidelitate()
                                {
                                    UtilizatorId = user.Id,
                                    NrProgramari = count
                                };

                                ctx.Fidelitate.Add(f);
                            }
                            ctx.SaveChanges();
                            BindFidelitateDataGrid();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case ActionState.Edit:
                    try
                    {
                        cmbUserValues.IsEnabled = false;
                        dynamic selectedAppointment = programariDataGrid.SelectedItem;
                        int id = selectedAppointment.Id;

                        var editedAppointment = ctx.Programari.FirstOrDefault(p => p.Id == id);
                        if(editedAppointment != null)
                        {
                            if(cmbProductValues.SelectedValue != null)
                            {
                                editedAppointment.Serviciu = Int32.Parse(cmbProductValues.SelectedValue.ToString());
                            }

                            if (dpDate.SelectedDate != null)
                            {
                                editedAppointment.DataProgramarii = dpDate.SelectedDate.Value;
                            }
                            ctx.SaveChanges();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case ActionState.Delete:
                    dynamic deleteSelectedAppointment = programariDataGrid.SelectedItem; // luam row-ul selectat din interfata
                    int currentId = deleteSelectedAppointment.Id; // salvam Id-ul (cheia primara) a roe-ului selectat
                    var deleteAppointment = ctx.Programari.FirstOrDefault(p => p.Id == currentId); // cautam in DB inregistrarea cu cheia primara egala cu id-ul row-ului selectat din interfata
                   if(deleteAppointment != null)
                    {
                        ctx.Programari.Remove(deleteAppointment);
                        ctx.SaveChanges();
                        MessageBox.Show("Programarea a fost stearsa cu succes.", "Message");
                        BindDataGrid();

                        var existedGift = ctx.Fidelitate.FirstOrDefault(f => f.UtilizatorId == deleteAppointment.UtilizatorId);
                        if (existedGift == null) // nu exista in Fidelitate user-ul pentru care am sters programarea 
                        {
                            return; // iesim din functie
                        }
                        else
                        {
                            if (existedGift.NrProgramari == 2) //verificam daca nr de programari ale user-ului
                            {
                                // stergem inregistrarea din Fidelitate
                                ctx.Fidelitate.Remove(existedGift);
                                ctx.SaveChanges(); // salvam tabela fidelitate cu noile date
                            }
                            else // user-ul exista in Fidelitate dar are mai mult de 2 programari
                            {
                                existedGift.NrProgramari = existedGift.NrProgramari - 1; // decrementam nrProgramari cu 1
                                ctx.SaveChanges();
                            }
                        }
                        BindFidelitateDataGrid();
                    }
                    break;
            }

            BindDataGrid();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
        }

        private void DataGridCell_GotFocus(object sender, EventArgs e)
        {
           
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reinitialize();
        }

        private void gbOperations_Click(object sender, RoutedEventArgs e)
        {
            Button selectedButton = (Button)e.OriginalSource;
            Panel panel = (Panel)selectedButton.Parent;

            foreach (Button b in panel.Children)
            {
                if (b != selectedButton)
                {
                    b.IsEnabled = false;
                }
            }
            gbActions.IsEnabled = true;
        }


        private void Reinitialize ()
        {
            Panel panel = (Panel)gbOperations.Content;
            foreach (Button b in panel.Children)
            {
                b.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem tab = tbCtrlEntities.SelectedItem as TabItem;
            switch (tab.Header)
            {
                case "Utilizatori":
                    SaveUser();
                    break;
                case "Servicii":
                    SaveProduct();
                break;
                case "Programari":
                    SaveAppointment();
                break;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbItemFidelitate.IsSelected)                   // verificam daca tab-ull Fidelitate e selectat
            {
                gbOperations.Visibility = Visibility.Hidden;   // ascundem group-box-urile de butoane
                gbActions.Visibility = Visibility.Hidden;
            } 
            else
            {
                gbOperations.Visibility = Visibility.Visible;   // afisam group-box-urile de butoane
                gbActions.Visibility = Visibility.Visible;
            }
        }

        private void BindDataGrid()
        {
            var query = from schedule in ctx.Programari
                        join user in ctx.Utilizator on schedule.UtilizatorId equals user.Id
                        join product in ctx.Servicii on schedule.Serviciu equals product.Id
                        select new { user.Nume, user.Prenume, productName = product.Nume, product.Pret, schedule.DataProgramarii, schedule.Id };
            programariVSource.Source = query.ToList();
        }

        private void BindFidelitateDataGrid()
        {
            var query = from gift in ctx.Fidelitate
                        join user in ctx.Utilizator on gift.UtilizatorId equals user.Id
                        select new { user.Nume, user.Prenume, gift.NrProgramari, gift.Id };
            fidelitateVSource.Source = query.ToList();
        }
    }


}
