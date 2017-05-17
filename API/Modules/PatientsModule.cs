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
            Get("/{patientId}", parameters => new
            {
                FirstName = "Test",
                LastName = "Patient",
                DateOfBirth = DateTime.Parse("03/27/1965"),
            });
        }
    }
}
