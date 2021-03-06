using Game.Models;
using Game.ViewModels;

using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Game.Views
{
    /// <summary>
    /// Create Item
    /// </summary>
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemCreatePage : ContentPage
    {
        // The item to create
        public GenericViewModel<ItemModel> ViewModel = new GenericViewModel<ItemModel>();

        // Empty Constructor for UTs
        public ItemCreatePage(bool UnitTest) { }

        /// <summary>
        /// Constructor for Create makes a new model
        /// </summary>
        public ItemCreatePage()
        {
            InitializeComponent();

            this.ViewModel.Data = new ItemModel();

            BindingContext = this.ViewModel;

            this.ViewModel.Title = "Create";

            //Need to make the SelectedItem a string, so it can select the correct item.
            LocationPicker.SelectedItem = ViewModel.Data.Location.ToString();
            AttributePicker.SelectedItem = ViewModel.Data.Attribute.ToString();
            
            // Default Values for Location and Attribute
            LocationPicker.SelectedIndex = 0;
            AttributePicker.SelectedIndex = 0;
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
                    ViewModel.Data.ImageURI = Services.ItemService.DefaultImageURI;
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
        /// Catch the change to the Stepper for Range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Range_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            RangeValue.Text = string.Format("{0}", e.NewValue);
        }

        /// <summary>
        /// Catch the change to the stepper for Value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Value_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            ValueValue.Text = string.Format("{0}", e.NewValue);
        }

        /// <summary>
        /// Catch the change to the stepper for Damage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Damage_OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            DamageValue.Text = string.Format("{0}", e.NewValue);
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
        /// Prevents submission if name or description is emtpy
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

            if(string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                return false;
            }

            return true;
        }

    }
}