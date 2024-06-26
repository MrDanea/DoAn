using DoAn.Services;
using MQTT;

namespace DoAn.Views.Loading
{
    public partial class CheckingNetworkView : ContentPage
    {
        private readonly AuthService _authService;
        public CheckingNetworkView(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }
        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            bool serverReady = await _authService.IsConnectedToNetworkAsync();
            while (!serverReady)
            {
                await DisplayAlert("Error", "Loi ket noi mang", "Thu lai");
                serverReady = await _authService.IsConnectedToNetworkAsync();
            }
            Broker.Instance.Connect();
            
            await Shell.Current.GoToAsync("//LoginView");
        }
    }

}

