using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicVets
{
    /// <summary>Matches pets to customers using CustomerID / IDNumber and OwnerID / Owner / CustomerID columns on Pets.</summary>
    internal static class CustomerPetMatcher
    {
        internal static IReadOnlyList<Pet> GetPetsForCustomer(IEnumerable<Pet> allPets, Customer customer)
        {
            if (customer == null || allPets == null)
            {
                return Array.Empty<Pet>();
            }

            var keys = GetCustomerMatchKeys(customer);
            if (keys.Count == 0)
            {
                return Array.Empty<Pet>();
            }

            return allPets
                .Where(p => PetMatchesAnyKey(p, keys))
                .ToList();
        }

        private static HashSet<string> GetCustomerMatchKeys(Customer customer)
        {
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddKey(keys, customer.CustomerID);
            AddKey(keys, customer.IDNumber);
            return keys;
        }

        private static bool PetMatchesAnyKey(Pet pet, HashSet<string> keys)
        {
            if (pet == null)
            {
                return false;
            }

            return KeyMatches(keys, pet.OwnerID);
        }

        private static void AddKey(HashSet<string> keys, string value)
        {
            string trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Length > 0)
            {
                keys.Add(trimmed);
            }
        }

        private static bool KeyMatches(HashSet<string> keys, string ownerValue)
        {
            string trimmed = (ownerValue ?? string.Empty).Trim();
            return trimmed.Length > 0 && keys.Contains(trimmed);
        }
    }
}
