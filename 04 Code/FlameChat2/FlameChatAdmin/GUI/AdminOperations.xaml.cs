using FlameChatClient.ChatClient;
using FlameChatShared.Communication;
using Microsoft.Win32;
using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
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

namespace FlameChatAdmin.GUI
{
    /// <summary>
    /// Interaction logic for AdminOperations.xaml
    /// </summary>
    public partial class AdminOperations : Window
    {
        public AdminOperations()
        {
            InitializeComponent();
            Test t = new Test();
            
            GlobalVariables.MessagesFromClient = t._test_messages;

            t.Show();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            await GlobalVariables.GetClient().StartClient();
            RefreshTraits();
        }

        private async void BtnAddTrait_Click(object sender, RoutedEventArgs e)
        {
           if(string.IsNullOrEmpty(txtTrait.Text))
            {

                MessageBox.Show("Please enter a trait.");
                return;
            }

            string trait = txtTrait.Text;
            await GlobalVariables.GetClient().SendCommandToServer(AdminUserAllowedCommands.AddPersonalityTrait, trait);
            txtTrait.Text = string.Empty;

            RefreshTraits();
        }

        public async void RefreshTraits()
        {
             Response r = await GlobalVariables.GetClient().SendCommandToServer(AdminUserAllowedCommands.GetAllPersonalityTraits, null);
             List<PersonalityTrait> traits = (List<PersonalityTrait>)r.Value;
            
            listOfTraits.ItemsSource = traits;
            listOfTraits.DisplayMemberPath= "TraitName";
            listOfTraits.Items.Refresh();
        }

      

        private async void BtnAddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt";
            List<string> traits = new List<string>();
            if (openFileDialog.ShowDialog() == true)
            {
                string[] lines = File.ReadAllLines(openFileDialog.FileName);

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (await TraitExists(line.Trim()))
                            {
                                GlobalVariables.LogMessage("Trait already exists: " + line);
                            }
                            else
                            {
                                GlobalVariables.LogMessage("Gui: Adding trait: " + line.Trim());
                                traits.Add(line.Trim());

                            }
                        }

                }
                    
                List<string> traitsSorted = traits.OrderBy(t => t).ToList(); // Sort traits alphabetically

                GlobalVariables.LogMessage("Admin operations  Gui: " + traitsSorted.Count + " different messages found in file");


                Response r = await GlobalVariables.GetClient().SendCommandToServer(AdminUserAllowedCommands.AddManyPersonalityTraits, traitsSorted);
                RefreshTraits();

                //GlobalVariables.LogMessage("kraj, traits count: " + traits.Count);
            }
            
            
             
        }

        private async Task<bool> TraitExists(string trait)
        {
            Response r = await GlobalVariables.GetClient().SendCommandToServer(AdminUserAllowedCommands.PersonalityTraitExists, trait);
            foreach (PersonalityTrait pt in listOfTraits.Items)
            {
                if (pt.TraitName == trait)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
