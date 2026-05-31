using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace ClinicVets.UI
{
    public class PetRepository
    {
        private List<Pet> pets;

        public PetRepository()
        {
            pets = new List<Pet>();
        }

        public List<Pet> LoadPetsFromExcel()
        {
            List<Pet> loadedPets = new List<Pet>();
            string filePath = ExcelFileManager.FilePath; // Ensure this matches your file path logic

            if (!File.Exists(filePath))
            {
                return loadedPets; // Return empty list if file doesn't exist yet
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    if (!workbook.Worksheets.Contains("Pets"))
                    {
                        return loadedPets;
                    }

                    var sheet = workbook.Worksheet("Pets");
                    var range = sheet.RangeUsed();

                    if (range == null) return loadedPets;

                    // Loop through all rows, skipping the first row (headers)
                    foreach (var row in range.RowsUsed().Skip(1))
                    {
                        Pet p = new Pet
                        {
                            PetID = row.Cell(1).GetValue<string>(),
                            PetName = row.Cell(2).GetValue<string>(),
                            AnimalType = row.Cell(3).GetValue<string>(),
                            Weight = row.Cell(4).GetValue<double>(),
                            BirthDate = row.Cell(5).GetValue<DateTime>(),
                            Owner = row.Cell(6).GetValue<string>(),
                            ChipNumber = row.Cell(7).GetValue<string>(),
                            LastVaccineDate = row.Cell(8).GetValue<DateTime>()
                        };

                        loadedPets.Add(p);
                    }
                }
            }
            catch (Exception)
            {
                // If Excel is locked or corrupt, it just returns what it has so far
            }

            return loadedPets;
        }
        public string GeneratePetID()
        {
            this.pets = LoadPetsFromExcel();
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