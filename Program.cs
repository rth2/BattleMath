namespace BattleMath    //old style .NET5 and earlier with explicit namespace, class, main.
{

    internal class Program
    {
        static void Main()
        {   //Create a simulator for our battle.
            Battle battleSimulator = new();

            //Set up our armies. Attackers and Defenders
            Army attackingArmy = new();
            Army defendingArmy = new();

            //Setting up factions that will form the overall army. This would be multiple players, entities, whatever.
            Faction aFaction = new(true, 200, 200, 200, 220, 220, 210, 220, 220, 210, 220, 220, 210, 220, 220, 210,
                                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                            200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200,
                                            0, 0, "first");
            Faction aFactionB = new(false, 200, 200, 200, 220, 220, 210, 220, 220, 210, 220, 220, 210, 220, 220, 210,
                                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                            200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200,
                                            0, 0, "second");
            Faction aFactionC = new(false, 200, 200, 200, 220, 220, 210, 220, 220, 210, 220, 220, 210, 220, 220, 210,
                                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                            200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200, 200,
                                            0, 0, "third");

            //This would be an example of a forsaken faction
            Faction dFaction = new(true, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190, 190,
                                            0, 0, 0, 0, 0, 0, 0, 180, 180, 180,
                                            170, 180, 180, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170, 170,
                                            0, 0, "forsaken");
                                    

            //ATTACKING FACTION TROOPS
            //note: do not 'reuse' these stacks. Need to make new ones for each assignment.
            //This will likely be added from the array of troops that you already have
            Troop infTroop = new(0, 100, 500, 200, 50);
            Troop infTroopa = new(0, 100, 500, 200, 25);
            Troop infTroopb = new(0, 100, 500, 200, 10);
            Troop shooterA = new(3, 600, 400, 400, 5);

            //Add troops to factions by specifying the type and which troop. This would be the cycle through each array part.
            aFaction.AddTroopToStack(TroopStack.TroopType.Infantry, infTroop);
            aFaction.AddTroopToStack(TroopStack.TroopType.Shooter, shooterA);
            aFactionB.AddTroopToStack(TroopStack.TroopType.Infantry, infTroopa);
            aFactionC.AddTroopToStack(TroopStack.TroopType.Infantry, infTroopb);


            //DEFENDING FACTION TROOPS
            Troop cavTroopA = new(0, 500, 100, 200, 100);
            Troop artTroopA = new(1, 300, 200, 500, 38);

            dFaction.AddTroopToStack(TroopStack.TroopType.Cavalry, cavTroopA);
            dFaction.AddTroopToStack(TroopStack.TroopType.Artillery, artTroopA);

            //Assign factions to armies. Attacking or defending.
            attackingArmy.AddFaction(aFaction);
            attackingArmy.AddFaction(aFactionB);
            attackingArmy.AddFaction(aFactionC);

            defendingArmy.AddFaction(dFaction);
            

            //simulate the battle
            battleSimulator.SimulateBattle(attackingArmy, defendingArmy);
        }

    }
}
