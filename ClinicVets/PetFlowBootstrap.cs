using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Registers secretary customer pet prompts for the main application.</summary>
    internal static class PetFlowBootstrap
    {
        internal static void Register()
        {
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
        }
    }
}
