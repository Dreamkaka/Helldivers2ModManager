using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Helldivers2ModManager.Components;
using Helldivers2ModManager.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Helldivers2ModManager.ViewModels;

internal sealed partial class SettingsPageViewModel : PageViewModelBase
{
	public override string Title => "设置";

	public string GameDir
	{
		get => _settingsStore.GameDirectory;
		set
		{
			OnPropertyChanging();
			_settingsStore.GameDirectory = value;
			OnPropertyChanged();
		}
	}

	public string TempDir
	{
		get => _settingsStore.TempDirectory;
		set
		{
			OnPropertyChanging();
			_settingsStore.TempDirectory = value;
			OnPropertyChanged();
		}
	}

	public string StorageDir
	{
		get => _settingsStore.StorageDirectory;
		set
		{
			OnPropertyChanging();
			_settingsStore.StorageDirectory = value;
			OnPropertyChanged();

			_storageDirChanged = true;
			WeakReferenceMessenger.Default.Send(new MessageBoxInfoMessage()
			{
				Message = "配置目录已更改。管理器需要重新启动，一旦你点击\"ok\"，它将退出。"
			});
		}
	}

	public LogLevel LogLevel
	{
		get => _settingsStore.LogLevel;
		set
		{
			OnPropertyChanging();
			_settingsStore.LogLevel = value;
			OnPropertyChanged();
		}
	}

	public float Opacity
	{
		get => _settingsStore.Opacity;
		set
		{
			OnPropertyChanging();
			_settingsStore.Opacity = value;
			OnPropertyChanged();
		}
	}

	public ObservableCollection<string> SkipList => _settingsStore.SkipList;

	public bool CaseSensitiveSearch
	{
		get => _settingsStore.CaseSensitiveSearch;
		set
		{
			OnPropertyChanging();
			_settingsStore.CaseSensitiveSearch = value;
			OnPropertyChanged();
		}
	}

	private readonly ILogger<SettingsPageViewModel> _logger;
	private readonly NavigationStore _navStore;
	private readonly SettingsStore _settingsStore;
	private bool _storageDirChanged = false;
	[ObservableProperty]
	private int _selectedSkip = -1;

	public SettingsPageViewModel(ILogger<SettingsPageViewModel> logger, NavigationStore navStore, SettingsStore settingsStore)
	{
		_logger = logger;
		_navStore = navStore;
		_settingsStore = settingsStore;

		SkipList.CollectionChanged += SkipList_CollectionChanged;
	}

	private bool ValidateSettings()
	{
		if (string.IsNullOrEmpty(GameDir))
		{
			WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
			{
				Message = "游戏目录不能为空！！！"
			});
			return false;
		}

		if (string.IsNullOrEmpty(StorageDir))
		{
			WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
			{
				Message = "配置目录不能为空！！！"
			});
			return false;
		}

		if (string.IsNullOrEmpty(TempDir))
		{
			WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
			{
				Message = "临时目录不能为空！！！"
			});
			return false;
		}

		return true;
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(SelectedSkip))
			RemoveSkipCommand.NotifyCanExecuteChanged();

		base.OnPropertyChanged(e);
	}

	private void SkipList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	{
		RemoveSkipCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand]
	void Ok()
	{
		if (!ValidateSettings())
			return;

		_settingsStore.Save();

		if (_storageDirChanged)
			Application.Current.Shutdown();
		else
			_navStore.Navigate<DashboardPageViewModel>();
	}

	[RelayCommand]
	void Reset()
	{
		WeakReferenceMessenger.Default.Send(new MessageBoxConfirmMessage
		{
			Title = "重置？",
			Message = "你确定要重置你的设置吗？",
			Confirm = () =>
			{
				_settingsStore.Reset();
				OnPropertyChanged(nameof(GameDir));
				OnPropertyChanged(nameof(TempDir));
				OnPropertyChanged(nameof(StorageDir));
				OnPropertyChanged(nameof(LogLevel));
				OnPropertyChanged(nameof(Opacity));
			}
		});
	}

	[RelayCommand]
	void BrowseGame()
	{
		var dialog = new OpenFolderDialog
		{
			Multiselect = false,
			Title = "请选择你的hd2游戏文件夹..."
		};

		if (dialog.ShowDialog() ?? false)
		{
			var newDir = new DirectoryInfo(dialog.FolderName);

			if (newDir.Parent is DirectoryInfo { Name: "Helldivers 2" })
			{
				newDir = newDir.Parent;
			}

			if (newDir is not DirectoryInfo { Name: "Helldivers 2" })
			{
				WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
				{
					Message = "你选择的文件夹不在一个有效的文件目录中！！！"
				});
				return;
			}

			var subDirs = newDir.EnumerateDirectories();
			if (!subDirs.Any(static dir => dir.Name == "data"))
			{
				WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
				{
					Message = "你选择的文件夹中不包含一个名为\"data\"的文件夹！！！"
				});
				return;
			}
			if (!subDirs.Any(static dir => dir.Name == "tools"))
			{
				WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
				{
					Message = "你选择的文件夹中不包含一个名为\"tools\"的文件夹！！！"
				});
				return;
			}
			if (!subDirs.Any(static dir => dir.Name == "bin"))
			{
				WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
				{
					Message = "你选择的文件夹中不包含一个名为\"bin\"的文件夹！！！"
				});
				return;
			}

			GameDir = newDir.FullName;
		}
		else
		{
			WeakReferenceMessenger.Default.Send(new MessageBoxErrorMessage()
			{
				Message = "你选择的文件夹不是hd2的游戏文件夹！！！"
			});
		}
	}

	[RelayCommand]
	void BrowseStorage()
	{
		var dialog = new OpenFolderDialog
		{
			Multiselect = false,
			ValidateNames = true,
			Title = "请选择一个文件夹，这个文件夹将用于存储mod..."
		};

		if (dialog.ShowDialog() ?? false)
			StorageDir = dialog.FolderName;
	}

	[RelayCommand]
	void BrowseTemp()
	{
		var dialog = new OpenFolderDialog
		{
			Multiselect = false,
			ValidateNames = true,
			Title = "请选择一个文件夹，这个文件夹将用于存储临时文件..."
		};

		if (dialog.ShowDialog() ?? false)
			TempDir = dialog.FolderName;
	}

	[RelayCommand]
	void HardPurge()
	{
		_logger.LogInformation("Hard purging patch files");
		
		var path = Path.Combine(_settingsStore.StorageDirectory, "installed.txt");
		if (File.Exists(path))
			File.Delete(path);

		var dataDir = new DirectoryInfo(Path.Combine(_settingsStore.GameDirectory, "data"));
		
		var files = dataDir.EnumerateFiles("*.patch_*").ToArray();
		_logger.LogDebug("Found {} patch files", files.Length);

		foreach (var file in files)
		{
			_logger.LogTrace("Deleting \"{}\"", file.Name);
			file.Delete();
		}

		_logger.LogInformation("Hard purge complete");
	}

	[RelayCommand]
	void AddSkip()
	{
		WeakReferenceMessenger.Default.Send(new MessageBoxInputMessage
		{
			Title = "文件名？",
			Message = "请输入一个16字符的文件名，这个文件名将用于跳过patch 0。",
			MaxLength = 16,
			Confirm = (str) =>
			{
				if (str.Length == 16)
					SkipList.Add(str);
				else
					WeakReferenceMessenger.Default.Send(new MessageBoxInfoMessage
					{
						Message = "文件名只能为16个字符！！！"
					});
			}
		});
	}

	bool CanRemoveSkip()
	{
		return SelectedSkip != -1;
	}

	[RelayCommand(CanExecute = nameof(CanRemoveSkip))]
	void RemoveSkip()
	{
		SkipList.RemoveAt(SelectedSkip);
	}
}
