namespace BattleMath
{
    internal class TroopStack
    {
        public enum TroopType
        {
            Infantry = 0,
            Shooter,
            Cavalry,
            Artillery
        }

        private TroopType troopType;

        private Troop[] stack = new Troop[10]; //tiers 0-9. All start as null.
        private List<TroopCasualtyReport> casualtyReports = new();
        bool hasTakenFullDamage = false;
        float damageToApply = 0;

        public TroopStack(TroopType type)
        {
            troopType = type;

        }

        #region Getters

        public TroopType GetTroopType() {  return troopType; }
        internal bool GetHasTakenFullDamage() {  return hasTakenFullDamage; }
        internal float GetDamageToApply() {  return damageToApply; }

        public float GetStackPower()
        {
            float power = 0;
            for (int i = 0; i < stack.Length; i++)
            {
                if (stack[i] != null)
                {
                    power += stack[i].GetPower();
                }
            }
            return power;
        }

        internal float GetStackDefense()
        {
            float defense = 0;
            for (int i = 0; i < stack.Length; ++i)
            {
                if (stack[i] != null)
                {
                    defense += stack[i].GetTotalDefense();
                }
            }
            return defense;
        }

        internal float GetStackHealth()
        {
            float health = 0;
            for (int i = 0; i < stack.Length; ++i)
            {
                if (stack[i] != null)
                {
                    health += stack[i].GetTotalHealth();
                }
            }
            return health;
        }


        #endregion

        #region Setters

        internal void SetHasTakenFullDamage(bool status) {  hasTakenFullDamage = status;}

        #endregion

        #region List Manipulation
        public void AddTroop(Troop troop)
        {
            //look for the correct Tier, if can't find then add
            int index = troop.GetTier();

            if (stack[index] == null)
            {   //this tier doesn't exist yet. So make it.
                stack[index] = troop;
            }
            else
            {   //tier exists. So add to it.
                stack[index].AddAmount(troop.GetAmount());
            }

        }

        #endregion

        internal void AdjustDamageToApply( float amount) {  damageToApply += amount; }
        //distributes damage to troops
        internal void DistributeDamage()
        {

            DistributeDamageToTroops(damageToApply);

        }
        //recursively distributes damage amongst all of the troops in this stack
        void DistributeDamageToTroops(float damage)
        {
            int troopsToDamage = 0;

            for(int i = 0; i < stack.Length; i++)
            {   //that troop exists
                if (stack[i] != null)
                {   //it can take more damage
                    if (!stack[i].GetHasTakenFullDamage())
                    {
                        troopsToDamage++;
                    }
                }
            }

            if (troopsToDamage == 0) { Console.WriteLine($"don't divide by zero. DISTRIBUTE DAMAGE TO TROOPS in TROOPSTACK.CS"); return; };

            float damagePerTroop = damage / (float)troopsToDamage;
            float overkillDamage = 0;

            for(int i = 0; i < stack.Length; ++i)
            {
                if (stack[i] != null)
                {
                    //troop can take damage
                    if (!stack[i].GetHasTakenFullDamage())
                    {
                        float health = stack[i].GetTotalHealth();   //health of that troop

                        if(damagePerTroop > health)
                        {   //overkill damage
                            overkillDamage += damagePerTroop - health;
                            stack[i].AdjustDamageToApply(health);
                            stack[i].SetHasTakenFullDamage(true);
                        }
                        else
                        {   //no overkill damage
                            stack[i].AdjustDamageToApply(damagePerTroop);
                        }
                    }
                }
            }
            //recursively distribute overkill damage
            if(overkillDamage > 0) { DistributeDamageToTroops(overkillDamage); }

        }
        //gets a casualty report from each troop in this stack and puts it in a list
        internal List<TroopCasualtyReport> GetCasualtyReports()
        {
            casualtyReports.Clear();
            for(int i = 0; i < stack.Length; ++i)
            {
                if (stack[i] != null)
                {
                    TroopCasualtyReport report = new();
                    report.troopType = $"{troopType}";
                    report.troopTier = stack[i].GetTier();

                    //This should always return an array of size 3
                    int[] woundedReport = stack[i].GetPossibleDead();

                    report.numSlightlyWounded = woundedReport[0];
                    report.numWounded = woundedReport[1];
                    report.numDead = woundedReport[2];

                    report.totalSent = stack[i].GetAmount();
                    report.numUnharmed = report.totalSent - report.numSlightlyWounded - report.numWounded - report.numDead;
                    //sometimes rounding can cause an extra troop to count as dead. this makes it so you don't have negative unharmed.
                    if(report.numUnharmed < 0) { report.numUnharmed = 0; }

                    casualtyReports.Add(report);
                }
            }

            return casualtyReports;
        }

        public void PrintTroops()
        {
            for(int i = 0; i < stack.Length; i++)
            {
                if (stack[i] != null)
                {
                    stack[i].PrintDataToScreen();
                }
            }
        }

        #region MATHS
        internal void AdjustTroopStats(float attack, float defense, float health, bool isAdditive)
        {
            for(int i = 0; i < stack.Length; ++i)
            {
                if (stack[i] != null)
                {
                    stack[i].AdjustAttack(attack, isAdditive);
                    stack[i].AdjustDefense(defense, isAdditive);
                    stack[i].AdjustHealth(health, isAdditive);
                }
            }
        }

        #endregion
    }
}
