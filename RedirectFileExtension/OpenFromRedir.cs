using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace RedirectFileExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class OpenFromRedir
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("8255648a-a3bc-442f-9797-63a14abfa17c");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenFromRedir"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private OpenFromRedir(AsyncPackage package, OleMenuCommandService commandService)
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
		public static OpenFromRedir Instance
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
			// Switch to the main thread - the call to AddCommand in OpenFromRedir's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new OpenFromRedir(package, commandService);
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
			string message = "Could not open file from redirect";
			string title = "Open From Redirect";

			IDictionary<string, string> config = RedirectProjectConfig.ReadConfig();

			string args = "-open " +
			              "-r " + config["RedirectDirectoryPath"] + @"\app\src\main\AndroidManifest.xml.redir " +
			              "-d " + config[RedirectProjectConfig.RealRepositoryPath] + " " +
			              "-b " + config[RedirectProjectConfig.BranchName];

			IDictionary<string, string> data = new Dictionary<string, string>()
			{
				{ RedirectProjectConfig.RealRepositoryPath, config[RedirectProjectConfig.RealRepositoryPath]},
				{ RedirectProjectConfig.BranchName, config[RedirectProjectConfig.BranchName]},
				{ RedirectProjectConfig.RedirectFile, "" }
			};

			MyForm form = new MyForm(data, "Open File from Redirect");
			DialogResult result = form.ShowDialog();

			if (result == DialogResult.OK)
			{
				data = form.data;
				args = "-open" +
					" -r " + data[RedirectProjectConfig.RedirectFile] + 
					" -d " + data[RedirectProjectConfig.RealRepositoryPath] + 
					" -b " + data[RedirectProjectConfig.BranchName];

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
