
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
	public partial class PhotosController : UITableViewController
	{
		public PhotosController (IntPtr handle) : base (handle)
		{
		}

		public PhotosController(){}

		public List<string> pdfList = new List<string>() { };
		public List<string> creationDate = new List<string>() { };
		public List<string> size = new List<string>() { };
		List<int> selectedCellsToRemove = new List<int>() { };
		List<int> corruptIndex = new List<int>() { };



		//empty photos instructions
		UIView emptyPDF = new UIView();
		UIImageView emptyImage = new UIImageView();
		UILabel description = new UILabel();
		UILabel description_2 = new UILabel();
		UILabel titleDescription = new UILabel();

		public List<UIImage> thumbnailImage = new List<UIImage>() { };
		UISearchController searchController = new UISearchController();
		UISearchBar search = new UISearchBar();
		public PDFReader pdfRead = new PDFReader();
		public AppDelegate appDelegate
		{
			get
			{
				return (AppDelegate)UIApplication.SharedApplication.Delegate;
			}
		}

		public override void ViewWillLayoutSubviews()
		{
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
			{
				this.emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

				this.emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.Y - 200.0f, 200, 200);

				this.description.Frame = new CGRect(this.View.Center.X - 315.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
				this.description_2.Frame = new CGRect(this.View.Center.X - 155.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);

				this.titleDescription.Frame = new CGRect(this.View.Center.X - 70.0f, this.View.Center.Y - 180.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
			}
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
			{
				emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

				emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.X - 95.0f, 200, 200);

				this.description.Frame = new CGRect(this.View.Center.X - 190.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
				this.description_2.Frame = new CGRect(this.View.Center.X - 95.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);
				this.titleDescription.Frame = new CGRect(this.View.Center.X - 70.0f, this.View.Center.Y - 240.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
			}
		}


		public override void ViewDidAppear(bool animated)
		{
			this.NavigationController.SetToolbarHidden(false, true);

			if (this.appDelegate.tablePhotoSearch == 1)
			{
				this.TableView.ScrollToRow(this.appDelegate.selectedIndexPhoto, UITableViewScrollPosition.Top, true);
				this.TableView.SelectRow(this.appDelegate.selectedIndexPhoto, true, UITableViewScrollPosition.Top);
				//	Console.WriteLine("Search selected");
			}
		}

		private void addImageContext(string[] directory, int i)
		{
			Console.WriteLine("i: " + i);
			try
			{
				CGPDFDocument pdfToLoad = CGPDFDocument.FromFile(directory[i]);
				if (pdfToLoad == null)
				{
					throw new NullReferenceException();
				}
				else {
					var firstPage = pdfToLoad.GetPage(1);
					var width = 240.0f;
					var pageRect = firstPage.GetBoxRect(CGPDFBox.Media);


					var pdfScale = width / pageRect.Size.Width;

					pageRect.Size = new CGSize(pageRect.Size.Width * pdfScale, pageRect.Size.Height * pdfScale);
					pageRect.X = 0;
					pageRect.Y = 0;
					UIGraphics.BeginImageContext(pageRect.Size);

					var context = UIGraphics.GetCurrentContext();

					context.SetFillColor(1.0f, 1.0f, 1.0f, 1.0f);
					context.FillRect(pageRect);
					context.SaveState();
					context.TranslateCTM(0.0f, pageRect.Size.Height);
					context.ScaleCTM(1.0f, -1.0f);
					context.ConcatCTM(firstPage.GetDrawingTransform(CGPDFBox.Media, pageRect, 0, true));
					context.DrawPDFPage(firstPage);
					context.RestoreState();

					UIImage thm = UIGraphics.GetImageFromCurrentImageContext();

					UIGraphics.EndImageContext();
					this.thumbnailImage.Add(thm);
				}
			}
			catch (NullReferenceException)
			{
				UIImage nullImage = new UIImage();
				this.corruptIndex.Add(i);
				this.thumbnailImage.Add(nullImage);
			}
		}

		public override void ViewDidLoad()
		{
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
			this.NavigationItem.Title = "My Photos & Videos";
			this.TableView.RowHeight = 70.0f;
			this.appDelegate.photo = this;
			this.appDelegate.resultsStringPhoto = this.pdfList;
			UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			this.appDelegate.photoReader = story.InstantiateViewController("PhotosReader") as PhotosReader;

			this.searchController = new UISearchController(new resultsControllerPhoto());
			this.searchController.SearchResultsUpdater = new searchUpdatorPhoto(this);
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

			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			var directoryJPG = Directory.GetFiles(documents, "*.jpg");
			var directoryPNG = Directory.GetFiles(documents, "*.png");
			var directoryGIF = Directory.GetFiles(documents, "*.gif");
			var directorySVG = Directory.GetFiles(documents, "*.svg");
			var directoryICO = Directory.GetFiles(documents, "*.ico");


			this.appDelegate.directories = directoryJPG;

			NSError error = new NSError();
			try
			{
				if (NSFileManager.DefaultManager.GetDirectoryContent(documents, out error).Length == 0)
				{
					throw new FileNotFoundException();
				}
				else {
					//Photo formats
					//.jpg
					for (int i = 0; i <= directoryJPG.Length - 1; i++)
					{

						try
						{
							if (directoryJPG[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								this.pdfList.Add(Path.GetFileName(directoryJPG[i]));
								this.creationDate.Add(Convert.ToString(Directory.GetCreationTime(directoryJPG[i])));
								addImageContext(directoryJPG, i);
							}
						}
						catch (ArgumentOutOfRangeException)
						{
							Console.WriteLine("File does not exist");
						}
					}

					//.png
					for (int i = 0; i <= directoryPNG.Length - 1; i++)
					{

						try
						{
							if (directoryPNG[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								this.pdfList.Add(Path.GetFileName(directoryPNG[i]));
								this.creationDate.Add(Convert.ToString(Directory.GetCreationTime(directoryPNG[i])));
								addImageContext(directoryPNG, i);
							}
						}
						catch (ArgumentOutOfRangeException)
						{
							Console.WriteLine("File does not exist");
						}
					}

					//gif
					for (int i = 0; i <= directoryGIF.Length - 1; i++)
					{

						try
						{
							if (directoryGIF[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								this.pdfList.Add(Path.GetFileName(directoryGIF[i]));
								this.creationDate.Add(Convert.ToString(Directory.GetCreationTime(directoryGIF[i])));
								addImageContext(directoryGIF, i);
							}
						}
						catch (ArgumentOutOfRangeException)
						{
							Console.WriteLine("File does not exist");
						}
					}

					//.svg
					for (int i = 0; i <= directorySVG.Length - 1; i++)
					{

						try
						{
							if (directorySVG[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								this.pdfList.Add(Path.GetFileName(directorySVG[i]));
								this.creationDate.Add(Convert.ToString(Directory.GetCreationTime(directorySVG[i])));
								addImageContext(directorySVG, i);
							}
						}
						catch (ArgumentOutOfRangeException)
						{
							Console.WriteLine("File does not exist");
						}
					}

					//.ico
					for (int i = 0; i <= directoryICO.Length - 1; i++)
					{

						try
						{
							if (directoryICO[i] == null)
							{
								throw new ArgumentOutOfRangeException();
							}
							else {
								this.pdfList.Add(Path.GetFileName(directoryICO[i]));
								this.creationDate.Add(Convert.ToString(Directory.GetCreationTime(directoryICO[i])));
								addImageContext(directoryICO, i);
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

			if (this.pdfList.Count == 0)
			{
				emptyPDF = new UIView();

				emptyImage = new UIImageView();

				this.description = new UILabel();
				this.description.Text = "This page contains no passwords. To get started adding";
				this.description.Font = UIFont.FromName("Georgia-Italic", 26.0f);

				this.description_2 = new UILabel();
				this.description_2.Text = "your own passwords click \ud83d\udcdd";
				this.description_2.Font = UIFont.FromName("Georgia-Italic", 26.0f);

				this.description_2.AdjustsFontSizeToFitWidth = true;
				this.description.AdjustsFontSizeToFitWidth = true;

				this.titleDescription = new UILabel();
				this.titleDescription.Text = "No Photos";
				this.titleDescription.Font = UIFont.SystemFontOfSize(26.0f);
				this.titleDescription.Font = UIFont.FromName("AmericanTypewriter-Bold", 26.0f);

				if (UIApplication.SharedApplication.StatusBarHidden == true)
				{
					emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

					emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.Y - 200.0f, 200, 200);

					this.description.Frame = new CGRect(this.View.Center.X - 315.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
					this.description_2.Frame = new CGRect(this.View.Center.X - 155.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);

					this.titleDescription.Frame = new CGRect(this.View.Center.X - 70.0f, this.View.Center.Y - 180.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
				}
				else
				{
					emptyPDF.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

					emptyImage.Frame = new CGRect(emptyPDF.Center.X - 100.0f, emptyPDF.Center.X - 95.0f, 200, 200);

					this.description.Frame = new CGRect(this.View.Center.X - 190.0f, this.View.Center.Y, UIScreen.MainScreen.Bounds.Width - 50.0f, 50.0f);
					this.description_2.Frame = new CGRect(this.View.Center.X - 95.0f, this.View.Center.Y + 50.0f, UIScreen.MainScreen.Bounds.Width - 210.0f, 50.0f);
					this.titleDescription.Frame = new CGRect(this.View.Center.X - 70.0f, this.View.Center.Y - 240.0f, UIScreen.MainScreen.Bounds.Width - 50.0f, 300.0f);
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

			this.NavigationController.SetToolbarHidden(false, true);
			this.NavigationController.Toolbar.BarTintColor = UIColor.White;
			this.NavigationController.Toolbar.BackgroundColor = UIColor.White;
			this.NavigationController.Toolbar.TintColor = UIColor.Blue;

			UIBarButtonItem bin = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, e) =>
			{
				//pushes the password reader controller which allows the user to enter 
				var documents_2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				//var passwordDirectory_2 = Path.Combine(documents, "Passwords");

				var directoryJPG_2 = Directory.GetFiles(documents, "*.jpg");
				var directoryPNG_2 = Directory.GetFiles(documents, "*.png");
				var directoryGIF_2 = Directory.GetFiles(documents, "*.gif");
				var directorySVG_2 = Directory.GetFiles(documents, "*.svg");
				var directoryICO_2 = Directory.GetFiles(documents, "*.ico");


				NSError error_2 = new NSError();
				if (this.pdfList.Count == 0)
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
						UIAlertController emptyPasswords = UIAlertController.Create("Clear your photos?", "Are you sure you want to clear the whole list of your photos?", UIAlertControllerStyle.Alert);

						UIAlertAction confirmed = UIAlertAction.Create("Yes", UIAlertActionStyle.Destructive, (UIAlertAction obj) =>
						{
							//delete list for the table 
							//delete files contained inside the directory 
							this.pdfList.Clear();
							this.creationDate.Clear();

							Console.WriteLine("Number of files: " + NSFileManager.DefaultManager.GetDirectoryContent(documents_2, out error_2).Length);

							//.jpg
							for (int j = 0; j <= directoryJPG_2.Length - 1; j++)
							{
								File.Delete(directoryJPG_2[j]);
							}

							/*try
							{
								if (String.IsNullOrEmpty(directoryJPG_2[NSFileManager.DefaultManager.GetDirectoryContent(documents_2, out error_2).Length - 1]) == true)
								{
									throw new IndexOutOfRangeException();
								}
								else {
									File.Delete(directoryJPG_2[NSFileManager.DefaultManager.GetDirectoryContent(documents_2, out error_2).Length - 1]);
								}
							}
							catch (IndexOutOfRangeException)
							{
								Console.WriteLine("File does not exist inside this directory");
							}*/

							//.png
							for (int j = 0; j <= directoryPNG_2.Length - 1; j++)
							{
								File.Delete(directoryPNG_2[j]);
							}

							//.gif
							for (int j = 0; j <= directoryGIF_2.Length - 1; j++)
							{
								File.Delete(directoryGIF_2[j]);
							}

							//.svg
							for (int j = 0; j <= directorySVG_2.Length - 1; j++)
							{
								File.Delete(directorySVG_2[j]);
							}

							//.ico
							for (int j = 0; j <= directoryICO_2.Length - 1; j++)
							{
								File.Delete(directoryICO_2[j]);
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
						for (int i = 0; i <= this.selectedCellsToRemove.Count - 1; i++)
						{
							this.pdfList.RemoveAt(i);
							this.creationDate.RemoveAt(i);

							Console.WriteLine("Contents: " + Directory.GetFiles(documents_2)[i]);
							File.Delete(Directory.GetFiles(documents_2)[i]);
							this.NavigationController.PopViewController(true);
						}
					}
				}
			});

			this.SetToolbarItems(new UIBarButtonItem[] { bin }, true);

			UIBarButtonItem cancelEditing = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) =>
			{
				this.selectedCellsToRemove.Clear();
				this.TableView.SetEditing(false, true);
				this.NavigationItem.SetRightBarButtonItem(this.appDelegate.editPassword, true);
			});

			//nav bar items
			UIBarButtonItem edit = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (object sender, EventArgs e) =>
			{
				//pushes the password reader controller which allows the user to enter 
				var documents_2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				var directoryJPG_3 = Directory.GetFiles(documents_2, "*.jpg");
				var directoryPNG_3 = Directory.GetFiles(documents_2, "*.png");
				var directoryGIF_3 = Directory.GetFiles(documents_2, "*.gif");
				var directorySVG_3 = Directory.GetFiles(documents_2, "*.svg");
				var directoryICO_3 = Directory.GetFiles(documents_2, "*.ico");

				NSError error_2 = new NSError();
				if (this.pdfList.Count == 0)
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

			this.appDelegate.editPassword = edit;
			this.NavigationItem.SetRightBarButtonItem(edit, false);

			this.TableView.AllowsSelection = true;
			this.TableView.AllowsSelectionDuringEditing = true;
			this.TableView.AllowsMultipleSelectionDuringEditing = true;
			this.TableView.AllowsMultipleSelection = true;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return this.pdfList.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell pdfCell = new UITableViewCell(UITableViewCellStyle.Subtitle, "pdfCell");

			if (pdfCell == null)
			{
				pdfCell = new UITableViewCell(UITableViewCellStyle.Subtitle, "pdfCell");
			}

			pdfCell.TextLabel.Text = pdfList[indexPath.Row];
			pdfCell.TextLabel.TextColor = UIColor.Black;

			pdfCell.DetailTextLabel.Text = "Created on: " + creationDate[indexPath.Row];
			pdfCell.DetailTextLabel.TextColor = UIColor.LightGray;

			pdfCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			UIImageView screenShot = new UIImageView();
			screenShot.Frame = new CGRect(0, 20, 40, 40);
			screenShot.Image = this.thumbnailImage[indexPath.Row];

			//pdfCell.AccessoryView = screenShot;

			return pdfCell;
		}

		public void filteredContent(string searchedText)
		{
			this.appDelegate.newResultsPhoto = this.pdfList.Where((arg) => arg.ToLower().Contains(searchedText.ToLower()) || arg.ToUpper().Contains(searchedText.ToUpper())).ToList();

			this.TableView.ReloadData();

			this.appDelegate.tableView.ReloadData();
		}

		private void corruptController(int index)
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var directory = Directory.GetFiles(documents, "*.jpg");

			UIAlertController corruptAlert = UIAlertController.Create("Corrupt file cannot be opened!", "'" + Path.GetFileName(directory[index]) + "'" + " cannot be opened because it appears to be corrupt. Would you like to delete this file?", UIAlertControllerStyle.Alert);

			UIAlertAction confirmed = UIAlertAction.Create("No", UIAlertActionStyle.Destructive, (UIAlertAction obj) =>
			{
				corruptAlert.Dispose();
			});

			UIAlertAction deleteFile = UIAlertAction.Create("Delete file", UIAlertActionStyle.Cancel, (UIAlertAction obj) =>
			{
				File.Delete(directory[index]);
				this.pdfList.RemoveAt(index);
				this.creationDate.RemoveAt(index);
				this.thumbnailImage.RemoveAt(index);

				int indexCorrupt = this.pdfList.IndexOf(this.pdfList[index]);
				int indexOfficial = this.corruptIndex.IndexOf(indexCorrupt);

				this.corruptIndex.RemoveAt(indexOfficial);
				this.TableView.ReloadData();
			});

			corruptAlert.AddAction(confirmed);
			corruptAlert.AddAction(deleteFile);

			if (this.PresentedViewController == null)
			{
				this.PresentViewController(corruptAlert, true, () =>
				{
					SystemSound sound = new SystemSound(4095);
					sound.PlaySystemSound();
				});
			}
			else {
				this.PresentedViewController.DismissViewController(true, () =>
				{
					this.PresentedViewController.Dispose();
					this.PresentViewController(corruptAlert, true, () =>
					{
						SystemSound sound = new SystemSound(4095);
						sound.PlaySystemSound();
					});
				});
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			this.search.ResignFirstResponder();

			//has the user selected the cell

			/*	if (this.corruptIndex.Contains(indexPath.Row) == true)
				{
					//replace the previous indices inside the corrupt index list with the new indices 
					corruptController(indexPath.Row);
				}*/
			if (this.TableView.Editing == true)
			{
				this.selectedCellsToRemove.Add(indexPath.Row);
				Console.WriteLine("Remove cells list count is: " + this.selectedCellsToRemove.Count);
			}
			else {
				UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
				//pushes the web view controller that loads the PDF 
				tableView.DeselectRow(indexPath, true);

				appDelegate.photoString = tableView.CellAt(indexPath).TextLabel.Text;

				try
				{
					PhotosReader photoControl = story.InstantiateViewController("PhotosReader") as PhotosReader;
					if (photoControl == null)
					{
						throw new NullReferenceException();
					}
					else {
						this.NavigationController.PushViewController(photoControl, true);
					}
				}
				catch (NullReferenceException)
				{
					Console.WriteLine("Word Viewer");
				}
			}
		}

		public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
		{
			if (this.TableView.Editing == true)
			{
				int index = this.selectedCellsToRemove.IndexOf(indexPath.Row);
				this.selectedCellsToRemove.RemoveAt(index);
				Console.WriteLine("Remove cells list count is: " + this.selectedCellsToRemove.Count);
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
	}

	public class searchUpdatorPhoto : UISearchResultsUpdating
	{
		PhotosController search = new PhotosController();

		public searchUpdatorPhoto(PhotosController searchValue)
		{
			search = searchValue;
		}

		public override void UpdateSearchResultsForSearchController(UISearchController searchController)
		{
			search.filteredContent(searchController.SearchBar.Text);
		}
	}

	public class resultsControllerPhoto : UITableViewController
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

		public resultsControllerPhoto() { }

		public resultsControllerPhoto(UISearchController searchController)
		{
			search = searchController;
		}

		public resultsControllerPhoto(UISearchController searchController, List<string> filteredResultsRef)
		{
			search = searchController;
			filteredResults = filteredResultsRef;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return this.appDelegate.newResultsPhoto.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cellSearch = new UITableViewCell(UITableViewCellStyle.Value1, "SearchCell");

			if (cellSearch == null)
			{
				cellSearch = new UITableViewCell(UITableViewCellStyle.Value1, "SearchCell");
			}

			cellSearch.TextLabel.Text = this.appDelegate.newResultsPhoto[indexPath.Row];
			cellSearch.TextLabel.TextColor = UIColor.Black;

			cellSearch.DetailTextLabel.Text = ">";
			cellSearch.DetailTextLabel.TextColor = UIColor.LightGray;

			return cellSearch;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//UIStoryboard story = UIStoryboard.FromName("Main", NSBundle.MainBundle);
			//pushes the web view controller that loads the PDF 
			this.appDelegate.tablePhotoSearch = 1;
			this.DismissViewController(true, () =>
			{
				this.appDelegate.tablePhotoSearch = 1;
				int indexRow = this.appDelegate.resultsStringPhoto.IndexOf(tableView.CellAt(indexPath).TextLabel.Text);
				Console.WriteLine("Index to use: " + indexRow);
				this.appDelegate.selectedIndexPhoto = NSIndexPath.FromRowSection(indexRow, 0);

				//search cell has been selected
				this.appDelegate.searchedStringPhoto = tableView.CellAt(indexPath).TextLabel.Text;
				this.appDelegate.photo.ViewDidAppear(true);
			});

			tableView.DeselectRow(indexPath, true);
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
