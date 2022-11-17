using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace RedirectFileExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class Commit
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 4129;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("8255648a-a3bc-442f-9797-63a14abfa17c");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="Commit"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private Commit(AsyncPackage package, OleMenuCommandService commandService)
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
		public static Commit Instance
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
			// Switch to the main thread - the call to AddCommand in Commit's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new Commit(package, commandService);
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
			string message = "Commit Failed . . .";
			string title = "Commit";

			IDictionary<string, string> config = RedirectProjectConfig.ReadConfig();

			string args = "-commit" +
			              @" -f app\src\main\AndroidManifest.xml" +
			              " -d " + config[RedirectProjectConfig.RealRepositoryPath] +
			              " -m \"Visual Studio extensions commit\"" +
			              " -u " + config[RedirectProjectConfig.Username ] +
			              " -e " + config[RedirectProjectConfig.Mail];
			IDictionary<string, string> data = new Dictionary<string, string>()
			{
				{ RedirectProjectConfig.RealRepositoryPath, config[RedirectProjectConfig.RealRepositoryPath] },
				{ RedirectProjectConfig.Username, config[RedirectProjectConfig.Username] },
				{ RedirectProjectConfig.Mail, config[RedirectProjectConfig.Mail] },
				{ RedirectProjectConfig.Message, ""},
				{ RedirectProjectConfig.Filepath, ""}
			};

			MyForm form = new MyForm(data, "Commit");
			DialogResult result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				data = form.data;
				args = "-commit" +
				" -f " + data[RedirectProjectConfig.Filepath] +
				" -d " + data[RedirectProjectConfig.RealRepositoryPath] +
				" -m " + data[RedirectProjectConfig.Message] +
				" -u " + data[RedirectProjectConfig.Username] +
				" -e " + data[RedirectProjectConfig.Mail];

				message = RedirectProjectConfig.StartUtilitiesProcess(args) ?? message;
			}

			// Show a message box to prove we were here
			VsShellUtilities.ShowMessageBox(
				this.package,
				message, 
				title, 
				OLEMSGICON.OLEMSGICON_INFO, 
				OLEMSGBUTTON.OLEMSGBUTTON_OK, 
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}
	}
}
