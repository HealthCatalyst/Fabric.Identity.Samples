using System;
using System.Security.Claims;
using Nancy;
using Nancy.Security;

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
                    FirstName = "Matt",
                    LastName = "Murdock",
                    DateOfBirth = DateTime.Parse("03/27/1965")
                },
                new
                {
                    FirstName = "Jessica",
                    LastName = "Jones",
                    DateOfBirth = DateTime.Parse("06/12/1972")
                },
                new
                {
                    FirstName = "Luke",
                    LastName = "Cage",
                    DateOfBirth = DateTime.Parse("05/29/1980")
                }
            };
        }
    }
}
