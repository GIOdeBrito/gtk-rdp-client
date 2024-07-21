using System;
using System.Diagnostics;
using Gtk;
using Gdk;
using Pango;
using SaveXML.Model;
using XmlHelper.Open;

class Program
{
	private static Gtk.Window? win = null;
	private static Gtk.VBox? mbox = null;

	private static Gtk.VBox? boxLogin = null;
	private static Gtk.VBox? boxConfig = null;
	private static Gtk.VBox? boxCredits = null;

	private static Gtk.Entry? userField = null;
	private static Gtk.Entry? pwdField = null;
	private static Gtk.CheckButton? remember = null;

	private static Gtk.Entry? ipField = null;
	private static Gtk.Switch? ignoreCert = null;
	private static Gtk.Switch? timeoutSwitch = null;
	private static Gtk.Switch? fullscreenSwitch = null;

	private static string domain { get; set; }

	public static void Main (string[] args)
	{
		Application.Init();

		win = new Gtk.Window("RDP Client");
		win.SetDefaultSize(500, 300);
		win.Resizable = false;
		win.DeleteEvent += DestroyClient;
		win.SetPosition(WindowPosition.Center);

		CreateNav();
		LoginScreen();
		ConfigScreen();
		CreditsScreen();
		LoadData();

		win.ShowAll();

		SetTheme(AppContext.BaseDirectory + "style.css");

		Application.Run();
	}

	private static void SetTheme (string path)
    {
        CssProvider cssProvider = new CssProvider();
        cssProvider.LoadFromPath(path);

        StyleContext.AddProviderForScreen(
            Screen.Default,
            cssProvider,
            Gtk.StyleProviderPriority.Application
        );
    }

	public static void CreateNav ()
	{
		mbox = new VBox(false, 0);
		win.Add(mbox);

		VBox box1 = new VBox(false, 0);

		Stack stack = new Stack();

        StackSwitcher stackSwitcher = new StackSwitcher
        {
            Stack = stack
        };

		boxLogin = new VBox(false, 10);
		boxConfig = new VBox(false, 5);
		boxCredits = new VBox(false, 10);

        stack.AddTitled(boxLogin, "page1", "Login");
        stack.AddTitled(boxConfig, "page2", "Configurações");
        stack.AddTitled(boxCredits, "page3", "Créditos");

        box1.PackStart(stackSwitcher, false, false, 0);
        box1.PackStart(stack, true, true, 0);

		Gtk.Alignment alignNav = new Gtk.Alignment(0.5f, 0.5f, 0, 0);
		alignNav.Add(box1);

		mbox.PackStart(alignNav, false, false, 20);
	}

	public static void LoginScreen ()
	{
		// Divider
		boxLogin.PackStart(new Label(""), false, false, 15);

		userField = new Entry()
		{
			PlaceholderText = "Usuário"
		};

		boxLogin.PackStart(userField, false, false, 0);

		pwdField = new Entry()
		{
			PlaceholderText = "Senha",
			Visibility = false
		};

		boxLogin.PackStart(pwdField, false, false, 0);;

		remember = new CheckButton("Lembrar dados de login");
		boxLogin.PackStart(remember, false, false, 0);

		Button submit = new Button("Entrar");
		submit.StyleContext.AddClass("button-submit");

		submit.Clicked += (s, e) => FireRDPConnection();

		boxLogin.PackStart(submit, false, false, 0);

		Gtk.Alignment alignNav = new Gtk.Alignment(0.5f, 0.5f, 0, 0);
		alignNav.Add(boxLogin);

		mbox.PackStart(alignNav, true, true, 10);
	}

	public static void ConfigScreen ()
	{
		boxConfig.PackStart(new Label(""), false, false, 5);

		ipField = new Gtk.Entry()
		{
			Text = "192.168.0.250",
			PlaceholderText = "IP do servidor"
		};
		boxConfig.PackStart(ipField, false, false, 0);

		boxConfig.PackStart(new Label("Ignorar certificado"), false, false, 5);
		ignoreCert = new Gtk.Switch();
		boxConfig.PackStart(ignoreCert, false, false, 0);

		boxConfig.PackStart(new Label("Timeout 80000"), false, false, 5);
		timeoutSwitch = new Gtk.Switch();
		boxConfig.PackStart(timeoutSwitch, false, false, 0);

		boxConfig.PackStart(new Label("Tela cheia"), false, false, 5);
		fullscreenSwitch = new Gtk.Switch();
		boxConfig.PackStart(fullscreenSwitch, false, false, 0);

		mbox.Add(boxConfig);
	}

	public static void CreditsScreen ()
	{
		boxCredits.PackStart(new Label(""), false, false, 15);
		boxCredits.PackStart(new Label("RDP Client"), false, false, 0);

		/*PixbufAnimation diddyGif = new PixbufAnimation(AppContext.BaseDirectory + "Assets/diddy.gif");

		boxCredits.PackStart(new Image(diddyGif), false, false, 0);*/

		boxCredits.PackStart(new Label("Desenvolvido por Giordano 🕶️"), false, false, 0);
		boxCredits.PackStart(new Label("GTK Sharp 3.24"), false, false, 0);
		boxCredits.StyleContext.AddClass("box-credits");
	}

	public static void FireRDPConnection ()
	{
		SaveClientData();

		string server = ipField.Text;
		string user = userField.Text;
		string pwd = pwdField.Text;

		string baseCmd = $"/v:{server} /u:{user} /p:{pwd} /d:{domain} /sec:rdp";
		baseCmd += " /fonts";
		baseCmd += " /async-update";
		baseCmd += " /compression";
		baseCmd += $" /t:\"RDP Client - {server}\"";

		if(fullscreenSwitch.Active)
		{
			baseCmd += " /size:100%";
		}
		else
		{
			baseCmd += " /size:60%";
		}

		if(ignoreCert.Active)
		{
			baseCmd += " /cert:ignore";
		}

		try
		{
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = "/bin/xfreerdp",
				Arguments = baseCmd,
				/*RedirectStandardOutput = true,
				RedirectStandardError = true,*/
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using(Process process = new Process() { StartInfo = startInfo })
			{
				process.Start();

				win.Hide();

				process.WaitForExit();

				win.ShowAll();
			}
		}
		catch(Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public static void LoadData ()
	{
		XMLData? data = XmlRead.GetXmlData();

		if(data == null)
		{
			Console.WriteLine("Data is null");
			return;
		}

		/* Loads configurations first */

		fullscreenSwitch.Active = (bool) data.fullscreen;
		timeoutSwitch.Active = (bool) data.timeout;
		ignoreCert.Active = (bool) data.ignorecertificate;
		remember.Active = (bool) data.hasmemory;

		ipField.Text = data.server.Trim();
		domain = data?.domain.Trim();

		/* Loads user data */

		if(!(bool) data.hasmemory)
		{
			return;
		}

		userField.Text = data.user.Trim();
		pwdField.Text = Base64Decode(data.pwd.Trim());
	}

	public static void SaveClientData ()
	{
		XMLData newDataModel = new XMLData();

		newDataModel.server = ipField.Text ?? "";
		newDataModel.domain = domain ?? "";
		newDataModel.user = userField.Text ?? "";
		newDataModel.pwd = Base64Encode(pwdField.Text ?? "");

		newDataModel.hasmemory = remember.Active;
		newDataModel.timeout = timeoutSwitch.Active;
		newDataModel.fullscreen = fullscreenSwitch.Active;
		newDataModel.ignorecertificate = ignoreCert.Active;

		XmlHelper.Etch.XmlWrite.EtchDataXml(newDataModel);
	}

	public static string Base64Decode (string str64)
	{
		byte[] text64Bytes = System.Convert.FromBase64String(str64);
		return System.Text.Encoding.UTF8.GetString(text64Bytes);
	}

	public static string Base64Encode (string str)
	{
		byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(str);
		return System.Convert.ToBase64String(textBytes);
	}

	public static void DestroyClient (object obj, DeleteEventArgs args)
	{
		SaveClientData();

		Console.WriteLine("Shutting down...");
		win.Dispose();
		Application.Quit();
	}
}