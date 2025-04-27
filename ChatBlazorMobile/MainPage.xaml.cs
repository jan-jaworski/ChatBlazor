namespace ChatBlazorMobile;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void registerButton_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new RegisterPage());
    }
}