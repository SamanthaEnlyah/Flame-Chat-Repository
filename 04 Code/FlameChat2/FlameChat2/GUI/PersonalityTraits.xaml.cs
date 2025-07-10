using FlameChatClient.Client_Operations;
using SharedModel.Models;
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

namespace FlameChatClient.GUI
{
    /// <summary>
    /// Interaction logic for PersonalityTraits.xaml
    /// </summary>
    public partial class PersonalityTraits : Window
    {
        UserInfo userInfo;
        public PersonalityTraits(UserInfo userInfo)
        {
            InitializeComponent();
            this.userInfo = userInfo;

        }

        private void BtnAddToUserList_Click(object sender, RoutedEventArgs e)
        {
            userInfo.AddTrait((PersonalityTrait)listOfAllTraits.SelectedValue);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            PersonalityTraitsOp personalityTraits = new PersonalityTraitsOp();
            List<PersonalityTrait> traits = await personalityTraits.GetAllPersonalityTraits();
            listOfAllTraits.ItemsSource = traits;
            listOfAllTraits.DisplayMemberPath = "TraitName";
        }
    }
}
