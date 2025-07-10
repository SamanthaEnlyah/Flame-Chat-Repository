using FlameChatClient.GUI;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlameChatClient.GUI;
using FlameChatClient.Client_Operations;
using FlameChatClient.ChatClient;


namespace FlameChat2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MessageBox.Show("Welcome to FlameChat! ");



       
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        SignUpChatUser signUp = new SignUpChatUser();
        signUp.Show();
    }

    private void btn_login_Click(object sender, RoutedEventArgs e)
    {
        LogInChatUser logIn = new LogInChatUser(); // Ensure 'LogInChatUser' is defined in the correct namespace
        logIn.Show();
    }


    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (SessionManager.IsSessionStarted())
        {
            // If the session is already started, navigate to the chat window
            ChatWindow chatWindow = new ChatWindow();
            await chatWindow.StartClientIfNeeded();
            chatWindow.Show();
            chatWindow.SetAllData();
           
        }
    }
}
