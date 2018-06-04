using System;
using Nancy;

namespace Fabric.Identity.Samples.API.Modules
{
    public class PatientsModule : NancyModule
    {
        public PatientsModule() : base("/patients")
        {
            Get("/{patientId}", parameters => GetPatients());
        }

        private dynamic GetPatients()
        {
            return new object[]
            {
                new
                {
                    Id = 1,
                    FirstName = "Matt",
                    LastName = "Murdock",
                    DateOfBirth = DateTime.Parse("03/27/1965")
                },
                new
                {
                    Id = 2,
                    FirstName = "Jessica",
                    LastName = "Jones",
                    DateOfBirth = DateTime.Parse("06/12/1972")
                },
                new
                {
                    Id = 3,
                    FirstName = "Luke",
                    LastName = "Cage",
                    DateOfBirth = DateTime.Parse("05/29/1980")
                }
            };
        }
    }
}