// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections;
using System.Collections.Generic; 
using System.IO;
using System.Linq;

using AudioToolbox;
using Foundation;
using UIKit;
using CoreGraphics;


namespace SecurePDF
{
	public partial class PasswordNotes : UITableViewController
	{
		public PasswordNotes (IntPtr handle) : base (handle)
		{
		}

		public PasswordNotes() {}

		public AppDelegate applicationDelegate
		{
			get
			{
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}

		//search functions
		UISearchController searchController = new UISearchController();
		UISearchBar search = new UISearchBar();

		UIView emptyPDF = new UIView();
		UIImageView emptyImage = new UIImageView();
		UILabel description = new UILabel();
		UILabel description_2 = new UILabel();
		UILabel titleDescription = new UILabel();

		List<int> selectedCellsToRemove = new List<int>() { };
		List<string> passwordList = new List<string>() { };
		List<string> passwordCreation = new List<string>() { };

		//nav button items
		UIBarButtonItem edit = new UIBarButtonItem();

		//toolbar items 
		UIBarButtonItem addNewPassword = new UIBarButtonItem(); 
		UIBarButtonItem bin = new UIBarButtonItem();

		public void filteredContent(string searchedText)
		{
			this.applicationDelegate.newResultsPassword = this.passwordList.Where((arg) => arg.ToLower().Contains(searchedText.ToLower()) || arg.ToUpper().Contains(searchedText.ToUpper())).ToList();

			this.TableView.ReloadData();

			this.applicationDelegate.tableView.ReloadData();
		}

		public override void ViewWillLayoutSubviews()
		{
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
			{
				this.emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
					
				this.emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.Y - 200.0f, 200, 200);

				this.description.Frame = new CGRect(this.View.Center.X - 315.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
				this.description_2.Frame = new CGRect(this.View.Center.X - 155.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);

				this.titleDescription.Frame = new CGRect(this.View.Center.X - 100.0f, this.View.Center.Y - 180.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
			}
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
			{
				emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

				emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.X - 95.0f, 200, 200);

				this.description.Frame = new CGRect(this.View.Center.X - 190.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
				this.description_2.Frame = new CGRect(this.View.Center.X - 95.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);
				this.titleDescription.Frame = new CGRect(this.View.Center.X - 100.0f, this.View.Center.Y - 240.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
			}
		}


		public override void ViewDidAppear(bool animated)
		{
			Console.WriteLine("View appears");
			this.TableView.ReloadData();
			this.applicationDelegate.passwordChosenSelected = 0;

			if (this.applicationDelegate.tablePasswordSearch == 1)
			{
				this.TableView.ScrollToRow(this.applicationDelegate.selectedIndexPassword, UITableViewScrollPosition.Top, true);
				this.TableView.SelectRow(this.applicationDelegate.selectedIndexPassword, true, UITableViewScrollPosition.Top);
			}
		}

		public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool ShouldHighlightRow(UITableView tableView, NSIndexPath rowIndexPath)
		{
			return true;
		}
		public override void ViewDidLoad()
		{
			this.searchController = new UISearchController(new resultsControllerPassword());
			this.searchController.SearchResultsUpdater = new searchUpdatorPassword(this);
			this.searchController.DimsBackgroundDuringPresentation = true;
			this.searchController.HidesNavigationBarDuringPresentation = true;

			this.search = this.searchController.SearchBar;
			this.EdgesForExtendedLayout = UIRectEdge.None;

			this.search.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width - 5.0f, 50.0f);
			this.search.SpellCheckingType = UITextSpellCheckingType.No;
			this.search.BarStyle = UIBarStyle.Default;
			this.search.SearchBarStyle = UISearchBarStyle.Prominent;
			this.search.Placeholder = "Search...";

			this.search.CancelButtonClicked += (object sender, EventArgs e) =>
			{
				this.search.ResignFirstResponder();
			};
			this.search.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) =>
			{
				SystemSound keyboardClick = new SystemSound(1105);
				keyboardClick.PlaySystemSound();
			};

			this.search.SearchButtonClicked += (object sender, EventArgs e) =>
			{
				search.ResignFirstResponder();
			};

			this.TableView.TableHeaderView = search;

			this.applicationDelegate.resultsStringPassword = this.passwordList;
			this.TableView.AllowsMultipleSelectionDuringEditing = true;
			this.TableView.AllowsSelectionDuringEditing = true;
			this.TableView.AllowsSelection = true;
			this.TableView.AllowsMultipleSelection = true;

			this.applicationDelegate.passwordControl = this;
			this.applicationDelegate.tableAccessPassword = this.TableView;
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var passwordDirectory = Path.Combine(documents, "Passwords");
			Directory.CreateDirectory(passwordDirectory);

			var directory = Directory.GetFiles(passwordDirectory, "*.txt");

			var fileName = Path.Combine(documents, "*.txt");

			this.applicationDelegate.directories = directory;


			NSError error = new NSError();
			try
			{
				if (NSFileManager.DefaultManager.GetDirectoryContent(documents, out error).Length == 0)
				{
					throw new FileNotFoundException();
				}
				else {
					for (int i = 0; i <= directory.Length - 1; i++)
					{

						try
						{
							if (directory[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								if (this.passwordList.Contains(directory[i]) == true)
								{
									Console.WriteLine("Value already exists");
								}
								else {
									this.passwordList.Add(Path.GetFileName(directory[i]));
									this.passwordCreation.Add(Convert.ToString(Directory.GetCreationTime(directory[i])));
								}
							}
						}
						catch (ArgumentOutOfRangeException)
						{
							Console.WriteLine("File does not exist");
						}

					}
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Cannot find the specified pdf file in the library");
			}

			this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
			this.EdgesForExtendedLayout = UIRectEdge.None;

			this.NavigationItem.Title = "My Passwords";

			UIBarButtonItem cancelEditing = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) =>
			{
				this.selectedCellsToRemove.Clear();
				this.TableView.SetEditing(false, true);
				this.NavigationItem.SetRightBarButtonItem(this.applicationDelegate.editPassword, true);
			});

			//nav bar items
			this.edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (object sender, EventArgs e) =>
			{
				//pushes the password reader controller which allows the user to enter 
				var documents_2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var passwordDirectory_2 = Path.Combine(documents, "Passwords");

				NSError error_2 = new NSError();
				if (NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length == 0)
				{
					UIAlertController emptyPasswords = UIAlertController.Create("Nothing to delete", "You have no passwords listed here to delete", UIAlertControllerStyle.Alert);

					UIAlertAction confirmed = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						emptyPasswords.Dispose();
					});

					emptyPasswords.AddAction(confirmed);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(emptyPasswords, true, () =>
						{
							SystemSound soundError = new SystemSound(4095);
							soundError.PlaySystemSound();
						});
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(emptyPasswords, true, () =>
							{
								SystemSound soundError = new SystemSound(4095);
								soundError.PlaySystemSound();
							});
						});
					}
				}
				else {
					this.TableView.SetEditing(true, true);
					this.NavigationItem.SetRightBarButtonItem(cancelEditing, true);
				}
			});

			this.applicationDelegate.editPassword = this.edit;



			//toolbar items
			UIBarButtonItem space_1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, null);

			this.addNewPassword = new UIBarButtonItem("\ud83d\udcdd", UIBarButtonItemStyle.Plain, (sender, e) =>
			{
				UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
				//pushes the password reader controller which allows the user to enter 
				PasswordReader password = story.InstantiateViewController("PasswordReader") as PasswordReader;

				this.NavigationController.PushViewController(password, true);
			});

			this.bin.TintColor = UIColor.Red;

			this.bin = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, e) =>
			{
					//pushes the password reader controller which allows the user to enter 
					var documents_2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					var passwordDirectory_2 = Path.Combine(documents, "Passwords");

					NSError error_2 = new NSError();
				if (NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length == 0)
				{
					UIAlertController emptyPasswords = UIAlertController.Create("Nothing to delete", "You have no passwords listed here to delete", UIAlertControllerStyle.Alert);

					UIAlertAction confirmed = UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						emptyPasswords.Dispose();
					});

					emptyPasswords.AddAction(confirmed);

					if (this.PresentedViewController == null)
					{
						this.PresentViewController(emptyPasswords, true, () =>
						{
							SystemSound soundError = new SystemSound(4095);
							soundError.PlaySystemSound();
						});
					}
					else {
						this.PresentedViewController.DismissViewController(true, () =>
						{
							this.PresentedViewController.Dispose();
							this.PresentViewController(emptyPasswords, true, () =>
							{
								SystemSound soundError = new SystemSound(4095);
								soundError.PlaySystemSound();
							});
						});
					}
				}

				//files do exist in the directory
				else {
					if (this.TableView.Editing == false)
					{
						UIAlertController emptyPasswords = UIAlertController.Create("Clear passwords?", "Are you sure you want to clear the whole list of your passwords?", UIAlertControllerStyle.Alert);

						UIAlertAction confirmed = UIAlertAction.Create("Yes", UIAlertActionStyle.Destructive, (UIAlertAction obj) =>
						{
							//delete list for the table 
							//delete files contained inside the directory 
							this.passwordList.Clear();
							this.passwordCreation.Clear();

							Console.WriteLine("Number of files: " + NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length);

							for (int j = 0; j <= NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length - 1; j++)
							{
								Console.WriteLine("Contents: " + Directory.GetFiles(passwordDirectory_2)[j]);
								File.Delete(Directory.GetFiles(passwordDirectory_2)[j]);
							}

							try
							{
								if (String.IsNullOrEmpty(Directory.GetFiles(passwordDirectory_2)[NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length - 1]) == true)
								{
									throw new IndexOutOfRangeException();
								}
								else {
									File.Delete(Directory.GetFiles(passwordDirectory_2)[NSFileManager.DefaultManager.GetDirectoryContent(passwordDirectory_2, out error_2).Length - 1]);
								}
							}
							catch (IndexOutOfRangeException)
							{
								Console.WriteLine("File does not exist inside this directory");
							}
							this.NavigationController.PopViewController(true);
							emptyPasswords.Dispose();
						});

						UIAlertAction denied = UIAlertAction.Create("No", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
						{
							emptyPasswords.Dispose();
						});

						emptyPasswords.AddAction(confirmed);
						emptyPasswords.AddAction(denied);

						if (this.PresentedViewController == null)
						{
							this.PresentViewController(emptyPasswords, true, () =>
							{
								SystemSound soundError = new SystemSound(4095);
								soundError.PlaySystemSound();
							});
						}
						else {
							this.PresentedViewController.DismissViewController(true, () =>
							{
								this.PresentedViewController.Dispose();
								this.PresentViewController(emptyPasswords, true, () =>
								{
									SystemSound soundError = new SystemSound(4095);
									soundError.PlaySystemSound();
								});
							});
						}
					}
					//table view is in editing mode
					else {
						//bar button item will clear only the values that match the index chosen
						for (int i = 0; i <= this.selectedCellsToRemove.Count - 1; i++) {
							this.passwordList.RemoveAt(i);
							this.passwordCreation.RemoveAt(i);

							Console.WriteLine("Contents: " + Directory.GetFiles(passwordDirectory_2)[i]);

							File.Delete(Directory.GetFiles(passwordDirectory_2)[i]);
							this.NavigationController.PopViewController(true);
						}
					}
				}
			});
	
			this.NavigationController.SetToolbarHidden(false, true);
			this.NavigationController.Toolbar.BarTintColor = UIColor.White;
			this.NavigationController.Toolbar.BackgroundColor = UIColor.White;
			this.NavigationController.Toolbar.TintColor = UIColor.Blue;

			this.SetToolbarItems(new UIBarButtonItem[] { this.bin, space_1, space_1, this.addNewPassword }, true);

			Console.WriteLine("Toolbar items: " + this.NavigationController.Toolbar.Items.Length);


			Console.WriteLine("Is Toolbar hidden: " + this.NavigationController.Toolbar.Hidden);

			this.NavigationItem.SetRightBarButtonItem(this.edit, false);

			if (this.passwordList.Count == 0)
			{
				emptyPDF = new UIView();

				emptyImage = new UIImageView();

				this.description = new UILabel();
				this.description.Text = "This page contains no passwords. To get started adding";
				this.description.Font = UIFont.FromName("Georgia-Italic", 26.0f);
				this.description.AdjustsFontSizeToFitWidth = true;

				this.description_2 = new UILabel();
				this.description_2.Text = "your own passwords click \ud83d\udcdd";
				this.description_2.Font = UIFont.FromName("Georgia-Italic", 26.0f);
				this.description_2.AdjustsFontSizeToFitWidth = true;


				this.titleDescription = new UILabel();
				this.titleDescription.Text = "No Passwords";
				this.titleDescription.Font = UIFont.SystemFontOfSize(26.0f);
				this.titleDescription.Font = UIFont.FromName("AmericanTypewriter-Bold", 26.0f);

				if (UIApplication.SharedApplication.StatusBarHidden == true) 
				{
					emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

					emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.Y - 200.0f, 200, 200);

					this.description.Frame = new CGRect(this.View.Center.X - 315.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
					this.description_2.Frame = new CGRect(this.View.Center.X - 155.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);
					this.titleDescription.Frame = new CGRect(this.View.Center.X - 100.0f, this.View.Center.Y - 180.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
				}
				else
				{
					emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

					emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.X - 95.0f, 200, 200);

					this.description.Frame = new CGRect(this.View.Center.X - 190.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
					this.description_2.Frame = new CGRect(this.View.Center.X - 95.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);
					this.titleDescription.Frame = new CGRect(this.View.Center.X - 100.0f, this.View.Center.Y - 240.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
				}

				emptyImage.Image = new UIImage("EmptyScreen.png");

				emptyPDF.AddSubview(emptyImage);
				emptyPDF.AddSubview(this.description);
				emptyPDF.AddSubview(this.description_2);
				emptyPDF.AddSubview(this.titleDescription);

				this.TableView.SeparatorColor = UIColor.GroupTableViewBackgroundColor;
				this.TableView.TintColor = UIColor.GroupTableViewBackgroundColor;
				this.TableView.TableHeaderView = null;

				this.Add(emptyPDF);
				this.View.BringSubviewToFront(emptyPDF);
			}
			else {
				this.TableView.SeparatorColor = UIColor.LightGray;
				this.TableView.TintColor = UIColor.LightGray;

				//this.TableView.TableHeaderView = this.search;
				emptyPDF = new UIView();

				emptyImage = new UIImageView();

				emptyPDF.RemoveFromSuperview();
				emptyImage.RemoveFromSuperview();
			}
			this.emptyPDF.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return this.passwordList.Count; 
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "");

			cell.TextLabel.Text = this.passwordList[indexPath.Row];

			cell.DetailTextLabel.Text = this.passwordCreation[indexPath.Row];
			cell.DetailTextLabel.TextColor = UIColor.LightGray;

			cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;

			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			return cell;
		}

		public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
		{
			if(this.TableView.Editing == true) {
				int index = this.selectedCellsToRemove.IndexOf(indexPath.Row);
				this.selectedCellsToRemove.RemoveAt(index);
				Console.WriteLine("Remove cells list count is: " + this.selectedCellsToRemove.Count);
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if(this.TableView.Editing == true) {
				this.selectedCellsToRemove.Add(indexPath.Row);
				Console.WriteLine("Remove cells list count is: " + this.selectedCellsToRemove.Count);
			}
			else {
				this.applicationDelegate.passwordChosenSelected = 1; 

				var documents_2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var passwordDirectory = Path.Combine(documents_2, "Passwords");
				var fileName_2 = Path.Combine(passwordDirectory, tableView.CellAt(indexPath).TextLabel.Text);

				this.applicationDelegate.textfieldPasswordDescription = tableView.CellAt(indexPath).TextLabel.Text;
				this.applicationDelegate.textfieldPassword = File.ReadAllText(fileName_2);

				UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
				//pushes the password reader controller which allows the user to enter 
				PasswordReader password = story.InstantiateViewController("PasswordReader") as PasswordReader;

				this.NavigationController.PushViewController(password, true);
			}
		}

		public override void RowHighlighted(UITableView tableView, NSIndexPath rowIndexPath)
		{
			if (tableView.Editing == true)
			{
				tableView.CellAt(rowIndexPath).SelectionStyle = UITableViewCellSelectionStyle.Default;
			}
			else {
				tableView.CellAt(rowIndexPath).SelectionStyle = UITableViewCellSelectionStyle.Default;
			}
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete;
		}
	}

	public class searchUpdatorPassword : UISearchResultsUpdating
	{
		PasswordNotes search = new PasswordNotes();

		public searchUpdatorPassword(PasswordNotes searchValue)
		{
			search = searchValue;
		}

		public override void UpdateSearchResultsForSearchController(UISearchController searchController)
		{
			search.filteredContent(searchController.SearchBar.Text);
		}
	}

	public class resultsControllerPassword : UITableViewController
	{

		public AppDelegate appDelegate
		{
			get
			{
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}

		//controller used to display results
		UISearchController search = new UISearchController();
		List<string> filteredResults = new List<string>() { "" };

		public resultsControllerPassword() { }

		public resultsControllerPassword(UISearchController searchController)
		{
			search = searchController;
		}

		public resultsControllerPassword(UISearchController searchController, List<string> filteredResultsRef)
		{
			search = searchController;
			filteredResults = filteredResultsRef;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return this.appDelegate.newResultsPassword.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cellSearch = new UITableViewCell(UITableViewCellStyle.Value1, "SearchCell");

			if (cellSearch == null)
			{
				cellSearch = new UITableViewCell(UITableViewCellStyle.Value1, "SearchCell");
			}

			cellSearch.TextLabel.Text = this.appDelegate.newResultsPassword[indexPath.Row];
			cellSearch.TextLabel.TextColor = UIColor.Black;

			cellSearch.DetailTextLabel.Text = ">";
			cellSearch.DetailTextLabel.TextColor = UIColor.LightGray;

			return cellSearch;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			//pushes the web view controller that loads the PDF 
			this.appDelegate.tablePasswordSearch = 1;
			tableView.DeselectRow(indexPath, true);
			this.DismissViewController(true, () =>
			{
				this.appDelegate.tablePasswordSearch = 1;
				int indexRow = this.appDelegate.resultsStringPassword.IndexOf(tableView.CellAt(indexPath).TextLabel.Text);
				Console.WriteLine("Index to use: " + indexRow);
				this.appDelegate.selectedIndexPassword = NSIndexPath.FromRowSection(indexRow, 0);

				//search cell has been selected
				this.appDelegate.searchedStringPassword = tableView.CellAt(indexPath).TextLabel.Text;
				this.appDelegate.passwordControl.ViewDidAppear(true);
			});

		}

		public override void ViewDidAppear(bool animated)
		{
			this.TableView.Frame = new CGRect(0, -40, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

			this.EdgesForExtendedLayout = UIRectEdge.None;

			this.TableView.ContentMode = UIViewContentMode.Top;

			this.appDelegate.tableView = this.TableView;

			this.TableView.ReloadData();

		}

		public override void ViewDidLoad()
		{
			this.EdgesForExtendedLayout = UIRectEdge.Top;
			this.TableView.ContentMode = UIViewContentMode.Top;
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			if (evt.Type == UIEventType.Touches)
			{
				//Console.WriteLine("Table view count: " + this.appDelegate.newResults.Count);
			}
		}
	}
}