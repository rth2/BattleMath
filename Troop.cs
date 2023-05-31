namespace BattleMath
{
    public class Troop
    {
        private int tier, amount;
        private float attack, defense, health;
        float damageToApply = 0;
        bool hasTakenFullDamage = false;
        int[] woundedReport = new int[3];   //one for [slightly wounded, wounded, casualty]

        public Troop( int tier, float attack, float defense, float health, int amount)
        {
            this.tier = tier;
            this.attack = attack;
            this.defense = defense;
            this.health = health;
            this.amount = amount;
        }

        #region Getters
        public int GetTier() { return tier; }
        public float GetAttack() { return attack; }
        public float GetDefense() {  return defense; }
        public float GetHealth() { return health; }
        public int GetAmount() { return amount; }
        public float GetPower() { return attack * amount;}
        internal float GetTotalDefense() { return defense * amount; }
        internal float GetTotalHealth() { return health * amount; }
        internal bool GetHasTakenFullDamage() { return hasTakenFullDamage; }
        internal float GetDamageToApply() { return damageToApply; }
        internal int[] GetPossibleDead() { return CalculateLosses(); }
        #endregion

        #region Setters

        internal void SetHasTakenFullDamage(bool status) { hasTakenFullDamage = status; }

        #endregion
        //add to the amount of units in this stack
        public void AddAmount(int toAdd)
        {
            amount += toAdd;
        }

        internal void AdjustDamageToApply(float damage) { damageToApply += damage; }
        //used for overall army adjustments
        internal void AdjustAttack(float number, bool isAdditive)
        {
            if(isAdditive)
            {
                attack += number;
            }
            else
            {   //multiplicative
                attack *= number;
            }
            //every unit needs at least 1 attack
            if(attack < 1)
            {
                attack = 1;
            }
        }
        //used for overall army adjustments
        internal void AdjustDefense(float number, bool isAdditive)
        {
            if (isAdditive)
            {
                defense += number;
            }
            else
            {   //multiplicative
                defense *= number;
            }
            //every unit needs at least one defense
            if(defense < 1)
            {
                defense = 1;
            }
        }
        //used for overall army adjustments
        internal void AdjustHealth(float number, bool isAdditive)
        {
            if (isAdditive)
            {
                health += number;
            }
            else
            {   //multiplicative
                health *= number;
            }
            //every unit needs at least one health
            if(health < 1)
            {
                health = 1;
            }
        }
        //figure out how many units could potentiall die in this stack based on damage taken
        int[] CalculateLosses()
        {   
            int numDead = (int)Math.Floor(damageToApply / health);
            //figures a % for each wound type (slightly, wounded, dead)
            WoundedReportPercentages(); //puts % amount in wounded report array
            //converts the %'s into numbers based on the # that could potentially die
            WoundedReportPercentageToNumber(numDead);

            return woundedReport;//the wounded report for this troop
        }

        public void PrintDataToScreen()
        {
            Console.WriteLine($"Tier: {tier}, attack: {attack}, defense: {defense}, health: {health}, amount: {amount}, damage taken: {damageToApply}.");
            for (int i = 0; i < woundedReport.Length; i++)
            {
                Console.WriteLine($"Troop Numbers. Slight, Wounded, Casualty: {woundedReport[0]},{woundedReport[1]},{woundedReport[2]}");
            }
        }
        //give a # of possibly wounded, and get a percentage of wounded in each wounded category based on
        //      the % in that slot and rounding.
        void WoundedReportPercentageToNumber(int  wounded)
        {
            for(int i = 0; i < woundedReport.Length; i++)
            {
                if(woundedReport[i] > 0 )
                {
                    //woundedReport[i] = (int)Math.Floor(((float)wounded * ((float)woundedReport[i] / 100)));
                    woundedReport[i] = (int)Math.Round(((float)wounded * ((float)woundedReport[i] / 100)));
                }
                else
                {
                    woundedReport[i] = 0;
                }
            }
        }
        //This fills out the 3 slots in wounded report with a random %.
        //      These 3 numbers will add up to 100.
        void WoundedReportPercentages()
        {
            const int ExpectedSum = 100;    //out of 100%
            Random rnd = new Random();

            int sum = 0;    //keep a running tally
            for (int i = 0; i < woundedReport.Length - 1; i++)
            {
                woundedReport[i] = rnd.Next(ExpectedSum);
                sum += woundedReport[i];
            }
            int actualSum = sum * woundedReport.Length / (woundedReport.Length - 1);
            sum = 0;
            for (int i = 0; i < woundedReport.Length - 1; i++)
            {
                woundedReport[i] = woundedReport[i] * ExpectedSum / actualSum;
                sum += woundedReport[i];
            }
            woundedReport[woundedReport.Length - 1] = ExpectedSum - sum;

        }


    }
}
