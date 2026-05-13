using OfficeOpenXml;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ClinicVets.UI
{
    public class PetRepository
    {
        private readonly string filePath = @"C:\Users\royal\source\repos\ClinicVets-main\ClinicVetsData.xlsx";

        public void AddPet(Pet pet)
        {
            ExcelPackage.License.SetNonCommercialOrganization("ClinicVets Student Project");

            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets["Pets"];

                if (worksheet == null)
                {
                    worksheet = package.Workbook.Worksheets.Add("Pets");

                    worksheet.Cells[1, 1].Value = "PetID";
                    worksheet.Cells[1, 2].Value = "PetName";
                    worksheet.Cells[1, 3].Value = "AnimalType";
                    worksheet.Cells[1, 4].Value = "Weight";
                    worksheet.Cells[1, 5].Value = "BirthDate";
                    worksheet.Cells[1, 6].Value = "Owner";
                    worksheet.Cells[1, 7].Value = "ChipNumber";
                    worksheet.Cells[1, 8].Value = "LastVaccineDate";
                }

                int lastRow = worksheet.Dimension?.Rows ?? 1;
                int newRow = lastRow + 1;

                worksheet.Cells[newRow, 1].Value = pet.PetID;
                worksheet.Cells[newRow, 2].Value = pet.PetName;
                worksheet.Cells[newRow, 3].Value = pet.AnimalType;
                worksheet.Cells[newRow, 4].Value = pet.Weight;
                worksheet.Cells[newRow, 5].Value = pet.BirthDate.ToString("yyyy-MM-dd");
                worksheet.Cells[newRow, 6].Value = pet.Owner;
                worksheet.Cells[newRow, 7].Value = pet.ChipNumber;
                worksheet.Cells[newRow, 8].Value = pet.LastVaccineDate.ToString("yyyy-MM-dd");

                package.Save();
            }
        }

        public string GeneratePetID()
        {
            ExcelPackage.License.SetNonCommercialOrganization("ClinicVets Student Project");

            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets["Pets"];

                if (worksheet == null || worksheet.Dimension == null)
                    return "P001";

                int rowCount = worksheet.Dimension.Rows;
                int nextNumber = rowCount;
                return "P" + nextNumber.ToString("D3");
            }
        }

        public List<Pet> SearchPets(string petName, string chipNumber)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Yazan Eissa");

            List<Pet> pets = new List<Pet>();

            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                var worksheet = package.Workbook.Worksheets["Pets"];

                if (worksheet == null || worksheet.Dimension == null)
                    return pets;

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string currentPetID = worksheet.Cells[row, 1].Text.Trim();          // A
                    string currentPetName = worksheet.Cells[row, 2].Text.Trim();        // B
                    string currentAnimalType = worksheet.Cells[row, 3].Text.Trim();     // C
                    string currentWeight = worksheet.Cells[row, 4].Text.Trim();         // D
                    string currentBirthDate = worksheet.Cells[row, 5].Text.Trim();      // E
                    string currentOwner = worksheet.Cells[row, 6].Text.Trim();          // F
                    string currentChipNumber = worksheet.Cells[row, 7].Text.Trim();     // G
                    string currentLastVaccineDate = worksheet.Cells[row, 8].Text.Trim(); // H

                    bool matchesPetName = string.IsNullOrWhiteSpace(petName) ||
                                          currentPetName.ToLower().Contains(petName.ToLower());

                    bool matchesChipNumber = string.IsNullOrWhiteSpace(chipNumber) ||
                                             currentChipNumber.ToLower().Contains(chipNumber.ToLower());

                    if (matchesPetName && matchesChipNumber)
                    {
                        pets.Add(new Pet
                        {
                            PetID = currentPetID,
                            PetName = currentPetName,
                            AnimalType = currentAnimalType,
                            Weight = double.TryParse(currentWeight, out double w) ? w : 0,
                            BirthDate = DateTime.TryParse(currentBirthDate, out DateTime bd) ? bd : DateTime.MinValue,
                            Owner = currentOwner,
                            ChipNumber = currentChipNumber,
                            LastVaccineDate = DateTime.TryParse(currentLastVaccineDate, out DateTime vd) ? vd : DateTime.MinValue
                        });
                    }
                }
            }

            return pets;
        }
    }
}