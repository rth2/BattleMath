namespace BattleMath
{
    internal class Army
    {

        private List<Faction> factionsList = new();         //all of the factions in the army
        private List<BattleReport> battleReports = new();   //each factions battle report

        public Army() { }


        #region List Manipulation
        public void AddFaction(Faction faction)
        {
            //May need to add the faction, if it's the leader set a bunch of fields, and if reinforcements add troops to the correct stacks
            factionsList.Add(faction);
        }

        #endregion

        #region Getters
        public float GetTotalPower( TroopStack.TroopType type)
        {
            float power = 0;    //total power
            for(int i = 0; i < factionsList.Count; i++)
            {
                power += factionsList[i].GetPower(type);    //get total power from requested troop stack
            }
            return power;   //total power from the stack. Will be 0 if no stack.
        }

        public float GetTotalDefense(TroopStack.TroopType type)
        {
            float defense = 0;  //total defense
            for(int i = 0; i  < factionsList.Count; i++)
            {
                defense += factionsList[i].GetDefense(type);    //defense total from requested troop stack
            }
            return defense; //total defense from the stack. will be 0 if no stack.
        }

        internal float GetTotalHealth(TroopStack.TroopType type)
        {
            float health = 0;  //total defense
            for (int i = 0; i < factionsList.Count; i++)
            {
                health += factionsList[i].GetHealth(type);    //defense total from requested troop stack
            }
            return health; //total defense from the stack. will be 0 if no stack.
        }

        internal Faction GetLeader()
        {
            for(int i = 0; i < factionsList.Count; i++)
            {
                if (factionsList[i].GetIsLeader())
                {
                    return factionsList[i]; //return the leader faction
                }
            }
            //May want to throw a warning here. There should always be a leader, even for forsaken.
            return null;    //no leader was found
        }
        //Gets battle reports from all of the factions and returns them in it's own list.
        internal List<BattleReport> GetBattleReports()
        {
            battleReports.Clear();

            for (int i = 0; i < factionsList.Count; i++)
            {
                battleReports.Add(factionsList[i].GetBattleReport());
            }

            return battleReports;
        }

        #endregion

        #region Setters

        internal void SetVictory(bool isVictorious)
        {
            for(int i = 0; i < factionsList.Count;i++)
            {
                factionsList[i].SetVictory(isVictorious);
            }
        }
        #endregion
        //distribute damage amongst all of the factions
        internal void DistributeDamage(float totalDamageTaken)
        {
            if(factionsList.Count == 0) { Console.WriteLine($"no factions in DistributeDamage function!"); return; }

            DistributeDamageToFactions(totalDamageTaken);   //factions now have their damage distributed AMONGST the factions
            
            
            //factions distribute their damage to the troop stacks within the faction
            for(int i = 0; i < factionsList.Count; i++)
            {
                factionsList[i].DistributeDamage();
            }

        }
        /// <summary>
        /// Recursively distributes damage amongst all of the factions.
        ///     Avoids overkilling any one faction by redistributing overkill damage among remaining factions.
        /// </summary>
        /// <param name="damageToDistribute"></param>
        void DistributeDamageToFactions(float damageToDistribute)
        {
            int factionsToDamage = 0;

            for(int i = 0; i < factionsList.Count; i++)
            {   //faction can still be damaged
                if (!factionsList[i].GetHasTakenFullDamage())
                {   //add to the count
                    factionsToDamage++;
                }
            }

            if(factionsToDamage == 0) { Console.WriteLine($"don't divide by zero. DISTRIBUTE DAMAGE TO FACTIONS in ARMY.CS"); return; }

            float damagePerFaction = damageToDistribute / (float)factionsToDamage;  //split damage between all available factions
            float overkillDamage = 0;   //track so we can recurse if we have  extra damage to distribute

            for(int j = 0; j < factionsList.Count; j++)
            {
                //faction can be damaged
                if (!factionsList[j].GetHasTakenFullDamage())
                {   //factions total health
                    float health = factionsList[j].GetFactionCombinedHealth();
                    
                    if(damagePerFaction > health)
                    {   //overkill damage
                        overkillDamage += damagePerFaction - health;
                        factionsList[j].AdjustDamageToApply(health);    //apply the entire stack of damage
                        factionsList[j].SetHasTakenFullDamage(true);    //faction can't take anymore damage
                    }
                    else
                    {   //no overkill damage
                        factionsList[j].AdjustDamageToApply(damagePerFaction);
                    }
                }
            }
            //if we have overkill damage to distribute, then do so recursively
            if (overkillDamage > 0) { DistributeDamageToFactions(overkillDamage); }

            //we are now done distributing damage to factions
        }

        #region Math Adjustments

        internal void AdjustTroopStats(TroopStack.TroopType troopType, float attack, float defense, float health, bool isAdditive)
        {
            for(int i = 0; i  < factionsList.Count; i++)
            {
                factionsList[i].AdjustTroopStats(troopType, attack, defense, health, isAdditive);
            }
        }

        internal void AdjustAllTroops(float attack, float defense, float health, bool isAdditive)
        {
            for(int i = 0; i < factionsList.Count; i++)
            {
                factionsList[i].AdjustAllTroops(attack, defense, health, isAdditive);
            }
        }

        #endregion

        internal void PrintDetails()
        {
            for(int i = 0; i < factionsList.Count; i++)
            {
                Console.WriteLine($"\nFaction at index {i}");
                Console.WriteLine($"Faction is the battle leader? {factionsList[i].GetIsLeader()}");
                factionsList[i].PrintTroopStacks();
            }
        }

        internal void PrintBattleReport()
        {
            for(int i = 0; i < factionsList.Count; i++)
            {
                Console.WriteLine($"Damage to apply to faction {factionsList[i].GetPlayerId()}: {factionsList[i].GetDamageToApply()}");
                factionsList[i].PrintTroopStacks();

            }
        }
        //used to find the army composition and return it
        //  as an array with yes/no in the order: [infantry, shooter, cavalry, artillery]
        internal bool[] GetArmyCompositionAsBoolArray()
        {
            bool[] troopTypeArray = new bool[4];    //infantry, shooter, cavalry, artillery

            for(int i = 0; i < factionsList.Count;i++)
            {
                bool filled = true;

                for(int j = 0; j < troopTypeArray.Length; j++)
                {   //does not have this troop type
                    if(!troopTypeArray[j])
                    {
                        filled = false;
                        break;
                    }
                }

                if(filled)
                {
                    return troopTypeArray;
                }

                factionsList[i].FillTroopBoolArray(troopTypeArray);
            }

            return troopTypeArray;
        }
    }

}
