using NUnit.Framework;

using Game;
using Game.Views;
using Game.ViewModels;
using Game.Models;

using Xamarin.Forms;
using Xamarin.Forms.Mocks;
using System.Linq;

namespace UnitTests.Views
{
    [TestFixture]
    public class MonsterCreatePageTests : MonsterCreatePage
    {
        App app;
        MonsterCreatePage page;

        public MonsterCreatePageTests() : base(true) { }

        [SetUp]
        public void Setup()
        {
            // Initilize Xamarin Forms
            MockForms.Init();

            //This is your App.xaml and App.xaml.cs, which can have resources, etc.
            app = new App();
            Application.Current = app;

            page = new MonsterCreatePage(new GenericViewModel<MonsterModel>(new MonsterModel()));
        }

        [TearDown]
        public void TearDown()
        {
            Application.Current = null;
        }

        [Test]
        public void MonsterCreatePage_Constructor_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = page;

            // Reset

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void MonsterCreatePage_Cancel_Clicked_Default_Should_Pass()
        {
            // Arrange

            // Act
            page.Cancel_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Save_Clicked_Default_Should_Pass()
        {
            // Arrange

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Save_Clicked_Null_Image_Should_Pass()
        {
            // Arrange
            page.ViewModel.Data.ImageURI = null;

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_OnBackButtonPressed_Valid_Should_Pass()
        {
            // Arrange

            // Act
            _ = OnBackButtonPressed();

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_CheckifReadyToSubmit_Valid_All_But_Name_Should_Pass()
        {
            // Arrange
            var NameEntry = page.FindByName("NameEntry");
            ((Entry)NameEntry).Text = "";

            page.Name_onTextChange(null, null);

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_CheckifReadyToSubmit_Valid_All_But_Name_With_Whitespace_Should_Pass()
        {
            // Arrange
            var NameEntry = page.FindByName("NameEntry");
            ((Entry)NameEntry).Text = " ";

            page.Name_onTextChange(null, null);

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_CheckifReadyToSubmit_Valid_All_But_Description_With_Whitespace_Should_Pass()
        {
            // Arrange
            var DescriptionEntry = page.FindByName("DescriptionEntry");
            ((Entry)DescriptionEntry).Text = " ";

            page.DescriptionEntry_TextChanged(null, null);

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_CheckifReadyToSubmit_Valid_All_But_Description_Should_Pass()
        {
            // Arrange
            var DescriptionEntry = page.FindByName("DescriptionEntry");
            ((Entry)DescriptionEntry).Text = "";

            page.DescriptionEntry_TextChanged(null, null);

            // Act
            page.Save_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_RollDice_Clicked_Default_Should_Pass()
        {
            // Arrange
            page.ViewModel.Data = new MonsterModel();

            // Act
            page.Randomize_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_ClosePopup_Default_Should_Pass()
        {
            // Arrange

            // Act
            page.ClosePopup();

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_ClosePopup_Clicked_Default_Should_Pass()
        {
            // Arrange

            // Act
            page.ClosePopup_Clicked(null, null);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_ShowPopup_Default_Should_Pass()
        {
            // Arrange
            page.ViewModel.Data = new MonsterModel();

            // Act
            _ = page.ShowPopup(ItemLocationEnum.PrimaryHand);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_OnPopupItemSelected_Clicked_Default_Should_Pass()
        {
            // Arrange

            var data = new ItemModel();

            var selectedCharacterChangedEventArgs = new SelectedItemChangedEventArgs(data, 0);

            // Act
            page.OnPopupItemSelected(null, selectedCharacterChangedEventArgs);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_OnPopupItemSelected_Clicked_Null_Should_Fail()
        {
            // Arrange

            var selectedCharacterChangedEventArgs = new SelectedItemChangedEventArgs(null, 0);

            // Act
            page.OnPopupItemSelected(null, selectedCharacterChangedEventArgs);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Defense_OnStepperDefenseChanged_Default_Should_Pass()
        {
            // Arrange
            var data = new MonsterModel();
            var ViewModel = new GenericViewModel<MonsterModel>(data);

            page = new MonsterCreatePage(ViewModel);
            var oldDefense = 0.0;
            var newDefense = 1.0;

            var args = new ValueChangedEventArgs(oldDefense, newDefense);

            // Act
            page.DefenseSlider_ValueChanged(null, args);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Speed_ValueChanged_Default_Should_Pass()
        {
            // Arrange
            var data = new MonsterModel();
            var ViewModel = new GenericViewModel<MonsterModel>(data);

            page = new MonsterCreatePage(ViewModel);
            var oldSpeed = 0.0;
            var newSpeed = 1.0;

            var args = new ValueChangedEventArgs(oldSpeed, newSpeed);

            // Act
            page.SpeedSlider_ValueChanged(null, args);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Attack_ValueChanged_Default_Should_Pass()
        {
            // Arrange
            var data = new MonsterModel();
            var ViewModel = new GenericViewModel<MonsterModel>(data);

            page = new MonsterCreatePage(ViewModel);
            var oldAttack = 0.0;
            var newAttack = 1.0;

            var args = new ValueChangedEventArgs(oldAttack, newAttack);

            // Act
            page.AttackSlider_ValueChanged(null, args);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_Item_ShowPopup_Default_Should_Pass()
        {
            // Arrange

            var item = page.GetItemToDisplay(ItemLocationEnum.Head);

            // Act
            var itemButton = item.Children.FirstOrDefault(m => m.GetType().Name.Equals("Button"));

            _ = page.ShowPopup(ItemLocationEnum.Head);

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }

        [Test]
        public void MonsterCreatePage_GetItemToDisplay_Click_Button_Valid_Should_Pass()
        {
            // Arrange
            var item = ItemIndexViewModel.Instance.GetDefaultItem(ItemLocationEnum.Head);
            page.ViewModel.Data.Head = item.Id;
            var StackItem = page.GetItemToDisplay(ItemLocationEnum.Head);
            var dataImage = StackItem.Children[0];

            // Act
            ((ImageButton)dataImage).PropagateUpClicked();

            // Reset

            // Assert
            Assert.IsTrue(true); // Got to here, so it happened...
        }
    }
}