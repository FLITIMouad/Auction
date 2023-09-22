﻿using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp","Auction app full access"),
           // new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
       /*      new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "auctionApp" }
            }, */

            // interactive client using code flow + pkce
         /*    new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" }
            }, */

            new Client{
                ClientId="postman",
                ClientName="Postman",
                AllowedScopes={"openid", "profile", "auctionApp"},
                RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                ClientSecrets= new[] { new Secret("NotASecret".Sha256())},
                AllowedGrantTypes={GrantType.ResourceOwnerPassword}
            },

               new Client
               {
                ClientId="nextApp",
                ClientName="nextApp",
                AllowedScopes={"openid", "profile", "auctionApp"},
                RedirectUris = { "http://localhost:3000/api/auth/callback/id-server" },
                ClientSecrets= new[] { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                AllowedGrantTypes=GrantTypes.CodeAndClientCredentials,
                RequirePkce=false,
                AllowOfflineAccess=false,
                AccessTokenLifetime=3600*24*30
              }
        };
}
