using System;

namespace ClinicVets.UI
{
    public class Pet
    {
        public string PetID { get; set; }
        public string PetName { get; set; }
        public string AnimalType { get; set; }
        public double Weight { get; set; }
        public DateTime BirthDate { get; set; }
        public string Owner { get; set; }
        public string ChipNumber { get; set; }
        public DateTime LastVaccineDate { get; set; }
    }
}