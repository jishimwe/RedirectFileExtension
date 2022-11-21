using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.RpcContracts.DiagnosticManagement;
using Task = System.Threading.Tasks.Task;
using System.Windows.Forms;

namespace RedirectFileExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class RedirectProjectConfig
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 4134;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("8255648a-a3bc-442f-9797-63a14abfa17c");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		// Constants
		public static readonly string RedirectRepositoryUrl = "RedirectRepositoryUrl",
			RedirectDirectoryPath = "RedirectDirectoryPath",
			RealRepositoryUrl = "RealRepositoryUrl",
			RealRepositoryPath = "RealRepositoryPath",
			Username = "Username",
			Mail = "Mail",
			TokenPath = "TokenPath",
			BranchName = "BranchName",
			RemoteName = "RemoteName",
			RefSpecs = "RefSpecs",
			Filepath = "Filepath",
			Message = "Message",
			RedirectFile = "RedirectFile",
			MergeOptions = "MergeOptions";

		private static readonly string _configPath = "redirect.ini";

		public static readonly string _redirectUtilitiesPath =
			@"C:\Users\test\Documents\RedirectFileExtension\RedirectFileExtension\RedirectFilesUtilities\RedirectFilesUtilities.exe";

		public static readonly ProcessStartInfo psiUtilities = new ProcessStartInfo()
		{
			FileName = _redirectUtilitiesPath,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="RedirectProjectConfig"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private RedirectProjectConfig(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(this.Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static RedirectProjectConfig Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in RedirectProjectConfig's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new RedirectProjectConfig(package, commandService);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			//string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
			string message = "Opening the configuration file...";
			string title = "RedirectProjectConfig";

			CreateOrOpenConfig();

			//IDictionary<string, string> config = ReadConfig();

			// Show a message box to prove we were here
			VsShellUtilities.ShowMessageBox(
				this.package,
				message,
				title,
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}

		private static void CreateOrOpenConfig()
		{
			FileInfo fi = new FileInfo(_configPath);
			if (!fi.Exists)
			{
				CreateConfig(_configPath);
			}

			IDictionary<string, string> config = ReadConfig();
			MyForm form = new MyForm(config, "Configuration");
			DialogResult result = form.ShowDialog();

			if (result == DialogResult.OK)
			{
				string[] toFile =
				{
					"Usage: This a redirect config file - make sure the all the values are filled",
					"\n",
					RedirectRepositoryUrl + " = " + config[RedirectRepositoryUrl],
					RedirectDirectoryPath + " = " + config[RedirectDirectoryPath],
					RealRepositoryUrl + " = " + config[RealRepositoryUrl],
					RealRepositoryPath + " = " + config[RealRepositoryPath],
					Username + " = " + config[Username],
					Mail + " = " + config[Mail],
					TokenPath + " = " + config[TokenPath],
					"\n",
					"[Optional]",
					BranchName + " = " + config[BranchName],
					RemoteName + " = " + config[RemoteName],
					RefSpecs + " = " + config[RefSpecs]
				};
				//File.WriteAllLines(_configPath, toFile);
				
				File.WriteAllText(_configPath, string.Join("\n", toFile));
			}

			/*ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = _configPath,
				UseShellExecute = true
			};
			Process.Start(psi);*/
		}

		private static void CreateConfig(string path)
		{
			string[] toFile =
			{
				"Usage: This a redirect config file - make sure the all the values are filled",
				"\n",
				RedirectRepositoryUrl +" =  ",
				RedirectDirectoryPath + " = ",
				RealRepositoryUrl + " = ",
				RealRepositoryPath + " = ",
				Username + " = ",
				Mail + " = ",
				TokenPath + " = ",
				"\n",
				"[Optional]",
				BranchName + " = ",
				RemoteName + " = ",
				RefSpecs + " = "
			};

			File.WriteAllLines(path, toFile);
		}

		public static IDictionary<string, string> ReadConfig()
		{
			IDictionary<string, string> config = new Dictionary<string, string>();
			if (File.Exists(_configPath))
			{
				StreamReader file = new StreamReader(_configPath);
				string ln = file.ReadLine();
				while ((ln = file.ReadLine()) != null) 
				{
					if (string.IsNullOrEmpty(ln) || string.IsNullOrWhiteSpace(ln))
					{
						continue;
					}
					string[] stabs = ln.Split(' ');
					if (stabs.Length == 3)
					{
						config.Add(stabs[0], stabs[2]);
					}
				}
				file.Close();
			}

			return config;
		}

		public static string StartUtilitiesProcess(string args)
		{
			ProcessStartInfo psi = psiUtilities;
			psi.Arguments = args;
			Process proc = new Process() { StartInfo = psi };
			proc.Start();

			while (!proc.StandardOutput.EndOfStream || !proc.StandardError.EndOfStream)
			{
				string line = proc.StandardOutput.ReadToEnd();
				return string.IsNullOrEmpty(line) ? proc.StandardError.ReadToEnd() : line;
			}

			return null;
		}
	}
}
