using System.Security.Authentication;
using JWT.Algorithms;
using JWT.Builder;
using LanguageExt.Common;
using LiveOrderService.Application.DTOs;
using LiveOrderService.Application.Repositories;
using LiveOrderService.Domain.Tokens;
using LiveOrderService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LiveOrderService.Infrastructure.Repositories
{
    public class AuthRepository(PostgresDbContext _context, ILogger<AuthRepository> _logger) : IAuthRepository
    {

        public async Task<Result<AuthResponse>> AuthenticateUserAsync(string username, string password, string personalKey)
        {
            var fetchedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username
                .Equals(username.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if(fetchedUser == null)
            {
                _logger.LogError("User not found");
                return new Result<AuthResponse>(new EntryPointNotFoundException("User not found"));
            }

            if(!fetchedUser.VerifyPassword(password))
            {
                _logger.LogError("Invalid password");
                return new Result<AuthResponse>(new AuthenticationException("Invalid password"));
            }

            if(string.Equals(personalKey, "last 3 personal keys"))
            {
                _logger.LogError("Please use a new different personal key");
                return new Result<AuthResponse>(new AuthenticationException("Please use a new different personal key"));
            }

            var token = GenerateToken(personalKey, "comsumer_token", 30);
            var refreshToken = GenerateToken(personalKey, "refresh_token", 60*4);

            if(token is null || refreshToken is null)
            {
                _logger.LogError("Error generating token");
                return new Result<AuthResponse>(new Exception("error generating token"));
            }

            return new AuthResponse(token, refreshToken);
        }

        private string GenerateToken(string personalKey, string tokenType, int minutes)
        {
            var token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(personalKey)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(minutes).ToUnixTimeSeconds())
                .AddClaim("type", tokenType)
                .Encode();

            if(token is null)
            {
                _logger.LogError("Error authenticating user");
                return null!;
            }
            
            return token;
        }

        private IDictionary<string, string> DecodeToken(string token, string personalKey)
        {
            var decodedToken = new JwtBuilder()
                .WithSecret(personalKey)
                .MustVerifySignature()
                .Decode<IDictionary<string, string>>(token);

            if(decodedToken is null)
            {
                _logger.LogError("Invalid token");
                return null!;
            }

            return decodedToken;
        }

        private string ValidateTokenData(IDictionary<string, string> decodedToken)
        {
            if(decodedToken is null)
            {
                _logger.LogError("Invalid token");
                return "Invalid token";
            }

            if(!decodedToken.TryGetValue("exp", out var exp) || !decodedToken.TryGetValue("username", out _))
            {
                _logger.LogError("Invalid token");
                return "Invalid token";
            }

            if(DateTimeOffset.TryParse(exp, out var expiry) && DateTimeOffset.UtcNow > expiry)
            {
                _logger.LogError("Token expired");
                return "Token expired";
            }

            return "";
        }

        public async Task<Result<string>> ValidateTokenAsync(string token, string personalKey)
        {
            var decodedToken = DecodeToken(token, personalKey);

            if(ValidateTokenData(decodedToken) is string error && !string.IsNullOrEmpty(error))
            {
                return new Result<string>(new AuthenticationException(error));
            }

            if(decodedToken.TryGetValue("type", out var tokenType) && !tokenType.Equals("consumer_token"))
            {
                _logger.LogError("Invalid token");
                return new Result<string>(new AuthenticationException("Invalid token"));
            }

            decodedToken.TryGetValue("exp", out var exp);
            DateTimeOffset.TryParse(exp, out var expiry);

            _context.Tokens.Add(new Token(token, expiry));
            await _context.SaveChangesAsync();

            return new Result<string>(GenerateToken(personalKey, "consumer_token", 30));
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string personalKey)
        {
            var decodedToken = DecodeToken(refreshToken, personalKey);

            if(ValidateTokenData(decodedToken) is string error && !string.IsNullOrEmpty(error))
            {
                return new Result<AuthResponse>(new AuthenticationException(error));
            }

            if(_context.Tokens.Exists(x => x.TokenKey.Equals(refreshToken)))
            {
                _logger.LogError("Token already used");
                return new Result<AuthResponse>(new AuthenticationException("Token already used"));
            }
            
            decodedToken.TryGetValue("exp", out var exp);
            DateTimeOffset.TryParse(exp, out var expiry);

            _context.Tokens.Add(new Token(refreshToken, expiry));
            await _context.SaveChangesAsync();

            return new AuthResponse(GenerateToken(personalKey, "consumer_token", 30), 
                                    GenerateToken(personalKey, "refresh_token", 60*4));
        }
    }
}