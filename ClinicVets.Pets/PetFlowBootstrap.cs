using System.Windows.Forms;
using ClinicVets;
using ClinicVets.UI;

namespace ClinicVets.Pets
{
    /// <summary>Registers pet UI flows for the main ClinicVets application.</summary>
    public static class PetFlowBootstrap
    {
        public static void Register()
        {
            PetFlowGateway.OpenViewPets = (owner, ownerId) =>
            {
                PetFormHost.ShowAllPets(owner);
            };

            PetFlowGateway.OpenPetManagementForOwner = (owner, ownerId) =>
            {
                PetFormHost.ShowPetManagement(owner, ownerId, vetWorkflow: false);
            };

            PetFlowGateway.OpenVetPetManagement = owner =>
            {
                PetFormHost.ShowPetManagement(owner, ownerId: null, vetWorkflow: true);
            };

            PetNavigationHooks.OpenVisitForPet = (owner, petId) =>
            {
                VetVisitsLauncher.TryLaunchVisitsApplication(owner, petId);
            };

            PetNavigationHooks.OpenMedicinesPharmacy = owner =>
            {
                VetVisitsLauncher.TryLaunchMedicinesApplication(owner);
            };
        }
    }
}
