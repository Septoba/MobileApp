﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Game.GameRules;
using Game.Models;
using Game.ViewModels;

namespace Game.Views
{
    /// <summary>
    /// Create Character
    /// </summary>
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public partial class CharacterCreatePage : ContentPage
    {
        // The Character to create
        public GenericViewModel<CharacterModel> ViewModel { get; set; }

        // Hold the current location selected
        public ItemLocationEnum PopupLocationEnum = ItemLocationEnum.Unknown;


        // Empty Constructor for UTs
        public CharacterCreatePage(bool UnitTest) { }

        /// <summary>
        /// Constructor for Create makes a new model
        /// </summary>
        public CharacterCreatePage(GenericViewModel<CharacterModel> data)
        {
            InitializeComponent();

            data.Data = new CharacterModel();
            this.ViewModel = data;

            this.ViewModel.Title = "Create";

            // Load the values for the Level into the Picker
            for (var i = 1; i <= LevelTableHelper.MaxLevel; i++)
            {
                LevelPicker.Items.Add(i.ToString());
            }

            this.ViewModel.Data.Level = 1;
            // LevelPicker.SelectedIndex = ViewModel.Data.Level - 1;

            _ = UpdatePageBindingContext();

            //Give picker a default value
            LevelPicker.SelectedIndex = 0;
            JobPicker.SelectedIndex = 0;
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

            //Set stat labels on refresh
            AttackLabel.Text = this.ViewModel.Data.Attack.ToString();
            DefenseLabel.Text = this.ViewModel.Data.Defense.ToString();
            SpeedLabel.Text = this.ViewModel.Data.Speed.ToString();

            // This resets the Picker to -1 index, need to reset it back
            ViewModel.Data.Level = level;
            LevelPicker.SelectedIndex = ViewModel.Data.Level - 1;

            //Should set picker to most current value. Not working for some reason
            //Set Job Picker to the new job
            JobPicker.SelectedItem = ViewModel.Data.Job.ToString();

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
            //prevent submission if something is wrong
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0019:Use pattern matching", Justification = "<Pending>")]
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
                ImageURI = "icon_cancel.png",
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
            var ImageSource = "icon_add.png";

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
            _ = DiceAnimationHandeler();

            _ = RandomizeCharacter();

            return;
        }

        /// <summary>
        /// 
        /// Randomize the Character
        /// Keep the Level the Same
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RandomizeCharacter()
        {
            // Randomize Name & Job
            ViewModel.Data.Name = RandomPlayerHelper.GetCharacterName();
            ViewModel.Data.Description = RandomPlayerHelper.GetCharacterDescription();
            ViewModel.Data.CodeName = RandomPlayerHelper.GetCharacterCodeName();
            ViewModel.Data.Job = CharacterJobEnumHelper.ConvertStringToEnum(RandomPlayerHelper.GetCharacterJob());
           

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

        /// <summary>
        /// Setup the Dice Animation
        /// </summary>
        /// <param name="sender"></param>   
        /// <param name="e"></param>
        public bool DiceAnimationHandeler()
        {
            // Animate the Rolling of the Dice
            var image = RollDice;
            uint duration = 1000;

            var parentAnimation = new Animation();

            // Grow the image Size
            var scaleUpAnimation = new Animation(v => image.Scale = v, 1, 2, Easing.SpringIn);

            // Spin the Image
            var rotateAnimation = new Animation(v => image.Rotation = v, 0, 360);

            // Shrink the Image
            var scaleDownAnimation = new Animation(v => image.Scale = v, 2, 1, Easing.SpringOut);

            parentAnimation.Add(0, 0.5, scaleUpAnimation);
            parentAnimation.Add(0, 1, rotateAnimation);
            parentAnimation.Add(0.5, 1, scaleDownAnimation);

            parentAnimation.Commit(this, "ChildAnimations", 16, duration, null, null);

            return true;
        }

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
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                NameEntry.BackgroundColor = Color.FromHex("848884");
                NameFrame.BorderColor = Color.Red;
            }
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                NameEntry.BackgroundColor = Color.FromHex("848884");
                NameFrame.BorderColor = Color.Red;
            }
            if (!string.IsNullOrEmpty(NameEntry.Text))
            {
                NameEntry.BackgroundColor = Color.FromHex("36454F");
                NameFrame.BorderColor = Color.FromHex("#696969");
            }
            if (!string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                NameEntry.BackgroundColor = Color.FromHex("36454F");
                NameFrame.BorderColor = Color.FromHex("#696969");
            }
        }

        /// <summary>
        /// Catch changes in description text and changes the color of the box if empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Description_onTextChange(object sender, ValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = Color.FromHex("848884");
                DescFrame.BorderColor = Color.Red;
            }
            if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = Color.FromHex("848884");
                DescFrame.BorderColor = Color.Red;
            }
            if (!string.IsNullOrEmpty(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = Color.FromHex("36454F");
                DescFrame.BorderColor = Color.FromHex("#696969");
            }
            if (!string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                DescriptionEntry.BackgroundColor = Color.FromHex("36454F");
                DescFrame.BorderColor = Color.FromHex("#696969");
            }
        }

        /// <summary>
        /// Catch changes in codename text and changes the color of the box if empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Codename_onTextChange(object sender, ValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = Color.FromHex("848884");
                CodenameFrame.BorderColor = Color.Red;
            }
            if (string.IsNullOrWhiteSpace(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = Color.FromHex("848884");
                CodenameFrame.BorderColor = Color.Red;
            }
            if (!string.IsNullOrEmpty(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = Color.FromHex("36454F");
                CodenameFrame.BorderColor = Color.FromHex("#696969");
            }
            if (!string.IsNullOrWhiteSpace(CodenameEntry.Text))
            {
                CodenameEntry.BackgroundColor = Color.FromHex("36454F");
                CodenameFrame.BorderColor = Color.FromHex("#696969");
            }
        }
    }
}