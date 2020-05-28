using Avalonia.Diagnostics.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using WalletWasabi.Gui.ViewModels;
using System.IO;
using ReactiveUI;
using System.Reactive;
using WalletWasabi.Helpers;
using WalletWasabi.Logging;
using System.Reactive.Linq;
using WalletWasabi.WebClients.Wasabi;
using System.Reactive.Disposables;
using Splat;
using WalletWasabi.Models;
using WalletWasabi.Services;

namespace WalletWasabi.Gui.Tabs
{
	internal class AboutViewModel : WasabiDocumentTabViewModel
	{
		private string _currentBackendMajorVersion;

		public AboutViewModel() : base("About")
		{
			var global = Locator.Current.GetService<Global>();
			var hostedServices = global.HostedServices;

			UpdateChecker = hostedServices.FirstOrDefault<UpdateChecker>();

			CurrentBackendMajorVersion = UpdateChecker.UpdateStatus.CurrentBackendApiVersion;

			OpenBrowserCommand = ReactiveCommand.CreateFromTask<string>(IoHelpers.OpenBrowserAsync);

			OpenBrowserCommand.ThrownExceptions
				.ObserveOn(RxApp.TaskpoolScheduler)
				.Subscribe(ex => Logger.LogError(ex));
		}

		public ReactiveCommand<string, Unit> OpenBrowserCommand { get; }
		private UpdateChecker UpdateChecker { get; }
		public Version ClientVersion => Constants.ClientVersion;
		public string BackendCompatibleVersions => Constants.ClientSupportBackendVersionText;

		public string CurrentBackendMajorVersion
		{
			get => _currentBackendMajorVersion;
			set => this.RaiseAndSetIfChanged(ref _currentBackendMajorVersion, value);
		}

		public Version BitcoinCoreVersion => Constants.BitcoinCoreVersion;
		public Version HwiVersion => Constants.HwiVersion;

		public string ClearnetLink => "https://wasabiwallet.io/";

		public string TorLink => "http://wasabiukrxmkdgve5kynjztuovbg43uxcbcxn6y2okcrsg7gb6jdmbad.onion";

		public string SourceCodeLink => "https://github.com/zkSNACKs/WalletWasabi/";

		public string StatusPageLink => "https://stats.uptimerobot.com/YQqGyUL8A7";

		public string CustomerSupportLink => "https://www.reddit.com/r/WasabiWallet/";

		public string BugReportLink => "https://github.com/zkSNACKs/WalletWasabi/issues/";

		public string FAQLink => "https://docs.wasabiwallet.io/FAQ/";

		public string DocsLink => "https://docs.wasabiwallet.io/";

		public override void OnOpen(CompositeDisposable disposables)
		{
			base.OnOpen(disposables);

			Observable.FromEventPattern<UpdateStatus>(UpdateChecker, nameof(UpdateChecker.UpdateStatusChanged))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(e => CurrentBackendMajorVersion = e.EventArgs.CurrentBackendApiVersion)
				.DisposeWith(disposables);
		}

		public override bool OnClose()
		{
			return base.OnClose();
		}
	}
}
