using System.Collections.Generic;
using System;
using System.Linq;
using Game.Engine.EngineBase;
using Game.Engine.EngineInterfaces;
using Game.Engine.EngineModels;
using Game.Models;
using Game.GameRules;

namespace Game.Engine.EngineGame
{
    /// <summary>
    /// Manages the Rounds
    /// </summary>
    public class RoundEngine : RoundEngineBase, IRoundEngineInterface
    {
        // Hold the BaseEngine
        public new EngineSettingsModel EngineSettings = EngineSettingsModel.Instance;

     
        //Allows pseudo-random number generation
        public  Random RandomNumGenerator = new Random();

        // The Turn Engine
        public new ITurnEngineInterface Turn
        {
            get
            {
                if (base.Turn == null)
                {
                    base.Turn = new TurnEngine();
                }
                return base.Turn;
            }
            set { base.Turn = Turn; }
        }

        /// <summary>
        /// Clear the List between Rounds
        /// </summary>
        public override bool ClearLists()
        {
            EngineSettings.ItemPool.Clear();
            EngineSettings.MonsterList.Clear();
            return true;
        }

        /// <summary>
        /// Call to make a new set of monsters..
        /// </summary>
        public override bool NewRound()
        {
            // End the existing round
            _ = EndRound();

            // Remove Character Buffs
            _ = RemoveCharacterBuffs();

            // Make the BaseEngine.PlayerList
            _ = MakePlayerList();

            // Set Order for the Round
            _ = OrderPlayerListByTurnOrder();

            // Populate BaseEngine.MapModel with Characters and Monsters
            _ = EngineSettings.MapModel.PopulateMapModel(EngineSettings.PlayerList);

            // Update Score for the RoundCount
            EngineSettings.BattleScore.RoundCount++;

            return true;
        }

        /// <summary>
        /// Add Monsters to the Round
        /// 
        /// Because Monsters can be duplicated, will add 1, 2, 3 to their name
        ///   
        /*
            * Hint: 
            * I don't have crudi monsters yet so will add 6 new ones..
            * If you have crudi monsters, then pick from the list

            * Consdier how you will scale the monsters up to be appropriate for the characters to fight
            * 
            */
        /// </summary>
        /// <returns></returns>
        public override int AddMonstersToRound()
        {
            var targetLevel = 1;
            var totalLevels = 0;
            var partyCount = EngineSettings.CharacterList.Count;
            var roundCount = EngineSettings.BattleScore.RoundCount;

            for (int charIndex = 0; charIndex < partyCount; charIndex++)
            {
                totalLevels += EngineSettings.CharacterList[charIndex].Level;
            }

            if (partyCount > 0)
            {
                targetLevel = totalLevels / partyCount;
            }

            if (roundCount % 3 == 0 && roundCount != 0 && EngineSettings.BattleSettingsModel.BossesEnabled)
            {
                var data = RandomPlayerHelper.GetRandomBossMonster(targetLevel, EngineSettings.BattleSettingsModel.AllowMonsterItems);

                // Help identify which Monster it is
                data.Name += " " + EngineSettings.MonsterList.Count + 1;

                EngineSettings.MonsterList.Add(new PlayerInfoModel(data));
            }

            else
            {
                for (var i = EngineSettings.MonsterList.Count; i < EngineSettings.MaxNumberPartyMonsters; i++)
                {

                    //spreads the levels of the monsters out a bit creating a range of challenges
                    int randomLevel = targetLevel + RandomNumGenerator.Next(0, 2);

                    var data = RandomPlayerHelper.GetRandomMonster(randomLevel, EngineSettings.BattleSettingsModel.AllowMonsterItems);

                    // Help identify which Monster it is
                    data.Name += " " + EngineSettings.MonsterList.Count + 1;

                    EngineSettings.MonsterList.Add(new PlayerInfoModel(data));
                }
            }

            return EngineSettings.MonsterList.Count;
        }

        /// <summary>
        /// At the end of the round
        /// Clear the ItemModel List
        /// Clear the MonsterModel List
        /// </summary>
        public override bool EndRound()
        {
            // In Auto Battle this happens and the characters get their items, In manual mode need to do it manualy
            if (EngineSettings.BattleScore.AutoBattle)
            {
                _ = PickupItemsForAllCharacters();
            }

            // Reset Monster and Item Lists
            _ = ClearLists();

            return true;
        }

        /// <summary>
        /// For each character pickup the items
        /// </summary>
        public override bool PickupItemsForAllCharacters()
        {
            // In Auto Battle this happens and the characters get their items
            // When called manualy, make sure to do the character pickup before calling EndRound

            return true;
        }

        /// <summary>
        /// Manage Next Turn
        /// 
        /// Decides Who's Turn
        /// Remembers Current Player
        /// 
        /// Starts the Turn
        /// 
        /// </summary>
        public override RoundEnum RoundNextTurn()
        {
            // No characters, game is over..

            // Check if round is over

            // If in Auto Battle pick the next attacker

            // Do the turn..

            // No characters, game is over...
            if (EngineSettings.CharacterList.Count < 1)
            {
                // Game Over
                EngineSettings.RoundStateEnum = RoundEnum.GameOver;
                return EngineSettings.RoundStateEnum;
            }

            // Check if round is over
            if (EngineSettings.MonsterList.Count < 1)
            {
                // If over, New Round
                EngineSettings.RoundStateEnum = RoundEnum.NewRound;
                return RoundEnum.NewRound;
            }

            if (EngineSettings.BattleScore.AutoBattle)
            {
                // Decide Who gets next turn
                // Remember who just went...
                EngineSettings.CurrentAttacker = GetNextPlayerTurn();

                // Only Attack for now
                EngineSettings.CurrentAction = ActionEnum.Attack;
            }

            // Do the turn....
            _ = Turn.TakeTurn(EngineSettings.CurrentAttacker);

            EngineSettings.RoundStateEnum = RoundEnum.NextTurn;

            return EngineSettings.RoundStateEnum;
        }

        /// <summary>
        /// Get the Next Player to have a turn
        /// </summary>
        public override PlayerInfoModel GetNextPlayerTurn()
        {
            // Remove the Dead

            // Get Next Player

            // Remove the Dead
            _ = RemoveDeadPlayersFromList();

            // Get Next Player
            var PlayerCurrent = GetNextPlayerInList();

            return PlayerCurrent;
        }

        /// <summary>
        /// Remove Dead Players from the List
        /// </summary>
        public override List<PlayerInfoModel> RemoveDeadPlayersFromList()
        {
            EngineSettings.PlayerList = EngineSettings.PlayerList.Where(m => m.Alive == true).ToList();
            return EngineSettings.PlayerList;
        }

        /// <summary>
        /// Order the Players in Turn Sequence
        /// </summary>
        public override List<PlayerInfoModel> OrderPlayerListByTurnOrder()
        {
            EngineSettings.PlayerList = EngineSettings.PlayerList.OrderByDescending(a => a.GetSpeed())
                .ThenByDescending(a => a.Level)
                .ThenByDescending(a => a.ExperienceTotal)
                .ThenByDescending(a => a.PlayerType)
                .ThenBy(a => a.Name)
                .ThenBy(a => a.ListOrder)
                .ToList();

            return EngineSettings.PlayerList;
        }

        /// <summary>
        /// Who is Playing this round?
        /// </summary>
        public override List<PlayerInfoModel> MakePlayerList()
        {
            // Start from a clean list of players

            // Remember the Insert order, used for Sorting

            // Add the Characters

            // Add the Monsters

            // Start from a clean list of players
            EngineSettings.PlayerList.Clear();
            EngineSettings.MonsterList.Clear();

            // Remember the Insert order, used for Sorting
            var ListOrder = 0;

            foreach (var data in EngineSettings.CharacterList)
            {
                if (data.Alive)
                {
                    EngineSettings.PlayerList.Add(
                        new PlayerInfoModel(data)
                        {
                            // Remember the order
                            ListOrder = ListOrder
                        });

                    ListOrder++;
                }
            }

            AddMonstersToRound();

            EngineSettings.PlayerList.AddRange(EngineSettings.MonsterList);

            return EngineSettings.PlayerList;
        }

        /// <summary>
        /// Get the Information about the Player
        /// </summary>
        public override PlayerInfoModel GetNextPlayerInList()
        {
            // Walk the list from top to bottom
            // If Player is found, then see if next player exist, if so return that.
            // If not, return first player (looped)

            // If List is empty, return null
            if (EngineSettings.PlayerList.Count == 0)
            {
                return null;
            }

            // No current player, so set the first one
            if (EngineSettings.CurrentAttacker == null)
            {
                return EngineSettings.PlayerList.FirstOrDefault();
            }

            // Find current player in the list
            var index = EngineSettings.PlayerList.FindIndex(m => m.Guid.Equals(EngineSettings.CurrentAttacker.Guid));

            // If at the end of the list, return the first element
            if (index == EngineSettings.PlayerList.Count() - 1)
            {
                return EngineSettings.PlayerList.FirstOrDefault();
            }

            // Return the next element
            return EngineSettings.PlayerList[index + 1];
        }

        /// <summary>
        /// Pickup Items Dropped
        /// </summary>
        public override bool PickupItemsFromPool(PlayerInfoModel character)
        {
            // I use the same logic for Auto Battle as I do for Manual Battle

            //if (BaseEngine.BattleScore.AutoBattle)
            {
                // Have the character, walk the items in the pool, and decide if any are better than current one.

                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.Head);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.Necklass);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.PrimaryHand);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.OffHand);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.RightFinger);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.LeftFinger);
                _ = GetItemFromPoolIfBetter(character, ItemLocationEnum.Feet);
            }
            return true;
        }

        /// <summary>
        /// Swap out the item if it is better
        /// 
        /// Uses Value to determine
        /// </summary>
        public override bool GetItemFromPoolIfBetter(PlayerInfoModel character, ItemLocationEnum setLocation)
        {
            var thisLocation = setLocation;
            if (setLocation == ItemLocationEnum.RightFinger)
            {
                thisLocation = ItemLocationEnum.Finger;
            }

            if (setLocation == ItemLocationEnum.LeftFinger)
            {
                thisLocation = ItemLocationEnum.Finger;
            }

            var myList = EngineSettings.ItemPool.Where(a => a.Location == thisLocation)
                .OrderByDescending(a => a.Value)
                .ToList();

            // If no items in the list, return...
            if (!myList.Any())
            {
                return false;
            }

            var CharacterItem = character.GetItemByLocation(setLocation);
            if (CharacterItem == null)
            {
                _ = SwapCharacterItem(character, setLocation, myList.FirstOrDefault());
                return true;
            }

            foreach (var PoolItem in myList)
            {
                if (PoolItem.Value > CharacterItem.Value)
                {
                    _ = SwapCharacterItem(character, setLocation, PoolItem);
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Swap the Item the character has for one from the pool
        /// 
        /// Drop the current item back into the Pool
        /// </summary>
        public override ItemModel SwapCharacterItem(PlayerInfoModel character, ItemLocationEnum setLocation, ItemModel PoolItem)
        {
            // Put on the new ItemModel, which drops the one back to the pool
            var droppedItem = character.AddItem(setLocation, PoolItem.Id);

            // Add the PoolItem to the list of selected items
            EngineSettings.BattleScore.ItemModelSelectList.Add(PoolItem);

            // Remove the ItemModel just put on from the pool
            _ = EngineSettings.ItemPool.Remove(PoolItem);

            if (droppedItem != null)
            {
                // Add the dropped ItemModel to the pool
                EngineSettings.ItemPool.Add(droppedItem);
            }

            return droppedItem;
        }

        /// <summary>
        /// For all characters in player list, remove their buffs
        /// </summary>
        public override bool RemoveCharacterBuffs()
        {
            foreach (var data in EngineSettings.PlayerList)
            {
                _ = data.ClearBuffs();
            }

            foreach (var data in EngineSettings.CharacterList)
            {
                _ = data.ClearBuffs();
            }
            return true;
        }
    }
}