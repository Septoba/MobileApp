using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Game.ViewModels;
using Game.Models;
using Game.GameRules;

namespace Game.Views
{
    /// <summary>
    /// Character Update Page
    /// </summary>
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CharacterUpdatePage : ContentPage
    {
        // The Character to create
        public GenericViewModel<CharacterModel> ViewModel { get; set; }

        // Hold the current location selected
        public ItemLocationEnum PopupLocationEnum = ItemLocationEnum.Unknown;

        // Empty Constructor for UTs
        public CharacterUpdatePage(bool UnitTest) { }

        // Copy of character data in case of cancel
        public CharacterModel CharacterCopy;

        /// <summary>
        /// Constructor for Create makes a new model
        /// </summary>
        public CharacterUpdatePage(GenericViewModel<CharacterModel> data)
        {
            InitializeComponent();

            CharacterCopy = new CharacterModel(data.Data);

            BindingContext = this.ViewModel = data;

            this.ViewModel.Title = "Update " + data.Title;

            // Load the values for the Level into the Picker
            for (var i = 1; i <= LevelTableHelper.MaxLevel; i++)
            {
                LevelPicker.Items.Add(i.ToString());
            }

            _ = UpdatePageBindingContext();
        }

        /// <summary>
        /// Redo the Binding to cause a refresh
        /// </summary>
        /// <returns></returns>
        public bool UpdatePageBindingContext()
        {
            // Temp store off the Level
            var level = this.ViewModel.Data.Level;

            // Clear the Binding and reset it
            BindingContext = null;
            BindingContext = this.ViewModel;

            // This resets the Picker to -1 index, need to reset it back
            ViewModel.Data.Level = level;
            LevelPicker.SelectedIndex = this.ViewModel.Data.Level - 1;

            //Update job picker
            JobPicker.SelectedItem = this.ViewModel.Data.Job.ToString();

            // If unknown
            if (JobPicker.SelectedItem.ToString() == CharacterJobEnum.Unknown.ToString())
            {
                JobPicker.SelectedIndex = 0;
            }

            //Update stat labels
            AttackLabel.Text = this.ViewModel.Data.Attack.ToString();
            DefenseLabel.Text = this.ViewModel.Data.Defense.ToString();
            SpeedLabel.Text = this.ViewModel.Data.Speed.ToString();

            ManageHealth();

            AddItemsToDisplay();

            return true;
        }

        /// <summary>
        /// The Level selected from the list
        /// Need to recalculate Max Health
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Level_Changed(object sender, EventArgs args)
        {
            // Change the Level
            ViewModel.Data.Level = LevelPicker.SelectedIndex + 1;

            ManageHealth();
        }

        /// <summary>
        /// Change the Level Picker
        /// </summary>
        public void ManageHealth()
        {
            // Roll for new HP
            ViewModel.Data.MaxHealth = RandomPlayerHelper.GetHealth(ViewModel.Data.Level);

            // Show the Result
            HealthSlider.Value = ViewModel.Data.MaxHealth;

            HealthLabel.Text = ViewModel.Data.MaxHealth.ToString();
        }

        /// <summary>
        /// Save by calling for Create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Save_Clicked(object sender, EventArgs e)
        {
            if (CheckIfReadyToSubmit())
            { 
                // If the image in the data box is empty, use the default one..
                if (string.IsNullOrEmpty(ViewModel.Data.ImageURI))
                {
                    ViewModel.Data.ImageURI = new CharacterModel().ImageURI;
                }

                MessagingCenter.Send(this, "Create", ViewModel.Data);

                _ = await Navigation.PopModalAsync();
            }
        }

        /// <summary>
        /// Cancel the Create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            // TODO: Mike, refactor this. Setting and Showing is causing a bug
            // Don't want to set the value on update constructor, only after save on the page
            // need to make sure that cancel from a save, actually cancels.
            // Make a copy of the object and work from that and then have that passed in to update

            // Return to original state
            ViewModel.Data.Update(CharacterCopy);

            _ = await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Catch the change to the Slider for Attack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AttackSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newAtkValue = Math.Round(e.NewValue);
            AttackLabel.Text = newAtkValue.ToString();
            AttackSlider.Value = newAtkValue;
        }

        /// <summary>
        /// Catch the change to the Slider for Defense
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DefenseSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newDefValue = Math.Round(e.NewValue);
            DefenseLabel.Text = newDefValue.ToString();
            DefenseSlider.Value = newDefValue;
        }

        /// <summary>
        /// Catch the change to the Slider for Speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SpeedSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newSpdValue = Math.Round(e.NewValue);
            SpeedLabel.Text = newSpdValue.ToString();
            SpeedSlider.Value = newSpdValue;
        }

        /// <summary>
        /// The row selected from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnPopupItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ItemModel data = args.SelectedItem as ItemModel;
            if (data == null)
            {
                return;
            }

            _ = ViewModel.Data.AddItem(PopupLocationEnum, data.Id);

            AddItemsToDisplay();

            ClosePopup();
        }

        /// <summary>
        /// Show the Popup for Selecting Items
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool ShowPopup(ItemLocationEnum location)
        {
            PopupItemSelector.IsVisible = true;

            PopupLocationLabel.Text = "Items for :";
            PopupLocationValue.Text = location.ToMessage();

            // Make a fake item for None
            var NoneItem = new ItemModel
            {
                Id = null, // will use null to clear the item
                Guid = "None", // how to find this item amoung all of them
                ImageURI = "empty_item_blue.png",
                Name = "None",
                Description = "None"
            };

            List<ItemModel> itemList = new List<ItemModel>
            {
                NoneItem
            };

            // Add the rest of the items to the list
            itemList.AddRange(ItemIndexViewModel.Instance.GetLocationItems(location));

            // Populate the list with the items
            PopupLocationItemListView.ItemsSource = itemList;

            // Remember the location for this popup
            PopupLocationEnum = location;

            return true;
        }

        /// <summary>
        /// When the user clicks the close in the Popup
        /// hide the view
        /// show the scroll view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClosePopup_Clicked(object sender, EventArgs e)
        {
            ClosePopup();
        }

        /// <summary>
        /// Close the popup
        /// </summary>
        public void ClosePopup()
        {
            PopupItemSelector.IsVisible = false;
        }

        /// <summary>
        /// Show the Items the Character has
        /// </summary>
        public void AddItemsToDisplay()
        {
            var FlexList = ItemBox.Children.ToList();

            foreach (var data in FlexList)
            {
                _ = ItemBox.Children.Remove(data);
            }

            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.Head));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.Necklass));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.PrimaryHand));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.OffHand));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.RightFinger));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.LeftFinger));
            ItemBox.Children.Add(GetItemToDisplay(ItemLocationEnum.Feet));
        }

        /// <summary>
        /// Look up the Item to Display
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public StackLayout GetItemToDisplay(ItemLocationEnum location)
        {
            // Get the Item, if it exist show the info
            // If it does not exist, show a Plus Icon for the location

            // Defualt Image is the Plus
            var ImageSource = "add_item_blue.png";

            var data = ViewModel.Data.GetItemByLocation(location);
            if (data == null)
            {
                data = new ItemModel { Location = location, ImageURI = ImageSource };
            }

            // Hookup the Image Button to show the Item picture
            var ItemButton = new ImageButton
            {
                Style = (Style)Application.Current.Resources["ImageMediumStyle"],
                Source = data.ImageURI
            };

            // Add a event to the user can click the item and see more
            ItemButton.Clicked += (sender, args) => ShowPopup(location);

            // Add the Display Text for the item
            var ItemLabel = new Label
            {
                Text = location.ToMessage(),
                Style = (Style)Application.Current.Resources["ValueStyleMicro"],
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // Put the Image Button and Text inside a layout
            var ItemStack = new StackLayout
            {
                Padding = 3,
                Style = (Style)Application.Current.Resources["ItemImageLabelBox"],
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                    ItemButton,
                    ItemLabel
                },
            };

            return ItemStack;
        }

        /// <summary>
        /// Randomize Character Values and Items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RollDice_Clicked(object sender, EventArgs e)
        {
            //_ = DiceAnimationHandeler();

            _ = RandomizeCharacter();

            return;
        }

        /// <summary>
        /// Randomize the Character, keep the level the same
        /// </summary>
        /// <returns></returns>
        public bool RandomizeCharacter()
        {
            // Randomize Name
            ViewModel.Data.Name = RandomPlayerHelper.GetCharacterName();
            ViewModel.Data.Description = RandomPlayerHelper.GetCharacterDescription();
            ViewModel.Data.CodeName = RandomPlayerHelper.GetCharacterCodeName();

            // Randomize the Attributes
            ViewModel.Data.Attack = RandomPlayerHelper.GetAbilityValue();
            ViewModel.Data.Speed = RandomPlayerHelper.GetAbilityValue();
            ViewModel.Data.Defense = RandomPlayerHelper.GetAbilityValue();

            // Randomize an Item for Location
            ViewModel.Data.Head = RandomPlayerHelper.GetItem(ItemLocationEnum.Head);
            ViewModel.Data.Necklass = RandomPlayerHelper.GetItem(ItemLocationEnum.Necklass);
            ViewModel.Data.PrimaryHand = RandomPlayerHelper.GetItem(ItemLocationEnum.PrimaryHand);
            ViewModel.Data.OffHand = RandomPlayerHelper.GetItem(ItemLocationEnum.OffHand);
            ViewModel.Data.RightFinger = RandomPlayerHelper.GetItem(ItemLocationEnum.Finger);
            ViewModel.Data.LeftFinger = RandomPlayerHelper.GetItem(ItemLocationEnum.Finger);
            ViewModel.Data.Feet = RandomPlayerHelper.GetItem(ItemLocationEnum.Feet);

            ViewModel.Data.MaxHealth = RandomPlayerHelper.GetHealth(ViewModel.Data.Level);

            ViewModel.Data.ImageURI = RandomPlayerHelper.GetCharacterImage();

            _ = UpdatePageBindingContext();

            return true;
        }

        ///// <summary>
        ///// Setup the Dice Animation
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public bool DiceAnimationHandeler()
        //{
        //    // Animate the Rolling of the Dice
        //    var image = RollDice;
        //    uint duration = 1000;

        //    var parentAnimation = new Animation();

        //    // Grow the image Size
        //    var scaleUpAnimation = new Animation(v => image.Scale = v, 1, 2, Easing.SpringIn);

        //    // Spin the Image
        //    var rotateAnimation = new Animation(v => image.Rotation = v, 0, 360);

        //    // Shrink the Image
        //    var scaleDownAnimation = new Animation(v => image.Scale = v, 2, 1, Easing.SpringOut);

        //    parentAnimation.Add(0, 0.5, scaleUpAnimation);
        //    parentAnimation.Add(0, 1, rotateAnimation);
        //    parentAnimation.Add(0.5, 1, scaleDownAnimation);

        //    parentAnimation.Commit(this, "ChildAnimations", 16, duration, null, null);

        //    return true;
        //}

        /// <summary>
        /// Returns true if all required fields are filled out
        /// </summary>
        /// <returns></returns>
        public bool CheckIfReadyToSubmit()
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                return false;
            }

            if (string.IsNullOrEmpty(DescriptionEntry.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                return false;
            }

            if (string.IsNullOrEmpty(CodenameEntry.Text))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(CodenameEntry.Text))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Catch changes in the name text box and changes the color if empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Name_onTextChange(object sender, ValueChangedEventArgs e)
        {
            NameEntry.BackgroundColor = (Color)Application.Current.Resources["ViewBackgroundColor"];
            NameFrame.BorderColor = (Color)Application.Current.Resources["BorderColor"];
            NameEntry.Placeholder = "Name:";

            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                NameEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                NameFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }

            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                NameEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                NameFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }
        }
        /// <summary>
        /// Catch changes in description text and changes the color of the box if empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Description_onTextChange(object sender, ValueChangedEventArgs e)
        {
            DescriptionEntry.BackgroundColor = (Color)Application.Current.Resources["ViewBackgroundColor"];
            DescriptionFrame.BorderColor = (Color)Application.Current.Resources["BorderColor"];
            DescriptionEntry.Placeholder = "Description:";

            if (string.IsNullOrEmpty(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                DescriptionFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }

            if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                DescriptionFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }
        }

        /// <summary>
        /// Catch changes in codename text and changes the color of the box if empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Codename_onTextChange(object sender, ValueChangedEventArgs e)
        {
            CodenameEntry.BackgroundColor = (Color)Application.Current.Resources["ViewBackgroundColor"];
            CodenameFrame.BorderColor = (Color)Application.Current.Resources["BorderColor"];
            CodenameEntry.Placeholder = "Codename:";

            if (string.IsNullOrEmpty(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                CodenameFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }

            if (string.IsNullOrWhiteSpace(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = (Color)Application.Current.Resources["TriciaryBackgroundColor"];
                CodenameFrame.BorderColor = (Color)Application.Current.Resources["Error"];
            }
        }
    }
}