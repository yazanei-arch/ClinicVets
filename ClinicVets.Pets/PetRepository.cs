using System.Collections.Generic;
using System.Linq;

namespace ClinicVets.UI
{
    public class PetRepository
    {
        private List<Pet> pets;

        public PetRepository()
        {
            pets = new List<Pet>();
        }

        public string GeneratePetID()
        {
            int nextId = pets.Count + 1;
            return "P" + nextId.ToString("000");
        }

        public void AddPet(Pet pet)
        {
            pets.Add(pet);
        }

        public List<Pet> GetAllPets()
        {
            return pets;
        }

        public List<Pet> SearchPets(string petName, string chipNumber)
        {
            return pets
                .Where(p =>
                    (!string.IsNullOrWhiteSpace(petName) && p.PetName.Contains(petName)) ||
                    (!string.IsNullOrWhiteSpace(chipNumber) && p.ChipNumber.Contains(chipNumber)))
                .ToList();
        }

        public List<Pet> SearchPets(string text)
        {
            return pets
                .Where(p =>
                    p.PetName.Contains(text) ||
                    p.AnimalType.Contains(text) ||
                    p.Owner.Contains(text) ||
                    p.OwnerID.Contains(text) ||
                    p.PetID.Contains(text))
                .ToList();
        }
    }
}