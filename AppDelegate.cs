using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using AudioToolbox;
using Foundation;
using UIKit;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;
using CoreFoundation;

namespace SecurePDF
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		//this variable is used for when the user decides to change his currency
		public int selectedCurrency;

		//password delegate properties
		public int passwordChosenSelected;

		public int instructionsShared;

		//password  features
		public string textfieldPassword;
		public string textfieldPasswordDescription;

		//notes features
		public string textfieldNotes;
		public string textfieldNotesDescription;
		public int notesChosenSelected;

		public UITableView tableAccessPassword = new UITableView();

		public List<string> passwordReaderList = new List<string>() { };

		//bar button edit password 
		public UIBarButtonItem editPassword = new UIBarButtonItem();

		//PDF controller
		public PDFReader PDF = new PDFReader();
		public WordReader word = new WordReader();

		//Password controller 
		public PasswordNotes passwordControl = new PasswordNotes();

		public UINavigationItem navItem = new UINavigationItem();

		public List<string> newResults = new List<string>() { };
		public UITableView tableView = new UITableView();
		public int searchTableSelectPDF = 0;
		public int tablePDFSearch = 0;
		public NSIndexPath selectedIndexPDF;

		public UISearchBar notesSearch = new UISearchBar();

		public List<string> passwordList = new List<string>() { };

		//general documents
		public int searchTableSelectWord = 0;
		public int tableWordSearch = 0;
		public NSIndexPath selectedIndexWord;
		public List<string> newResultsWord = new List<string>() { };
		public List<string> resultsStringWord = new List<string>() { };
		public string wordString;
		public string searchedStringWord;


		//photos and videos
		public int searchTableSelectPhoto = 0;
		public int tablePhotoSearch = 0;
		public NSIndexPath selectedIndexPhoto;
		public List<string> newResultsPhoto = new List<string>() { };
		public List<string> resultsStringPhoto = new List<string>() { };
		public string photoString;
		public string searchedStringPhoto;
		public PhotosReader photoReader = new PhotosReader();

		public UITabBar tab = new UITabBar();

		//passwords
		public int searchTableSelectPassword = 0;
		public int tablePasswordSearch = 0;
		public NSIndexPath selectedIndexPassword;
		public List<string> newResultsPassword = new List<string>() { };
		public List<string> resultsStringPassword = new List<string>() { };
		public string passwordString;
		public string searchedStringPassword;

		//notes
		public int searchTableSelectNotes = 0;
		public int tableNotesSearch = 0;
		public NSIndexPath selectedIndexNotes;
		public List<string> newResultsNotes = new List<string>() { };
		public List<string> resultsStringNotes = new List<string>() { };
		public string notesString;
		public string searchedStringNotes;
		public NotesController notesControl = new NotesController();


		#region error variable 
		private NSError Error;
		#endregion

		public List<string> resultsStringPDF = new List<string>() { };

		public string searchedStringPDF;

		public List<string> dataList = new List<string>() { };

		public LibraryControllerTable library = new LibraryControllerTable();
		public WordController documentsWord = new WordController();
		public PhotosController photo = new PhotosController();
		public PasswordNotes passwordController = new PasswordNotes();

		public UINavigationBar navBar_2 = new UINavigationBar();

		public string[] directories;
		//password log in 
		public string logInTitle;
		public UIBarButtonItem logInConfirmed = new UIBarButtonItem();

		public string pdfString; 

		public UINavigationBar navBar = new UINavigationBar();
		public UITextField passwordLog = new UITextField();

		public override UIWindow Window { get; set; }

		public NavigationMaster navController = new NavigationMaster();
		public NavigationMaster navMaster = new NavigationMaster();

		public ContinousPasscodeController password = new ContinousPasscodeController();
		public FirstTimePasscodeController passwordFirstControl = new FirstTimePasscodeController();
		//expenses totals number, and description
		public List<double> expenseAmount = new List<double>() { };
		public List<string> nameExpenses = new List<string>() { };


		private void errorCamera(string title, string message)
		{
			UIAlertController errorController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			UIAlertAction confirmed = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (Action) =>
			{
				errorController.Dispose();
			});

			errorController.AddAction(confirmed);

			if (navController.PresentedViewController == null)
			{
				navController.PresentViewController(errorController, true, null);
			}
			else if (navController.PresentedViewController != null)
			{
				navController.PresentedViewController.DismissViewController(true, () =>
				{
					navController.PresentedViewController.Dispose();
					navController.PresentViewController(errorController, true, null);
				});
			}
		}

		private void SpeechText(string testedSpeech)
		{
			AVSpeechSynthesizer speech = new AVSpeechSynthesizer();

			AVSpeechUtterance speechUtterance = new AVSpeechUtterance(testedSpeech)
			{
				Rate = AVSpeechUtterance.MaximumSpeechRate / 2.0f,
				Voice = AVSpeechSynthesisVoice.FromLanguage("en-US"),
				Volume = 1.0f,
				PitchMultiplier = 1.0f
			};

			speech.SpeakUtterance(speechUtterance);
		}

		public override void FinishedLaunching(UIApplication application)
		{
			application.RegisterForRemoteNotifications();


			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					//OS is up to date
					var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert & UIUserNotificationType.Sound, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

					UILocalNotification batteryNotification = new UILocalNotification();

					NSDate.FromObject(UIDevice.BatteryLevelDidChangeNotification);

					batteryNotification.SoundName = UILocalNotification.DefaultSoundName;

					UIApplication.SharedApplication.ScheduleLocalNotification(batteryNotification);
					UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

					var memoryCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					var filePathPassword = System.IO.Path.Combine(memoryCache, "password.txt");

					try
					{
						if (filePathPassword == null)
						{
							throw new FileNotFoundException();
						}
						else {
							if (String.IsNullOrEmpty(File.ReadAllText(filePathPassword)) == true)
							{
								if (this.navMaster.VisibleViewController == this.passwordFirstControl)
								{
									this.Window.RootViewController = this.passwordFirstControl;
									this.navMaster.PushViewController(this.passwordFirstControl, true);
								}
								else {
									this.navMaster.PushViewController(this.passwordFirstControl, true);
								}
							}
							else {
								if (this.navMaster.ViewControllers.Contains(this.password) == true)
								{
									if (this.navMaster.VisibleViewController != this.password)
									{
										this.passwordLog.Text = "";
										this.passwordLog.ResignFirstResponder();
										this.navMaster.PopToViewController(this.password, true);
									}
									else {
										Console.WriteLine("Controller already exists");
									}
								}
								else {
									this.navMaster.PushViewController(this.password, true);
								}
							}
						}
					}
					catch (FileNotFoundException)
					{
						if (this.navMaster.VisibleViewController == this.passwordFirstControl)
						{
							this.Window.RootViewController = this.password;
							this.navMaster.PushViewController(this.passwordFirstControl, true);
						}
						else {
							this.navMaster.PushViewController(this.passwordFirstControl, true);
						}
					}
					//welcome the user when they launch the app for the first time on the device
					//add a key to the OS
					Console.WriteLine("Presented view controller" + this.navMaster.VisibleViewController);
				}
				else {
					//OS is out of date 
				}
			}

			else if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{

					var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert & UIUserNotificationType.Sound, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

					//local notifications
					UILocalNotification batteryNotification = new UILocalNotification();

					NSDate.FromObject(UIDevice.BatteryLevelDidChangeNotification);

					batteryNotification.SoundName = UILocalNotification.DefaultSoundName;

					UIApplication.SharedApplication.ScheduleLocalNotification(batteryNotification);
					UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

					var memoryCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					var filePathPassword = System.IO.Path.Combine(memoryCache, "password.txt");

					try
					{
						if (filePathPassword == null)
						{
							throw new FileNotFoundException();
						}
						else {
							if (String.IsNullOrEmpty(File.ReadAllText(filePathPassword)) == true)
							{
								if (this.navMaster.VisibleViewController == this.passwordFirstControl)
								{
									this.Window.RootViewController = this.passwordFirstControl;
									this.navMaster.PushViewController(this.passwordFirstControl, true);
								}
								else {
									this.navMaster.PushViewController(this.passwordFirstControl, true);
								}
							}
							else {
								if (this.navMaster.ViewControllers.Contains(this.password) == true)
								{
									if (this.navMaster.VisibleViewController != this.password)
									{
										this.passwordLog.Text = "";
										this.passwordLog.ResignFirstResponder();
										this.navMaster.PopToViewController(this.password, true);
									}
									else {
										Console.WriteLine("Controller already exists");
									}
								}
								else {
									this.navMaster.PushViewController(this.password, true);
								}
							}
						}
					}
					catch (FileNotFoundException)
					{
						if (this.navMaster.VisibleViewController == this.passwordFirstControl)
						{
							this.Window.RootViewController = this.password;
							this.navMaster.PushViewController(this.passwordFirstControl, true);
						}
						else {
							this.navMaster.PushViewController(this.passwordFirstControl, true);
						}
					}
					Console.WriteLine("Presented view controller" + this.navMaster.VisibleViewController);
				}
			}
		}

		public override bool WillFinishLaunching(UIApplication application, NSDictionary launchOptions)
		{
			application.RegisterForRemoteNotifications();

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					//OS is up to date
					var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert & UIUserNotificationType.Sound, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

					UILocalNotification batteryNotification = new UILocalNotification();

					NSDate.FromObject(UIDevice.BatteryLevelDidChangeNotification);

					batteryNotification.SoundName = UILocalNotification.DefaultSoundName;

					UIApplication.SharedApplication.ScheduleLocalNotification(batteryNotification);
					UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

					//welcome the user when they launch the app for the first time on the device
					//add a key to the OS

					return true;
				}
				else {
					//OS is out of date 
					return false;
				}
				return true;
			}

			else if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{


				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{

					var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert & UIUserNotificationType.Sound, null);
					UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);

					//local notifications
					UILocalNotification batteryNotification = new UILocalNotification();

					NSDate.FromObject(UIDevice.BatteryLevelDidChangeNotification);

					batteryNotification.SoundName = UILocalNotification.DefaultSoundName;

					UIApplication.SharedApplication.ScheduleLocalNotification(batteryNotification);
					UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

					Console.WriteLine("Current controller is: " + this.navMaster.VisibleViewController);
					return true;
				}
			}
			return true;
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			Console.WriteLine("Registration succeeded");
		}

		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			//place any remote notifications here. Including any advertising 
		}



		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			//local notification receieved
		}

		//register for remote notifications used as advertising servic 


		public override void ReceiveMemoryWarning(UIApplication application)
		{
			System.Console.WriteLine("Memory warning has been received");
		}

		public override void OnResignActivation(UIApplication application)
		{
			UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			VaultInstructionsController vaultInstructions = story.InstantiateViewController("VaultInstructionsController") as VaultInstructionsController;
			if (this.navMaster.VisibleViewController == vaultInstructions)
			{
				vaultInstructions.DismissViewController(true, null);
			}

			SystemSound soundLock = new SystemSound(1100);
			soundLock.PlaySystemSound();
			var memoryCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var filePathPassword = System.IO.Path.Combine(memoryCache, "password.txt");

			try
			{
				if (filePathPassword == null)
				{
					throw new FileNotFoundException();
				}
				else {
					if (String.IsNullOrEmpty(File.ReadAllText(filePathPassword)) == true)
					{
						if (this.navMaster.VisibleViewController != this.passwordFirstControl)
						{
							this.navMaster.PopToViewController(this.passwordFirstControl, true);
						}
						else {
							Console.WriteLine("Controller already exists");
						}
					}
					else {
						if (this.navMaster.ViewControllers.Contains(this.password) == true)
						{
							if (this.navMaster.VisibleViewController != this.password)
							{
								this.passwordLog.Text = "";
								this.passwordLog.ResignFirstResponder();
								this.navMaster.PopToViewController(this.password, true);
							}
							else {
								Console.WriteLine("Controller already exists");
							}
						}
						else {
							this.navMaster.PushViewController(this.password, true);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				if (this.navMaster.VisibleViewController != this.passwordFirstControl)
				{
					this.navMaster.PushViewController(this.passwordFirstControl, true);
				}
				else {
					Console.WriteLine("Controller already exists");
				}
			}
		}

		public override void DidEnterBackground(UIApplication application)
		{
			UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			VaultInstructionsController vaultInstructions = story.InstantiateViewController("VaultInstructionsController") as VaultInstructionsController;
			if (this.navMaster.VisibleViewController == vaultInstructions)
			{
				vaultInstructions.DismissViewController(true, null);
			}

			SystemSound soundLock = new SystemSound(1100);
			soundLock.PlaySystemSound();
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
			var memoryCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var filePathPassword = System.IO.Path.Combine(memoryCache, "password.txt");

			try
			{
				if (filePathPassword == null)
				{
					throw new FileNotFoundException();
				}
				else {
					if (String.IsNullOrEmpty(File.ReadAllText(filePathPassword)) == true)
					{
						if (this.navMaster.VisibleViewController != this.passwordFirstControl)
						{
							this.navMaster.PopToViewController(this.passwordFirstControl, true);
						}
						else {
							Console.WriteLine("Controller already exists");
						}
					}
					else {
						if (this.navMaster.ViewControllers.Contains(this.password) == true)
						{
							if (this.navMaster.VisibleViewController != this.password)
							{
								this.passwordLog.Text = "";
								this.passwordLog.ResignFirstResponder();
								this.navMaster.PopToViewController(this.password, true);
							}
							else {
								Console.WriteLine("Controller already exists");
							}
						}
						else {
							this.navMaster.PushViewController(this.password, true);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				if (this.navMaster.VisibleViewController != this.passwordFirstControl)
				{
					this.navMaster.PushViewController(this.passwordFirstControl, true);
				}
				else {
					Console.WriteLine("Controller already exists");
				}
			}
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.

			UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			VaultInstructionsController vaultInstructions = story.InstantiateViewController("VaultInstructionsController") as VaultInstructionsController;
			if (this.navMaster.VisibleViewController == vaultInstructions)
			{
				vaultInstructions.DismissViewController(true, null);
			}

			var memoryCache = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var filePathPassword = System.IO.Path.Combine(memoryCache, "password.txt");

			try
			{
				if (filePathPassword == null)
				{
					throw new FileNotFoundException();
				}
				else {
					if (String.IsNullOrEmpty(File.ReadAllText(filePathPassword)) == true)
					{
						if (this.navMaster.VisibleViewController != this.passwordFirstControl)
						{
							this.navMaster.PopToViewController(this.passwordFirstControl, true);
						}
						else {
							Console.WriteLine("Controller already exists");
						}
					}
					else {
						if (this.navMaster.ViewControllers.Contains(this.password) == true)
						{
							if (this.navMaster.VisibleViewController != this.password)
							{
								this.passwordLog.Text = "";
								this.passwordLog.ResignFirstResponder();
								this.navMaster.PopToViewController(this.password, true);
							}
							else {
								Console.WriteLine("Controller already exists");
							}
						}
						else {
							this.navMaster.PushViewController(this.password, true);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				if (this.navMaster.VisibleViewController != this.passwordFirstControl)
				{
					this.navMaster.PushViewController(this.passwordFirstControl, true);
				}
				else {
					Console.WriteLine("Controller already exists");
				}
			}
		}

		public override void WillTerminate(UIApplication application)
		{
		}
	}
}


