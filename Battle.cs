namespace BattleMath
{
    internal class Battle
    {
        //Create the overall attacking and defending armies. Tracks accumulated stats from all of the factions in the armies.
        OverallArmy attackersOverall = new();
        OverallArmy defendersOverall = new();
        //Battle reports that will generate for each army.
        List<BattleReport> attackersBattleReports = new();
        List<BattleReport> defendersBattleReports = new();

        /// <summary>
        /// Simulate a battle between two armies. The first is the attacking the second is the defending.
        ///     This could later be altered to take an army array if more than 2 armies fighting is desired.
        /// </summary>
        /// <param name="attackingArmy">initiating army</param>
        /// <param name="defendingArmy">army being attacked</param>
        internal void SimulateBattle(Army attackingArmy, Army defendingArmy)
        {
            //get the leaders from each army so we can use their fields
            Faction attackLeader = attackingArmy.GetLeader();
            Faction defenderLeader = defendingArmy.GetLeader();

            //Make the adjustments to the armies based on their leaders buffs / enemies debuffs
            AdjustArmyStats(attackingArmy, attackLeader, defendingArmy, defenderLeader, false);
            AdjustArmyStats(defendingArmy, defenderLeader, attackingArmy, attackLeader, true);


            //set base values
            SetOverallArmyValues(attackersOverall, attackingArmy);
            SetOverallArmyValues(defendersOverall, defendingArmy);
            //set total power values
            SetTotalPower(attackersOverall);
            SetTotalPower(defendersOverall);
            SetTotalDefense(attackersOverall);
            SetTotalDefense(defendersOverall);
            SetTotalHealth(attackersOverall);
            SetTotalHealth(defendersOverall);
            //Set overall damage taken by armies. Used to determine winners.
            attackersOverall.damageTaken = defendersOverall.totalPower - attackersOverall.totalDefense;
            defendersOverall.damageTaken = attackersOverall.totalPower - defendersOverall.totalDefense;

            //attacker has taken damage
            if(attackersOverall.damageTaken > 0 )
            {   //if damage greater than health, make equal to health of entire army
                if(attackersOverall.damageTaken > attackersOverall.totalHealth)
                {
                    attackersOverall.damageTaken = attackersOverall.totalHealth;
                }
                //distribute the damage
                DistributeDamage(attackingArmy, attackersOverall.damageTaken);
            }
            //defender damage and distribution
            if(defendersOverall.damageTaken > 0 )
            {
                if(defendersOverall.damageTaken > defendersOverall.totalHealth)
                {
                    defendersOverall.damageTaken = defendersOverall.totalHealth;
                }
                DistributeDamage(defendingArmy, defendersOverall.damageTaken);
            }

            //who won for battle report.
            if (attackersOverall.damageTaken < defendersOverall.damageTaken)
            {   //attacking army took less damage so they win
                attackingArmy.SetVictory(true);
                defendingArmy.SetVictory(false);
            }
            else
            {   //defenders win if they took less or equivalent damage to the attackers (defenders advantage)
                attackingArmy.SetVictory(false);
                defendingArmy.SetVictory(true);
            }
            //make sure it's a clean report. Only needed if multiple simulations ran in a row.
            attackersBattleReports.Clear();
            defendersBattleReports.Clear();

            //load up the battle reports.
            attackersBattleReports = attackingArmy.GetBattleReports();
            defendersBattleReports = defendingArmy.GetBattleReports();


            Console.WriteLine($"\noverall army attacker\n");
            attackersOverall.PrintValues();

            Console.WriteLine($"\noverall army defender\n");
            defendersOverall.PrintValues();
            /*
             * This prints out the battle reports to the screen.
             *      You may want to return data or whatever from here.
             */
            Console.WriteLine($"\nAttackers Battle Reports");
            for (int i = 0; i < attackersBattleReports.Count; i++)
            {
                attackersBattleReports[i].PrintReport();
            }

            Console.WriteLine($"\nDefenders Battle Reports");
            for (int i = 0; i < defendersBattleReports.Count; i++)
            {
                defendersBattleReports[i].PrintReport();
            }


        }

        //All logic for adjusting army stats are in here.
        private void AdjustArmyStats(Army armyToAdjust, Faction allyLeader, Army enemyArmy, Faction enemyLeader, bool isDefending)
        {
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Infantry, allyLeader.GetInfantryAttack(), allyLeader.GetInfantryDefense(), allyLeader.GetInfantryHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Shooter, allyLeader.GetShooterAttack(), allyLeader.GetShooterDefense(), allyLeader.GetShooterHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Cavalry, allyLeader.GetCavalryAttack(), allyLeader.GetCavalryDefense(), allyLeader.GetCavalryHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Artillery, allyLeader.GetArtilleryAttack(), allyLeader.GetArtilleryDefense(), allyLeader.GetArtilleryHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Infantry, -enemyLeader.GetDebuffInfantryAttack(), -enemyLeader.GetDebuffInfantryDefense(), -enemyLeader.GetDebuffInfantryHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Shooter, -enemyLeader.GetDebuffShooterAttack(), -enemyLeader.GetDebuffShooterDefense(), -enemyLeader.GetDebuffShooterHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Cavalry, -enemyLeader.GetDebuffCavalryAttack(), -enemyLeader.GetDebuffCavalryDefense(), -enemyLeader.GetDebuffCavalryHealth(), true);
            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Artillery, -enemyLeader.GetDebuffArtilleryAttack(), -enemyLeader.GetDebuffArtilleryDefense(), -enemyLeader.GetDebuffArtilleryHealth(), true);

            armyToAdjust.AdjustAllTroops(allyLeader.GetFactionAttack(), allyLeader.GetFactionDefense(), allyLeader.GetFactionHealth(), true);
            armyToAdjust.AdjustAllTroops(-enemyLeader.GetDebuffAttack(), -enemyLeader.GetDebuffDefense(), -enemyLeader.GetDebuffHealth(), true);


            if(isDefending)
            {
                armyToAdjust.AdjustAllTroops(allyLeader.GetDefenderAttack(), allyLeader.GetDefenderDefense(), allyLeader.GetDefenderHealth(), true);
            }

            //Adjusts for troops being attacked by troops they are weak against.
            //Doing last b/c it is multiplicative.
            AdjustForTroopWeaknesses(armyToAdjust, enemyArmy);
        }

        //Adjusts troops based on enemy composition. Does so multiplicatively.
        private void AdjustForTroopWeaknesses(Army armyToAdjust, Army enemyArmy)
        {   //finds out what type of troops the enemy army has.
            bool[] hasTroops = enemyArmy.GetArmyCompositionAsBoolArray();   //infantry, shooter, cavalry, artillery order.

            float weakToPercentage = 0.85f; //weaken by 15%
            /*
             * Type     :   Weak against
             * 
             * infantry :   artillery
             * shooter  :   infantry
             * cavalry  :   shooter
             * artillery:   cavalry
             */

            for (int i = 0; i < hasTroops.Length; i++)
            {
                if (hasTroops[i])
                {
                    switch (i)
                    {
                        case 0:     //weaken shooters in all respects multiplicatively as enemy has infantry
                            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Shooter, weakToPercentage, weakToPercentage, weakToPercentage, false);
                            break;
                        case 1:
                            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Cavalry, weakToPercentage, weakToPercentage, weakToPercentage, false);
                            break;
                        case 2:     //weaken infantry in all respects multiplicatively
                            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Artillery, weakToPercentage, weakToPercentage, weakToPercentage, false);
                            break;
                        case 3:
                            armyToAdjust.AdjustTroopStats(TroopStack.TroopType.Infantry, weakToPercentage, weakToPercentage, weakToPercentage, false);
                            break;
                    }
                }
            }
        }

        #region Setters
        private void SetOverallArmyValues(OverallArmy overallArmy, Army army)
        {   //set overall attack power
            overallArmy.infantryPower = army.GetTotalPower(TroopStack.TroopType.Infantry);
            overallArmy.cavalryPower = army.GetTotalPower(TroopStack.TroopType.Cavalry);
            overallArmy.shooterPower = army.GetTotalPower(TroopStack.TroopType.Shooter);
            overallArmy.artilleryPower = army.GetTotalPower(TroopStack.TroopType.Artillery);

            //set overall defense values
            overallArmy.infantryDefense = army.GetTotalDefense(TroopStack.TroopType.Infantry);
            overallArmy.cavalryDefense = army.GetTotalDefense(TroopStack.TroopType.Cavalry);
            overallArmy.shooterDefense = army.GetTotalDefense(TroopStack.TroopType.Shooter);
            overallArmy.artilleryDefense = army.GetTotalDefense(TroopStack.TroopType.Artillery);

            overallArmy.infantryHealth = army.GetTotalHealth(TroopStack.TroopType.Infantry);
            overallArmy.cavalryHealth = army.GetTotalHealth(TroopStack.TroopType.Cavalry);
            overallArmy.shooterHealth = army.GetTotalHealth(TroopStack.TroopType.Shooter);
            overallArmy.artilleryHealth = army.GetTotalHealth(TroopStack.TroopType.Artillery);

        }
        
        private void SetTotalPower(OverallArmy overallArmy)
        {
            float total = 0;

            total += overallArmy.infantryPower;
            total += overallArmy.cavalryPower;
            total += overallArmy.shooterPower;
            total += overallArmy.artilleryPower;

            overallArmy.totalPower = total;
        }

        private void SetTotalDefense(OverallArmy overallArmy)
        {
            float total = 0;

            total += overallArmy.infantryDefense;
            total += overallArmy.cavalryDefense;
            total += overallArmy.shooterDefense;
            total += overallArmy.artilleryDefense;

            overallArmy.totalDefense = total;
        }

        public void SetTotalHealth(OverallArmy overallArmy)
        {
            float total = 0;

            total += overallArmy.infantryHealth;
            total += overallArmy.cavalryHealth;
            total += overallArmy.shooterHealth;
            total += overallArmy.artilleryHealth;

            overallArmy.totalHealth = total;
        }
        #endregion
        //Distributes damage amongst an army
        private void DistributeDamage(Army army, float damageTaken)
        {
            army.DistributeDamage(damageTaken);
        }

        private void PrintValues(OverallArmy army)
        {
            Console.WriteLine($"\nOVERALL ARMY STATS");
            Console.WriteLine($"\nInfantry power: {army.infantryPower}");
            Console.WriteLine($"Cavalry power: {army.cavalryPower}");
            Console.WriteLine($"Shooter power: {army.shooterPower}");
            Console.WriteLine($"Artillery power: {army.artilleryPower}");
            Console.WriteLine($"\nInfantry defense: {army.infantryDefense}");
            Console.WriteLine($"Cavalry defense: {army.cavalryDefense}");
            Console.WriteLine($"Shooter defense: {army.shooterDefense}");
            Console.WriteLine($"Artillery defense: {army.artilleryDefense}");
            Console.WriteLine($"\nTotal Power: {army.totalPower}");
            Console.WriteLine($"Total Defense: {army.totalDefense}");
            Console.WriteLine($"TotalHealth: {army.totalHealth}");
        }
    }
    /// <summary>
    /// Helper Class for 
    /// </summary>
    internal class OverallArmy
    {
        //infantry
        public float infantryPower;
        public float infantryDefense;
        public float infantryHealth;
        //cavalry
        public float cavalryPower;
        public float cavalryDefense;
        public float cavalryHealth;
        //shooter
        public float shooterPower;
        public float shooterDefense;
        public float shooterHealth;
        //artillery
        public float artilleryPower;
        public float artilleryDefense;
        public float artilleryHealth;
        //damage fields
        public float totalPower;
        public float totalDefense;
        public float totalHealth;
        public float damageTaken;

        internal void PrintValues()
        {
            Console.WriteLine($"Infantry Power: {infantryPower}");
            Console.WriteLine($"Infantry defense: {infantryDefense}");
            Console.WriteLine($"Infantry health: {infantryHealth}");
            Console.WriteLine($"artillery Power: {artilleryPower}");
            Console.WriteLine($"artillery defense: {artilleryDefense}");
            Console.WriteLine($"artillery health: {artilleryHealth}");
            Console.WriteLine($"Shooter Power: {shooterPower}");
            Console.WriteLine($"Shooter defense: {shooterDefense}");
            Console.WriteLine($"Shooter health: {shooterHealth}");
            Console.WriteLine($"Cavalry Power: {cavalryPower}");
            Console.WriteLine($"Cavalry defense: {cavalryDefense}");
            Console.WriteLine($"Cavalry health: {cavalryHealth}");
            Console.WriteLine($"Total Power: {totalPower}");
            Console.WriteLine($"Total defense: {totalDefense}");
            Console.WriteLine($"Total health: {totalHealth}");
            Console.WriteLine($"Total damage taken: {damageTaken}");
        }
    }
    //helper class for reporting troop casualties
    internal class TroopCasualtyReport
    {
        public string troopType = "";
        public int troopTier;
        public int totalSent;
        public int numUnharmed;
        public int numSlightlyWounded;
        public int numWounded;
        public int numDead;
        
    }
    //helper class for putting together a Battle Report
    internal class BattleReport
    {
        public string playerId = "";
        public bool isVictorious;
        public List<TroopCasualtyReport> troopCasualtyReports = new();

        internal void PrintReport()
        {
            Console.WriteLine($"\nReport for ID: {playerId}");
            Console.WriteLine($"Is victorious: {isVictorious}");
            for(int i = 0; i < troopCasualtyReports.Count; i++)
            {
                Console.WriteLine($"Type: {troopCasualtyReports[i].troopType}");
                Console.WriteLine($"Tier: {troopCasualtyReports[i].troopTier}");
                Console.WriteLine($"Sent: {troopCasualtyReports[i].totalSent}");
                Console.WriteLine($"Unharmed: {troopCasualtyReports[i].numUnharmed}");
                Console.WriteLine($"Slightly Wounded: {troopCasualtyReports[i].numSlightlyWounded}");
                Console.WriteLine($"Wounded: {troopCasualtyReports[i].numWounded}");
                Console.WriteLine($"Dead: {troopCasualtyReports[i].numDead}\n");
            }
        }
    }

}
