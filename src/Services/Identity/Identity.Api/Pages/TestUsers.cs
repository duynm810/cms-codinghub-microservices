// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using IdentityModel;
using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;

namespace Identity.Api;

public static class TestUsers
{
    public static List<TestUser> Users =>
    [
        new TestUser
        {
            SubjectId = Guid.NewGuid().ToString(),
            Username = "duynguyen",
            Password = "admin123",
            Claims =
            {
                new Claim(JwtClaimTypes.Name, "Duy Nguyen"),
                new Claim(JwtClaimTypes.GivenName, "Duy"),
                new Claim(JwtClaimTypes.FamilyName, "Nguyen"),
                new Claim(JwtClaimTypes.Email, "duynguyen8101996@gmail.com"),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.WebSite, "http://codinghub.vn")
            }
        }
    ];
}