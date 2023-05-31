using static BattleMath.TroopStack;

namespace BattleMath
{
    internal class Faction
    {
        /*
        * PASSED IN VALUES FROM TEAL
        * attacker attack, defense, health
        * infantry attack, defense, health
        * cavalry attack, defense, health
        * shooter attack, defense, health
        * artillery attack, defense, health
        * Rally attack, defense, health
        * Shelter Rally attack, defense, health
        * Building Rally attack, defense, health
        * defender attack, defense, health (only applied to our attack/defense/health when we are defending)
        * 
        * 
        * DEBUFFED APPLIED TO OPPONENTS
        * defender infantry attack, defense, health
        * defender cavalry attack, defense, health
        * defender shooter attack, defense, health
        * defender artillery attack, defense, health
        * defender rally attack
        */
        //BUFFS
        float factionAttack, factionDefense, factionHealth;                         //overall adjustment for entire faction
        float infantryAttack, infantryDefense, infantryHealth;                      //adjustments for faction infantry
        float cavalryAttack, cavalryDefense, cavalryHealth;                         //adjustments for faction cavalry
        float shooterAttack, shooterDefense, shooterHealth;                         //adjustments for faction shooters
        float artilleryAttack, artilleryDefense, artilleryHealth;                   //adjustments for faction artillery
        float rallyAttack, rallyDefense, rallyHealth;                               //only applied on rallies for shelters or buildings
        float shelterRallyAttack, shelterRallyHealth;                               //adjustments when rallying a shelter
        float buildingRallyAttack, buildingRallyHealth;                             //adjustments when rallying a building
        float defenderAttack, defenderDefense, defenderHealth;                      //applies when you are the defender
        //DEBUFFS
        float debuffAttack, debuffDefense, debuffHealth;                            //debuffs for opposing faction
        float debuffInfantryAttack, debuffInfantryDefense, debuffInfantryHealth;    //debuffs for opposing army infantry
        float debuffCavalryAttack, debuffCavalryDefense, debuffCavalryHealth;       //debuffs for opposing army cavalry
        float debuffShooterAttack, debuffShooterDefense, debuffShooterHealth;       //debuffs for opposing army shooters
        float debuffArtilleryAttack, debuffArtilleryDefense, debuffArtilleryHealth; //debuffs for opposing army artillery
        float debuffRallyAttack, debuffRallyHealth;                                 //debuff for opposing faction rallies

        bool isLeader = false;  //faction that is the main attacker/defender. If not the leader, faction is a reinforcement.

        string playerId = "";   //could pass in "forsaken" as playerId if this is a forsaken faction

        private List<TroopStack> troopStacks = new();

        //damage tracking and applying
        float damageToApply = 0;                //will apply this much damage throughout the faction
        bool hasTakenFullDamage = false;        //faction can take more damage if false
        bool isVictorious = false;

        BattleReport battleReport = new BattleReport();

        //Constructor. Lots of info for a faction
        public Faction(bool isLeader, float attack, float defense, float health,
                        float infAttack, float infDefense, float infHealth,
                        float cavAttack, float cavDefense, float cavHealth,
                        float shooterAttack, float shooterDefense, float shooterHealth,
                        float artAttack, float artDefense, float artHealth,
                        float rallyAttack, float rallyDefense, float rallyHealth,
                        float sheltRallyAttack, float sheltRallyHealth, 
                        float buildingRallyAttack, float buildingRallyHealth,
                        float defAttack, float defDefense, float defHealth,
                        float debuffAttack, float debuffDefense, float debuffHealth,
                        float debuffInfAttack, float debuffInfDefense, float debuffInfHealth,
                        float debuffCavAttack, float debuffCavDefense, float debuffCavHealth,
                        float debuffShootAttack, float debuffShootDefense, float debuffShootHealth,
                        float debuffArtAttack, float debuffArtDefense, float debuffArtHealth,
                        float debuffRallyAttack, float debuffRallyHealth, string playerId)
        {
            //main faction attributes
            this.isLeader = isLeader;
            factionAttack = attack;
            factionDefense = defense;
            factionHealth = health;
            //unit buffs
            infantryAttack = infAttack;
            infantryDefense = infDefense;
            infantryHealth = infHealth;
            cavalryAttack = cavAttack;
            cavalryDefense = cavDefense;
            cavalryHealth = cavHealth;
            this.shooterAttack = shooterAttack;
            this.shooterDefense = shooterDefense;
            this.shooterHealth = shooterHealth;
            artilleryAttack = artAttack;
            artilleryDefense = artDefense;
            artilleryHealth = artHealth;
            //rally buffs
            this.rallyAttack = rallyAttack;
            this.rallyDefense = rallyDefense;
            this.rallyHealth = rallyHealth;
            shelterRallyAttack = sheltRallyAttack;
            shelterRallyHealth = sheltRallyHealth;
            this.buildingRallyAttack = buildingRallyAttack;
            this.buildingRallyHealth = buildingRallyHealth;
            //defenders buffs
            defenderAttack = defAttack;
            defenderDefense = defDefense;
            defenderHealth = defHealth;
            //faction debuffs
            this.debuffAttack = debuffAttack;
            this.debuffDefense = debuffDefense;
            this.debuffHealth = debuffHealth;
            //unit debuffs
            debuffInfantryAttack = debuffInfAttack;
            debuffInfantryDefense = debuffInfDefense;
            debuffInfantryHealth = debuffInfHealth;
            debuffCavalryAttack = debuffCavAttack;
            debuffCavalryDefense = debuffCavDefense;
            debuffCavalryHealth = debuffCavHealth;
            debuffShooterAttack = debuffShootAttack;
            debuffShooterDefense = debuffShootDefense;
            debuffShooterHealth = debuffShootHealth;
            debuffArtilleryAttack = debuffArtAttack;
            debuffArtilleryDefense = debuffArtDefense;
            debuffArtilleryHealth = debuffArtHealth;
            //rally debuffs
            this.debuffRallyAttack = debuffRallyAttack;
            this.debuffRallyHealth = debuffRallyHealth;
            this.playerId = playerId;
        }

        #region List Manipulation
        //Adds a stack of troops of a certain troop type to this faction.
        //creates a new stack if one doesn't already exist
        public void AddTroopToStack(TroopStack.TroopType type, Troop troopInfo)
        {   //Currently have no stacks of troops
            if(troopStacks.Count == 0)
            {   //make a troop stack
                TroopStack newStack = new(type);
                newStack.AddTroop(troopInfo);
                troopStacks.Add(newStack);
                return;
            }

            //do any existing troop stack types exist
            for(int i = 0; i < troopStacks.Count; i++)
            {
                if (troopStacks[i].GetTroopType() == type)
                {
                    troopStacks[i].AddTroop(troopInfo);
                    return;
                }
            }
            //have stacks but none of this type
            TroopStack freshStack = new(type);
            freshStack.AddTroop(troopInfo);
            troopStacks.Add(freshStack);

        }

        #endregion

        #region Getters

        #region BUFFS
        //attack, defense, health overall modifier
        public float GetFactionAttack() { return factionAttack; }
        public float GetFactionDefense() { return factionDefense; }
        public float GetFactionHealth() { return factionHealth; }
        //infantry attack, defense health
        public float GetInfantryAttack() { return infantryAttack; }
        public float GetInfantryDefense() { return infantryDefense; }
        public float GetInfantryHealth() {  return infantryHealth; }
        //cavalry attack, defense health
        public float GetCavalryAttack() { return cavalryAttack; }
        public float GetCavalryDefense() { return cavalryDefense; }
        public float GetCavalryHealth() { return cavalryHealth; }
        //shooter attack, defense health
        public float GetShooterAttack() { return shooterAttack; }
        public float GetShooterDefense() { return shooterDefense; }
        public float GetShooterHealth() {  return shooterHealth; }
        //artillery attack, defense health
        public float GetArtilleryAttack() { return artilleryAttack; }
        public float GetArtilleryDefense() { return artilleryDefense; }
        public float GetArtilleryHealth() {  return artilleryHealth; }
        //rally
        public float GetRallyAttack() { return rallyAttack; }
        public float GetRallyDefense() { return rallyDefense; }
        public float GetRallyHealth() {  return rallyHealth; }
        //shelter
        public float GetShelterRallyAttack() { return shelterRallyAttack; }
        public float GetShelterRallyHealth() {  return shelterRallyHealth; }
        //building
        public float GetBuildingRallyAttack() { return buildingRallyAttack; }
        public float GetBuildingRallyHealth() { return buildingRallyHealth; }
        //defenders advantages
        public float GetDefenderAttack() { return defenderAttack; }
        public float GetDefenderDefense() { return defenderDefense; }
        public float GetDefenderHealth() {  return defenderHealth; }
        #endregion
        #region DEBUFFS
        public float GetDebuffAttack() { return debuffAttack; }
        public float GetDebuffDefense() { return debuffDefense; }
        public float GetDebuffHealth() {  return debuffHealth; }
        public float GetDebuffInfantryAttack() { return debuffInfantryAttack; }
        public float GetDebuffInfantryDefense() { return debuffInfantryDefense;}
        public float GetDebuffInfantryHealth() {  return debuffInfantryHealth; }
        public float GetDebuffCavalryAttack() { return debuffCavalryAttack; }
        public float GetDebuffCavalryDefense() { return debuffCavalryDefense;}
        public float GetDebuffCavalryHealth() { return debuffCavalryHealth; }
        public float GetDebuffShooterAttack() { return debuffShooterAttack; }
        public float GetDebuffShooterDefense() { return debuffShooterDefense; }
        public float GetDebuffShooterHealth() {  return debuffShooterHealth; }
        public float GetDebuffArtilleryAttack() { return debuffArtilleryAttack;}
        public float GetDebuffArtilleryDefense() { return debuffArtilleryDefense;}
        public float GetDebuffArtilleryHealth() { return debuffArtilleryHealth;}
        public float GetDebuffRallyAttack() { return debuffRallyAttack;}
        public float GetDebuffRallyHealth() { return debuffRallyHealth;}
        #endregion

        public bool GetIsLeader() { return isLeader; }
        internal string GetPlayerId() { return playerId; }
        internal bool GetHasTakenFullDamage() {  return hasTakenFullDamage; }

        public float GetPower( TroopStack.TroopType type)
        {
            float power = 0;
            for(int i = 0; i < troopStacks.Count; i++)
            {
                if (troopStacks[i].GetTroopType() == type)
                {
                    power += troopStacks[i].GetStackPower();
                }
            }
            return power;
        }

        internal float GetDefense(TroopStack.TroopType type)
        {
            float defense = 0;
            for(int i = 0; i < troopStacks.Count; ++i)
            {
                if (troopStacks[i].GetTroopType() == type)
                {
                    defense += troopStacks[i].GetStackDefense();
                }
            }
            return defense;
        }

        internal float GetHealth(TroopStack.TroopType type)
        {
            float health = 0;
            for (int i = 0; i < troopStacks.Count; ++i)
            {
                if (troopStacks[i].GetTroopType() == type)
                {
                    health += troopStacks[i].GetStackHealth();
                }
            }
            return health;
        }

        internal float GetFactionCombinedHealth()
        {
            float health = 0;
            health += GetHealth(TroopStack.TroopType.Infantry);
            health += GetHealth(TroopStack.TroopType.Cavalry);
            health += GetHealth(TroopStack.TroopType.Shooter);
            health += GetHealth(TroopStack.TroopType.Artillery);

            return health;
        }

        internal float GetDamageToApply() { return damageToApply; }

        #endregion

        #region Setters

        internal void SetHasTakenFullDamage(bool status) {  hasTakenFullDamage = status; }
        internal void SetVictory(bool isVictorious) { this.isVictorious = isVictorious; }

        #endregion

        internal void AdjustDamageToApply(float damage) { damageToApply += damage; }

        //distribute damage throughout all of the stacks of troops in this faction
        internal void DistributeDamage()
        {
            DistributeDamageToStacks(damageToApply);

            for(int i = 0; i <troopStacks.Count; ++i)
            {
                troopStacks[i].DistributeDamage();
            }
        }
        //divides the damage between all of the stacks
        void DistributeDamageToStacks(float damage)
        {
            int stacksToDamage = 0;

            for(int i = 0; i < troopStacks.Count; i++)
            {   
                if (!troopStacks[i].GetHasTakenFullDamage())
                {   //has not taken full damage
                    stacksToDamage++;
                }
            }
            //avoid dividing by zero
            if(stacksToDamage == 0) { Console.WriteLine($"don't divide by zero. DISTRIBUTE DAMAGE TO STACKS in FACTION.CS"); return; }

            float damagePerStack = damage / (float)stacksToDamage;  //split the damage
            float overkillDamage = 0;                               //track overkill amount

            for(int j = 0; j  < troopStacks.Count; j++)
            {
                if (troopStacks[j] != null)
                {
                    //stack can take damage
                    if (!troopStacks[j].GetHasTakenFullDamage())
                    {
                        float health = troopStacks[j].GetStackHealth();

                        if (damagePerStack > health)
                        {   //overkill damage
                            overkillDamage += damagePerStack - health;
                            troopStacks[j].AdjustDamageToApply(health);
                            troopStacks[j].SetHasTakenFullDamage(true);
                        }
                        else
                        {   //no overkill damage
                            troopStacks[j].AdjustDamageToApply(damagePerStack);
                        }
                    }
                }
            }
            //recursively distribute overkill damage
            if(overkillDamage > 0) {  DistributeDamageToStacks(overkillDamage); }

        }
        //returns a battle report for this faction
        internal BattleReport GetBattleReport()
        {
            battleReport.playerId = playerId;                   //set the player id
            battleReport.isVictorious = isVictorious;           //did this side win
            battleReport.troopCasualtyReports.Clear();          //clear the report list
            
            List<TroopCasualtyReport> report = new();           //temp list for adding to battle report
            //for each troop stack
            for(int i = 0; i < troopStacks.Count; i++)
            {
                report.Clear();                                 //clear the temp list
                report = troopStacks[i].GetCasualtyReports();   //get that stacks list

                for(int j = 0; j < report.Count; j++)
                {   //copy reports over
                    battleReport.troopCasualtyReports.Add(report[j]);
                }
            }

            return battleReport;
        }

        internal void PrintTroopStacks()
        {
            if (troopStacks.Count == 0)
            {
                Console.WriteLine($"No troops in faction!");
                return;
            }

            for (int i = 0; i < troopStacks.Count; i++)
            {
                Console.WriteLine($"{troopStacks[i].GetTroopType()}");
                troopStacks[i].PrintTroops();
            }
        }

        internal void PrintTroopStackDamage()
        {
            if (troopStacks.Count == 0)
            {
                Console.WriteLine($"No troops in faction!");
                return;
            }

            Console.WriteLine($"\nTroop stack damage");
            for (int i = 0; i < troopStacks.Count; i++)
            {
                Console.WriteLine($"{troopStacks[i].GetDamageToApply()}");
                //troopStacks[i].PrintTroops();
            }
        }

        #region MATHS
        internal void AdjustTroopStats(TroopStack.TroopType troopType, float attack, float defense, float health, bool isAdditive)
        {
            for(int i = 0; i  < troopStacks.Count; i++)
            {   //same troop type
                if (troopStacks[i].GetTroopType() == troopType)
                {
                    troopStacks[i].AdjustTroopStats(attack, defense, health, isAdditive);
                }
            }
        }

        internal void AdjustAllTroops(float attack, float defense, float health, bool isAdditive)
        {
            for (int i = 0; i < troopStacks.Count; i++)
            {
                troopStacks[i].AdjustTroopStats(attack, defense, health, isAdditive);
            }
        }
        //fills a boolean array based on what stacks of troops are in this faction
        internal void FillTroopBoolArray(bool[] troopTypeArray)
        {
            for(int i = 0; i < troopStacks.Count; i++)
            {
                switch(troopStacks[i].GetTroopType())
                {
                    case TroopType.Infantry:
                        troopTypeArray[0] = true;
                        break;
                    case TroopType.Shooter:
                        troopTypeArray[1] = true;
                        break;
                    case TroopType.Cavalry:
                        troopTypeArray[2] = true;
                        break;
                    case TroopType.Artillery:
                        troopTypeArray[3] = true;
                        break;
                }
                
            }
        }

        #endregion
    }

}
