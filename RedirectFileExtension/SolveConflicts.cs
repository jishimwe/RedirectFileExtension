﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace RedirectFileExtension
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class SolveConflicts
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 4133;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("8255648a-a3bc-442f-9797-63a14abfa17c");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly AsyncPackage package;

		/// <summary>
		/// Initializes a new instance of the <see cref="SolveConflicts"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private SolveConflicts(AsyncPackage package, OleMenuCommandService commandService)
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
		public static SolveConflicts Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IAsyncServiceProvider ServiceProvider
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
			// Switch to the main thread - the call to AddCommand in SolveConflicts's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new SolveConflicts(package, commandService);
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
			string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
			string title = "Solve Conflicts";

			IDictionary<string, string> config = RedirectProjectConfig.ReadConfig();

			IDictionary<string, string> data = new Dictionary<string, string>()
			{
				{ RedirectProjectConfig.MergeOptions, "" }
			};

			MyForm form = new MyForm(data, "Merge", config);
			DialogResult result = form.ShowDialog();
			if(result == DialogResult.OK)
			{
				data = form.data;
				string args = "-merge " +
				              " -conf " + RedirectProjectConfig.GetConfigFilePath() +
				              " -mo " + data[RedirectProjectConfig.MergeOptions];

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
