using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Flashcards.IdentityProvider.App
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                var profileIdentityResources = new IdentityResources.Profile();
                profileIdentityResources.UserClaims.Add("username");

                return new IdentityResource[]
                {
                    new IdentityResources.OpenId(),
                    profileIdentityResources,
                    new IdentityResource("roles", new[] { JwtClaimTypes.Role })
                };
            }
        }

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("flashcardsclientaudience") { UserClaims = { JwtClaimTypes.Role } }
        };

        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
            new ApiScope("flashcardsapi", "Flashcards API", new[] { JwtClaimTypes.Role })
        };

        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client
            {
                ClientName = "Flashcards Client",
                ClientId = "flashcardsclient",
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    "https://oauth.pstmn.io/v1/callback",
                    "https://localhost:44355/authentication/login-callback",
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost:44355"
                },
                AllowedGrantTypes = [
                    GrantType.ResourceOwnerPassword,
                    GrantType.AuthorizationCode
                ],
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:44355"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "roles",
                    "flashcardsapi"
                },
                AlwaysIncludeUserClaimsInIdToken = true,
            },
            new Client
            {
                ClientId = "flashcardsbackend",
                
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = { new Secret("FlashcardsApiSecret".Sha256()) },

                AllowedScopes = { "user_management" }
            }
        };
    }
}
