using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using System.Windows.Forms;

namespace RedirectFileExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class AddFile
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 4135;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("8255648a-a3bc-442f-9797-63a14abfa17c");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="AddFile"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private AddFile(AsyncPackage package, OleMenuCommandService commandService)
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
		public static AddFile Instance
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
			// Switch to the main thread - the call to AddCommand in AddFile's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new AddFile(package, commandService);
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
			string message = "Failed to add file";
			string title = "Add File";

			IDictionary<string, string> config = RedirectProjectConfig.ReadConfig();

			string args = "-add";

			IDictionary<string, string> data = new Dictionary<string, string>()
			{
				{ RedirectProjectConfig.RealRepositoryPath, config[RedirectProjectConfig.RealRepositoryPath] },
				{ RedirectProjectConfig.RedirectDirectoryPath, config[RedirectProjectConfig.RedirectDirectoryPath] },
				{ RedirectProjectConfig.Username, config[RedirectProjectConfig.Username] },
				{ RedirectProjectConfig.Mail, config[RedirectProjectConfig.Mail] },
				{ RedirectProjectConfig.TokenPath, config[RedirectProjectConfig.TokenPath] },
				{ RedirectProjectConfig.Filepath, "" }
			};

			MyForm form = new MyForm(data, "Add File");
			DialogResult result = form.ShowDialog();

			if(result == DialogResult.OK)
			{
				data = form.data;
				args = "-add" +
					" -d " + data[RedirectProjectConfig.RealRepositoryPath] +
					" -f " + data[RedirectProjectConfig.Filepath] +
					" -r " + data[RedirectProjectConfig.RedirectDirectoryPath] +
					" -u " + data[RedirectProjectConfig.Username] +
					" -e " + data[RedirectProjectConfig.Mail] +
					" -t " + data[RedirectProjectConfig.TokenPath];

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
