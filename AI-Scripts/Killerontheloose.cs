using System;
using System.Windows.Forms;
using GTA;

//Created by Nathan Binks (Foul Play), Version 1.1.0 TEST

namespace KillerSpawner
{
    public class Main : Script
    {
        //Our Ped
        Ped killer = null;

        public Main()
        {
            this.KeyUp += onKeyUp;
            this.Tick += onTick;
            Interval = 1;
        }

        private void onKeyUp(object sender, GTA.KeyEventArgs e)
        {
            if (e.Shift && e.Key == Keys.D0)
            {
                CreateKiller(); //Creates the killer
            }
            if (e.Shift && e.Key == Keys.D9)
            {
                if (killer != null && Exists(killer))
                {
                    killer.NoLongerNeeded();
                    killer = null;
                }
            }
        }

        private void onTick(object sender, EventArgs e)
        {
            //If the ped exsist
            if (killer != null && Exists(killer))
            {
                //if the killer is dead then make it no longer needed and null
                if (killer.isDead)
                {
                    killer.NoLongerNeeded();
                    killer = null;
                }
            }
        }

        void CreateKiller()
        {
            //Spawn a random ped
            killer = World.CreatePed(World.GetNextPositionOnPavement(new Vector3(Player.Character.Position.X, Player.Character.Position.Y, Player.Character.Position.Z)));
            killer.CurrentRoom = Player.Character.CurrentRoom; //So if the ped is spawned inside a building, it won't be invisible
            killer.BecomeMissionCharacter(); //Allow me to give it tasks
            killer.BlockGestures = true; //Don't be distracted
            killer.BlockPermanentEvents = true; //Don't be distracted
            killer.CanSwitchWeapons = true; //Allow the ped to switch weapons
            killer.WantedByPolice = true; //Make the police attack the ped
            killer.SetPathfinding(true, true, true); //Allows the ped to climb over objects, use ladders and drop from heights
            killer.AlwaysDiesOnLowHealth = true; //Always die when having low health
            killer.Weapons.DesertEagle.Ammo = 50;//Give them a weapon
            killer.Weapons.AssaultRifle_AK47.Ammo = 100;
           // killer.Weapons.Select(killer.Weapons.DesertEagle); //Select the weapon
            killer.StartKillingSpree(false); //Start the killing spree
        }
    }
}
