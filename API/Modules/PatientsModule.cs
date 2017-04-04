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
            Predicate<Claim> readDemographicsClaim = claim => claim.Type == "allowedresource" && claim.Value == "user/Patient.read";

            this.RequiresClaims(new[] { readDemographicsClaim });
            Get("/{patientId}", parameters => new
            {
                FirstName = "Test",
                LastName = "Patient",
                DateOfBirth = DateTime.Parse("03/27/1965"),
            });
        }
    }
}
