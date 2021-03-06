using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Game.Models;
using Game.ViewModels;
using System.Linq;

namespace Game.Views
{
    /// <summary>
    /// The Main Game Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewRoundPage : ContentPage
    {
        // This uses the Instance so it can be shared with other Battle Pages as needed
        public BattleEngineViewModel EngineViewModel = BattleEngineViewModel.Instance;

        /// <summary>
        /// Constructor
        /// </summary>
        public NewRoundPage()
        {

            int i = 0, j = 0;
            InitializeComponent();

            BindingContext = EngineViewModel;

            int roundCount = BattleEngineViewModel.Instance.Engine.EngineSettings.BattleScore.RoundCount;
            NewRoundContentPage.Title = "Prepare for Round " + roundCount.ToString() + "!";

            // Draw the Characters
            foreach (var data in BattleEngineViewModel.Instance.Engine.EngineSettings.PlayerList.Where(m => m.PlayerType == PlayerTypeEnum.Character).ToList())
            {
                PartyListFrame.Children.Add(CreatePlayerDisplayBox(data), i%3, j);
                i++;

                if (i == 3)
                {
                    j++;
                }
            }

            i = 0;

            // Draw the Monsters
            foreach (var data in EngineViewModel.Engine.EngineSettings.MonsterList)
            {           
                MonsterListFrame.Children.Add(CreatePlayerDisplayBox(data),i%3,j);
                i++;

                if (i == 3)
                {
                    j++;
                }
            }

        }

        /// <summary>
        /// Start next Round, returning to the battle screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void BeginButton_Clicked(object sender, EventArgs e)
        {
            _ = await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Return a stack layout with the Player information inside
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public StackLayout CreatePlayerDisplayBox(PlayerInfoModel data)
        {
            if (data == null)
            {
                data = new PlayerInfoModel();
            }

            // Hookup the image
            var PlayerImage = new Image
            {
                Style = (Style)Application.Current.Resources["ImageMediumStyle"],
                Source = data.ImageURI
            };

            // Add the Level
            var PlayerLevelAndJob = new Label
            {
                Text = "Lvl:" + data.Level + "/" + data.Job,
                Style = (Style)Application.Current.Resources["TinyTitleStyle"],
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 0,
                LineBreakMode = LineBreakMode.TailTruncation,
                CharacterSpacing = 0,
                LineHeight = 1,
                MaxLines = 2,
            };

            var PlayerNameLabel = new Label()
            {
                Text = data.Name,
                Style = (Style)Application.Current.Resources["TinyTitleStyle"],
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 0,
                LineBreakMode = LineBreakMode.TailTruncation,
                CharacterSpacing = 0,
                LineHeight = 1,
                MaxLines = 1,
            };

            if (data.MonsterJob != MonsterJobEnum.Unknown) 
            {
                PlayerLevelAndJob.Text = "Lvl:" + data.Level + "/" + data.MonsterJob;
            }

            var Stats = new Label()
            {
                Text = "Atk:" + data.Attack + " Spd:" + data.Speed + " Def:" + data.Defense,
                Style = (Style)Application.Current.Resources["TinyTitleStyle"],
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = 0,
                LineBreakMode = LineBreakMode.TailTruncation,
                CharacterSpacing = 0,
                LineHeight = 1,
                MaxLines = 2,
            };

            // Put the Image Button and Text inside a layout
            var PlayerStack = new StackLayout
            {
                Style = (Style)Application.Current.Resources["MonsterInfoBox"],
                Children = {
                    PlayerImage,
                    PlayerNameLabel,
                    PlayerLevelAndJob,
                    Stats
                },
            };

            return PlayerStack;
        }
    }
}