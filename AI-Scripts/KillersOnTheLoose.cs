using System;
using System.Windows.Forms;
using GTA;

//Created by Nathan Binks (Foul Play), Version 1.6.0 TEST

namespace KillerSpawner
{
    public class Main : Script
    {
        Ped[] killers; //Array to store are killers
        Group KillersGroup; //The group that they will be in
        Random random; //Create a random numbers for their weapons
        public Main()
        {
            this.KeyUp += onKeyUp;
            this.Tick += onTick;
            Interval = 250; //Run for 250 Milliseconds
            killers = new Ped[8]; //Make 8 slots for our killers
            random = new Random(); //Random Number Generator
        }

        void GetPoliceToAttackKillers()
        {
            //If the killers exsist the run this code
            if (KillersExist())
            {
                //Get all peds in the world
                Ped[] ped = World.GetAllPeds();
                foreach (Ped p in ped)
                {
                    //If the ped Exsist and is a cop then run to postion around the leader
                    if (Game.Exists(p) && p.RelationshipGroup == RelationshipGroup.Cop && p.PedType == PedType.Cop && !p.isInVehicle()) p.Task.RunTo(KillersGroup.Leader.Position);
                    //If the ped Exsist and is a cop then run to postion around the leader
                    if (Game.Exists(p) && p.RelationshipGroup == RelationshipGroup.Cop && p.PedType == PedType.Cop && p.isInVehicle())
                    {
                        Ped tmpdriver = p.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver); //Get the driver
                        if (Game.Exists(tmpdriver) && tmpdriver.isInVehicle() && tmpdriver.CurrentVehicle.Model == "POLICE" || tmpdriver.CurrentVehicle.Model == "POLICE2" || tmpdriver.CurrentVehicle.Model == "NOOSE")
                        {
                            tmpdriver.Task.DriveTo(KillersGroup.Leader.Position, 15f, false);
                            tmpdriver.CurrentVehicle.SirenActive = true;
                        }
                        else if (tmpdriver.isInVehicle()) tmpdriver.Task.DriveTo(KillersGroup.Leader.Position, 15f, false);
                    }
                }
            }
        }

        //Checks if the kilers exist
        bool KillersExist()
        {
            //for each ped in killers if they exist then return true
            foreach (Ped ped in killers)
            {
                if (Game.Exists(ped)) return true;
            }
            return false;
        }

        //This deletes the killers
        void DeleteKillers()
        {   
            //If they exist then run for each ped in the killers array then delete them
            if (KillersExist())
            {
                foreach (Ped ped in killers)
                {
                    if (Game.Exists(ped))
                    {
                        ped.isRequiredForMission = false;
                        ped.NoLongerNeeded();
                        ped.Delete();
                    }
                }
            }
        }

        //This creates the killers.
        void CreateKillers()
        {
            if (KillersExist()) return; //If they already exist then don't do anything

            //Loop 8 times
            for (int i = 0; i < 3; i++)
            {
                //Try and spawn 8 killers if not then delete the spawned ones and stop the loop
                try
                {
                    killers[i] = World.CreatePed(Player.Character.Position.Around(5f).ToGround());
                }
                catch
                {
                    Game.DisplayText("CreateKillers() Failed! Ped " + (i + 1).ToString());
                    DeleteKillers();
                    return;
                }

                if (!Game.Exists(killers[i])) return; //If the killer doesn't exist then stop the loop

                //If i is 0 then run this code
                if (i == 0)
                {
                    KillersGroup = new Group(killers[i]);
                    KillersGroup.SeparationRange = 50f; //How far before they seperate
                    KillersGroup.FollowStatus = 3;
                }
                else KillersGroup.AddMember(killers[i]);

                killers[i].isRequiredForMission = true; //Allows me to give them tasks
                killers[i].BecomeMissionCharacter(); //Allows me to give them tasks
                killers[i].BlockGestures = true;
                killers[i].BlockPermanentEvents = true;
                killers[i].WantedByPolice = true;
                killers[i].RelationshipGroup = RelationshipGroup.Player;
                killers[i].ChangeRelationship(RelationshipGroup.Cop, Relationship.Hate);
                //killers[i].StartKillingSpree(false);
                killers[i].Task.AlwaysKeepTask = true;
                GiveWeapons(killers[i]);
            }
        }

        void GiveWeapons(Ped ped)
        {
            if (!Game.Exists(ped)) return; //If the killer doesn't exist then do nothing
            switch (random.Next(0, 4)) //Get a random number from 0 to 4
            {
                case (0): ped.Weapons.DesertEagle.Ammo = 500; break; //If the number is 0 then give the killer a desert eagle
                case (1): ped.Weapons.Glock.Ammo = 500; break; //If the number is 1 then give the killer a glock
                case (2): ped.Weapons.BasicShotgun.Ammo = 500; break; //If the number is 2 then give the killer a shotgun
                case (3): ped.Weapons.AssaultRifle_AK47.Ammo = 500; break; //If the number is 0 then give the killer a usi
                case (4): ped.Weapons.Uzi.Ammo = 500; break; //If the number is 0 then give the killer a usi
            }
        }

        private void onKeyUp(object sender, GTA.KeyEventArgs e)
        {
            if (e.Shift && e.Key == Keys.D0)
            {
                CreateKillers();
            }
            if (e.Shift && e.Key == Keys.D9)
            {
                DeleteKillers();
            }
            if (e.Shift && e.Key == Keys.D8)
            {
                GetPoliceToAttackKillers();
            }
        }
        private void onTick(object sender, EventArgs e)
        {
            if (KillersExist())
            {
                foreach(Ped ped in killers)
                {
                    if (Game.Exists(ped))
                    {
                        if (!ped.isAliveAndWell && KillersGroup.isLeader(ped))
                        {
                            KillersGroup.RemoveMember(ped);
                            ped.isRequiredForMission = false;
                            ped.NoLongerNeeded();
                            ped.Delete();
                        }
                        if (!ped.isAliveAndWell)
                        {
                            KillersGroup.RemoveMember(ped);
                            ped.isRequiredForMission = false;
                            ped.NoLongerNeeded();
                            ped.Delete();
                        }
                    }
                }
            }
        }
    }
}
