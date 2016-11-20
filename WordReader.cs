// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

namespace SecurePDF
{
	public partial class WordReader : UIViewController
	{
		public WordReader (IntPtr handle) : base (handle)
		{
		}

		public WordReader() 
		{
		}

		public AppDelegate appDelegate
		{
			get
			{
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}


		UIWebView pdfWebReader = new UIWebView();
		UISearchBar searchForText = new UISearchBar();

		public override void ViewWillLayoutSubviews()
		{
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
			{
				this.pdfWebReader.Frame = new CGRect(0, 50, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - 50.0f);
			}
			else {
				this.pdfWebReader.Frame = new CGRect(0, 50, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - 50.0f);
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			if (this.NavigationItem.TitleView == this.searchForText)
			{
				if (this.searchForText.IsFirstResponder == true)
				{
					this.searchForText.ResignFirstResponder();
				}
				this.NavigationItem.Title = "Viewer";
			}
		}

		private void errorPrinting(string title, string message)
		{
			UIAlertController errorPrintController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			UIAlertAction onePage = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (UIAlertAction obj) =>
			{
				errorPrintController.Dispose();
			});


			errorPrintController.AddAction(onePage);

			if (this.PresentedViewController == null)
			{
				this.PresentViewController(errorPrintController, true, null);
			}
			else {
				this.PresentedViewController.DismissViewController(true, () =>
				{
					this.PresentedViewController.Dispose();
					this.PresentViewController(errorPrintController, true, null);
				});
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			this.appDelegate.tablePDFSearch = 0;
			UIBarButtonItem searchText = new UIBarButtonItem(UIBarButtonSystemItem.Search, (object sender, EventArgs e) =>
			{
				//searches for matching text inside pdf
				this.searchForText.Placeholder = "Search...";
				this.searchForText.SpellCheckingType = UITextSpellCheckingType.No;
				this.searchForText.SearchBarStyle = UISearchBarStyle.Prominent;
				this.searchForText.BarStyle = UIBarStyle.Default;
				this.searchForText.ShowsCancelButton = true;
				this.NavigationItem.TitleView = this.searchForText;

				this.searchForText.BecomeFirstResponder();
				this.searchForText.CancelButtonClicked += (object sender_2, EventArgs e_2) =>
				{
					//replaces the navigation item with the title text
					this.searchForText.ResignFirstResponder();
					this.NavigationItem.TitleView = null;
					this.NavigationItem.Title = "Viewer";
				};
			});

			UIBarButtonItem pageMode = new UIBarButtonItem("Page Mode", UIBarButtonItemStyle.Done, (object sender, EventArgs e) =>
			{
				//page count is adjusted 
				//creates a small table which adjusts the number of pages to render

				//an alert controller is best for this
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				{
					UIAlertController pageModeController = UIAlertController.Create("", "", UIAlertControllerStyle.ActionSheet);

					UIAlertAction onePage = UIAlertAction.Create("Single Page", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						this.pdfWebReader.PaginationMode = UIWebPaginationMode.TopToBottom;
						pageModeController.Dispose();
					});

					UIAlertAction continousPages = UIAlertAction.Create("Continuous", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						this.pdfWebReader.PaginationMode = UIWebPaginationMode.Unpaginated;
						pageModeController.Dispose();

					});

					UIAlertAction denied = UIAlertAction.Create("Never Mind", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
					{
						pageModeController.Dispose();
					});

					pageModeController.AddAction(onePage);
					pageModeController.AddAction(continousPages);
					pageModeController.AddAction(denied);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(pageModeController, true, null);
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(pageModeController, true, null);
						});
					}
				}
				else {
					UIAlertController pageModeController = UIAlertController.Create("", "", UIAlertControllerStyle.Alert);

					UIAlertAction onePage = UIAlertAction.Create("Single Page", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						this.pdfWebReader.PaginationMode = UIWebPaginationMode.TopToBottom;
						pageModeController.Dispose();
					});

					UIAlertAction continousPages = UIAlertAction.Create("Full Screen", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						this.pdfWebReader.PaginationMode = UIWebPaginationMode.Unpaginated;
						pageModeController.Dispose();

					});

					UIAlertAction denied = UIAlertAction.Create("Never Mind", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
					{
						pageModeController.Dispose();
					});

					pageModeController.AddAction(onePage);
					pageModeController.AddAction(continousPages);
					pageModeController.AddAction(denied);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(pageModeController, true, null);
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(pageModeController, true, null);
						});
					}
				}
			});
			UIBarButtonItem share = new UIBarButtonItem(UIBarButtonSystemItem.Action, (sender, e) =>
			{
				//introduces an action sheet. with the following options: 
				/*
				 * allows the user to share file (which creates an activity controller where the user can choose how to share the file)
				 * allows updating to the cloud 
				 * allows an option to open the document in another application of choice (UIDocumentInterationController)
				 * Print the document (In a nearby printer via AirPrint)

				*/
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				{
					UIAlertController shareModeController = UIAlertController.Create("", "", UIAlertControllerStyle.ActionSheet);

					UIAlertAction shareFile = UIAlertAction.Create("Share File", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
						var filePath = Path.Combine(directory, this.appDelegate.wordString);

						NSUrl pdfURL = NSUrl.FromFilename(filePath);
						NSData pdfData = NSData.FromUrl(pdfURL);

						NSObject[] dataToShare = new NSObject[] { pdfURL };
						UIActivityViewController activity = new UIActivityViewController(dataToShare, null);

						//UIActivityType[] activityTypes = new UIActivityType[] { UIActivityType.Print, UIActivityType.SaveToCameraRoll };


						if (this.PresentedViewController == null)
						{
							this.PresentViewController(activity, true, null);
						}
						else {
							this.PresentedViewController.DismissViewController(true, () =>
							{
								this.PresentedViewController.Dispose();
								this.PresentViewController(activity, true, null);
							});
						}
						shareModeController.Dispose();
					});

					UIAlertAction printDocument = UIAlertAction.Create("Print Document", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
						var filePath = Path.Combine(directory, this.appDelegate.wordString);

						NSUrl pdfURL = NSUrl.FromFilename(filePath);
						var printInformation = UIPrintInfo.PrintInfo;
						printInformation.OutputType = UIPrintInfoOutputType.General;
						printInformation.JobName = "Print Job";

						var print = UIPrintInteractionController.SharedPrintController;
						print.PrintInfo = printInformation;
						print.PrintFormatter = this.pdfWebReader.ViewPrintFormatter;
						print.PrintingItem = pdfURL;
						print.ShowsPageRange = true;

						if (UIPrintInteractionController.CanPrint(pdfURL) == true)
						{
							print.Present(true, (UIPrintInteractionController printInteractionController, bool completed, NSError error) =>
							{
								if (error != null)
								{
									print.Dismiss(true);
									errorPrinting("Print Error!", "Cannot print your document. Check if your printer is connected");
								}
								else {
									Console.WriteLine("Printer success");
								}
							});
						}
						else {
							errorPrinting("Print Error!", "Cannot print your document. Check if your printer is connected");
						}
						shareModeController.Dispose();
					});

					UIAlertAction denied = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
					{
						shareModeController.Dispose();
					});

					shareModeController.AddAction(shareFile);
					shareModeController.AddAction(printDocument);
					shareModeController.AddAction(denied);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(shareModeController, true, null);
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(shareModeController, true, null);
						});
					}
				}
				else {
					UIAlertController shareModeController = UIAlertController.Create("", "", UIAlertControllerStyle.Alert);

					UIAlertAction shareFile = UIAlertAction.Create("Share File", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
						var filePath = Path.Combine(directory, this.appDelegate.wordString);

						NSUrl pdfURL = NSUrl.FromFilename(filePath);
						NSData pdfData = NSData.FromUrl(pdfURL);

						NSObject[] dataToShare = new NSObject[] { pdfURL };
						UIActivityViewController activity = new UIActivityViewController(dataToShare, null);

						//UIActivityType[] activityTypes = new UIActivityType[] { UIActivityType.Print, UIActivityType.SaveToCameraRoll };


						if (this.PresentedViewController == null)
						{
							this.PresentViewController(activity, true, null);
						}
						else {
							this.PresentedViewController.DismissViewController(true, () =>
							{
								this.PresentedViewController.Dispose();
								this.PresentViewController(activity, true, null);
							});
						}
						shareModeController.Dispose();
					});

					UIAlertAction printDocument = UIAlertAction.Create("Print Document", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
						var filePath = Path.Combine(directory, this.appDelegate.wordString);

						NSUrl pdfURL = NSUrl.FromFilename(filePath);

						var printInformation = UIPrintInfo.PrintInfo;
						printInformation.OutputType = UIPrintInfoOutputType.General;
						printInformation.JobName = "Print Job";

						var print = UIPrintInteractionController.SharedPrintController;
						print.PrintInfo = printInformation;
						print.PrintFormatter = this.pdfWebReader.ViewPrintFormatter;
						print.PrintingItem = pdfURL;

						print.ShowsPageRange = true;
						print.Present(true, (UIPrintInteractionController printInteractionController, bool completed, NSError error) =>
						{
							if (error != null)
							{
								print.Dismiss(true);
								errorPrinting("Print Error!", "Cannot print your document. Check if your printer is connected");
							}
							else {
								Console.WriteLine("Printer success");
							}
						});

						shareModeController.Dispose();
					});

					UIAlertAction denied = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
					{
						shareModeController.Dispose();
					});

					shareModeController.AddAction(shareFile);
					shareModeController.AddAction(printDocument);
					shareModeController.AddAction(denied);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(shareModeController, true, null);
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(shareModeController, true, null);
						});
					}
				}
			});
			UIBarButtonItem writeComment = new UIBarButtonItem("Comment", UIBarButtonItemStyle.Done, (sender, e) =>
			{
				//adjusts the toolbar items with drawing items used for drawing items on the PDF view controller 
			});

			this.NavigationController.SetToolbarHidden(false, true);
			UIBarButtonItem space_1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);
			UIBarButtonItem space_2 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);

			this.NavigationController.Toolbar.Items = new UIBarButtonItem[] { searchText, space_1, space_2, share };

		}

		public override void ViewDidLoad()
		{
			this.searchForText.SearchButtonClicked += (object sender, EventArgs e) =>
			{
				this.searchForText.ResignFirstResponder();
			};
			this.appDelegate.word = this;
			this.NavigationItem.Title = "Viewer";

			this.pdfWebReader.Frame = new CGRect(0, 50, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - 50.0f);


			var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var filePath = Path.Combine(directory, appDelegate.wordString);

			NSUrl urlFile = NSUrl.FromFilename(filePath);

			NSUrlRequest pdfToLoad = new NSUrlRequest(urlFile, NSUrlRequestCachePolicy.ReloadIgnoringLocalAndRemoteCacheData, 0.1f);

			this.pdfWebReader.LoadRequest(pdfToLoad);
			this.pdfWebReader.ScalesPageToFit = true;
			this.pdfWebReader.DataDetectorTypes = UIDataDetectorType.All;

			this.View.AddSubview(this.pdfWebReader);
		}
	}
}
