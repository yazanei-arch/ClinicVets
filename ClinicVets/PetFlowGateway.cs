using System;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Pet navigation hooks used by secretary customer screens.</summary>
    public static class PetFlowGateway
    {
        public static Action<IWin32Window, string> OpenViewPets { get; set; }
        public static Action<IWin32Window, string> OpenPetManagementForOwner { get; set; }
        public static Action<IWin32Window> OpenVetPetManagement { get; set; }
        public static Action<IWin32Window, Customer> PromptAddPetAfterCustomer { get; set; }

        public static string GetOwnerKey(Customer customer)
        {
            if (customer == null)
            {
                return string.Empty;
            }

            string customerId = (customer.CustomerID ?? string.Empty).Trim();
            if (customerId.Length > 0)
            {
                return customerId;
            }

            return (customer.IDNumber ?? string.Empty).Trim();
        }

        public static void TryOpenViewPets(IWin32Window owner, Customer customer)
        {
            string ownerKey = GetOwnerKey(customer);
            if (ownerKey.Length == 0)
            {
                MessageBox.Show(owner, "Customer ID is missing.", "Pets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenViewPets?.Invoke(owner, ownerKey);
        }

        public static void TryOpenPetManagementForOwner(IWin32Window owner, Customer customer)
        {
            string ownerKey = GetOwnerKey(customer);
            if (ownerKey.Length == 0)
            {
                MessageBox.Show(owner, "Customer ID is missing.", "Pets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (OpenPetManagementForOwner != null)
            {
                OpenPetManagementForOwner(owner, ownerKey);
                return;
            }

            NavigationHelper.OpenSecretaryPetManagement(owner, ownerKey);
        }

        public static void TryOpenVetPetManagement(IWin32Window owner)
        {
            if (OpenVetPetManagement != null)
            {
                OpenVetPetManagement(owner);
                return;
            }

            NavigationHelper.OpenVetPetManagement(owner);
        }

        public static void TryPromptAddPetAfterCustomer(IWin32Window owner, Customer customer)
        {
            if (PromptAddPetAfterCustomer != null)
            {
                PromptAddPetAfterCustomer(owner, customer);
                return;
            }

            DialogResult addPet = MessageBox.Show(
                owner,
                "Customer added successfully. Do you want to add a pet for this customer?",
                "Customer added",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (addPet == DialogResult.Yes)
            {
                TryOpenPetManagementForOwner(owner, customer);
            }
        }
    }
}
