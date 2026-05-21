using System.Windows.Forms;
using ClinicVets.UI;

namespace ClinicVets
{
    /// <summary>Registers pet and visit navigation when ClinicVets starts.</summary>
    internal static class PetFlowBootstrap
    {
        internal static void Register()
        {
            PetFlowGateway.OpenViewPets = (owner, ownerId) =>
            {
                using (var form = new AllPetsForm(ownerId))
                {
                    form.ShowDialog(owner);
                }
            };

            PetFlowGateway.OpenPetManagementForOwner = (owner, ownerId) =>
            {
                NavigationHelper.OpenSecretaryPetManagement(owner, ownerId);
            };

            PetFlowGateway.PromptAddPetAfterCustomer = (owner, customer) =>
            {
                DialogResult addPet = MessageBox.Show(
                    owner,
                    "Customer added successfully. Do you want to add a pet for this customer?",
                    "Customer added",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (addPet == DialogResult.Yes)
                {
                    PetFlowGateway.TryOpenPetManagementForOwner(owner, customer);
                }
            };

            PetNavigationHooks.OpenVisitForPet = (owner, petId) =>
            {
                VetVisitsLauncher.TryLaunchVisitsApplication(owner, petId);
            };
        }
    }
}
