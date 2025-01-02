//using System.Security.Claims;
//using System.Text.Json;
//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Components.Authorization;
//using Blazored.LocalStorage;

//namespace ChatBlazor.Auth
//{
//    public class CustomAuthStateProvider : AuthenticationStateProvider
//    {
//        private readonly ILocalStorageService _localStorage;

//        public CustomAuthStateProvider(ILocalStorageService localStorage)
//        {
//            _localStorage = localStorage;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var token = await _localStorage.GetItemAsync<string>("authToken");

//            if (string.IsNullOrEmpty(token))
//            {
//                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
//            }

//            var claims = ParseClaimsFromJwt(token);
//            var identity = new ClaimsIdentity(claims, "jwt");
//            var user = new ClaimsPrincipal(identity);
//            return new AuthenticationState(user);
//        }

//        public async Task MarkUserAsAuthenticated(string token, string userName)
//        {
//            await _localStorage.SetItemAsync("authToken", token);
//            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "jwt"));
//            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
//            NotifyAuthenticationStateChanged(authState);
//        }

//        public async Task MarkUserAsLoggedOut()
//        {
//            await _localStorage.RemoveItemAsync("authToken");
//            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
//            NotifyAuthenticationStateChanged(authState);
//        }

//        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//        {
//            var claims = new List<Claim>();
//            var payload = jwt.Split('.')[1];
//            var jsonBytes = ParseBase64WithoutPadding(payload);
//            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

//            if (keyValuePairs != null)
//            {
//                keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

//                if (roles != null)
//                {
//                    if (roles.ToString().Trim().StartsWith("["))
//                    {
//                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

//                        if (parsedRoles != null)
//                        {
//                            foreach (var parsedRole in parsedRoles)
//                            {
//                                claims.Add(new Claim(ClaimTypes.Role, parsedRole));
//                            }
//                        }
//                    }
//                    else
//                    {
//                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
//                    }

//                    keyValuePairs.Remove(ClaimTypes.Role);
//                }

//                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
//            }

//            return claims;
//        }

//        private byte[] ParseBase64WithoutPadding(string base64)
//        {
//            switch (base64.Length % 4)
//            {
//                case 2: base64 += "=="; break;
//                case 3: base64 += "="; break;
//            }
//            return Convert.FromBase64String(base64);
//        }
//    }
//}