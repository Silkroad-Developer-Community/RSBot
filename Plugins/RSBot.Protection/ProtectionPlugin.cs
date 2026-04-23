using RSBot.Core.Plugins;
using RSBot.Protection.Components.Pet;
using RSBot.Protection.Components.Player;
using RSBot.Protection.Components.Town;

namespace RSBot.Protection
{
    public class ProtectionPlugin : IPlugin
    {
        public string InternalName => "RSBot.Protection";
        public static ProtectionPlugin Instance { get; private set; }
        public ProtectionManager Manager { get; private set; }
        public void Initialize()
        {
            Instance = this;
            Manager = new ProtectionManager();

            //Player handlers
            HealthManaRecoveryHandler.Initialize();
            UniversalPillHandler.Initialize();
            VigorRecoveryHandler.Initialize();
            StatPointsHandler.Initialize();

            //Pet handlers
            CosHealthRecoveryHandler.Initialize();
            CosHGPRecoveryHandler.Initiliaze();
            CosBadStatusHandler.Initialize();
            CosReviveHandler.Initialize();
            AutoSummonAttackPet.Initialize();

            //Back town
            DeadHandler.Initialize();
            AmmunitionHandler.Initialize();
            InventoryFullHandler.Initialize();
            PetInventoryFullHandler.Initialize();
            NoManaPotionsHandler.Initialize();
            NoHealthPotionsHandler.Initialize();
            LevelUpHandler.Initialize();
            DurabilityLowHandler.Initialize();
            FatigueHandler.Initialize();
        }
        public void OnLoadCharacter() { }
    }
}
