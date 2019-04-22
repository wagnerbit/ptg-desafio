using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services {
    public interface IUserService {
        User Authenticate (string email, string password);
        IEnumerable<User> GetAll ();
        User GetById (int id);
        User Create (User user, string password);
        User Update (User user, string password = null, bool?fromAuth = false);
        void Delete (int id);
        object GenerateToken (User user);
    }

    public class UserService : IUserService {
        IMapper mapper;
        private DataContext _context;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService (DataContext context, IOptions<AppSettings> appSettings) {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public User Authenticate (string email, string password) {

            if (string.IsNullOrEmpty (email) || string.IsNullOrEmpty (password))
                return null;

            var user = _context.Users.SingleOrDefault (x => x.Email == email);

            if (user == null)
                return null;

            if (!VerifyPasswordHash (password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll () {
            var users = _context.Users
                .Include (p => p.Phones)
                .ToArray ();

            return users;
        }

        public User GetById (int id) {
            return _context.Users.Where (u => u.Id == id)
                .Include (p => p.Phones)
                .SingleOrDefault ();
        }

        public User Create (User user, string password) {
            // validation
            if (string.IsNullOrWhiteSpace (password))
                throw new AppException ("Password is required", 400);

            if (_context.Users.Any (x => x.Email.ToLower () == user.Email.ToLower ()))
                throw new AppException ("Email is already taken", 422);

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash (password, out passwordHash, out passwordSalt);

            user.Email = user.Email.ToLower ();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Created_At = DateTime.Now;

            _context.Users.Add (user);
            _context.SaveChanges ();

            return user;
        }

        public User Update (User userParam, string password = null, bool? fromAuth = false) {

            var user = _context.Users.Find (userParam.Id);

            if (user == null)
                throw new AppException ("User not found");

            if (userParam.Email != user.Email) {
                // email has changed so check if the new email is already taken
                if (_context.Users.Any (x => x.Email == userParam.Email))
                    throw new AppException ("E-mail already exists");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Email = userParam.Email.ToLower ();
            user.Phones = userParam.Phones;
            user.Last_Login = fromAuth == true ? DateTime.Now : userParam.Last_Login;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace (password)) {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash (password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update (user);
            _context.SaveChanges ();
            return user;
        }

        public void Delete (int id) {
            var user = _context.Users.Find (id);
            if (user == null) {
                throw new AppException ("Invalid UserId");
            } else {
                _context.Users.Remove (user);
                _context.SaveChanges ();
            }
        }

        // private helper methods
        private static void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
            if (password == null) throw new ArgumentNullException ("password");
            if (string.IsNullOrWhiteSpace (password)) throw new ArgumentException ("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
            }
        }

        private static bool VerifyPasswordHash (string password, byte[] storedHash, byte[] storedSalt) {
            if (password == null) throw new ArgumentNullException ("password");
            if (string.IsNullOrWhiteSpace (password)) throw new ArgumentException ("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException ("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException ("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512 (storedSalt)) {
                var computedHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

        public object GenerateToken (User user) {

            DateTime generatedDate = DateTime.Now;
            DateTime expiresDate = DateTime.UtcNow.AddDays (7);

            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {
                new Claim (ClaimTypes.Name, user.Id.ToString ())
                }),
                NotBefore = generatedDate,
                Expires = expiresDate,
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            var tokenString = tokenHandler.WriteToken (token);

            return new {
                authenticated = true,
                    generatedDate = generatedDate.ToString ("dd/MM/yyyy HH:mm:ss"),
                    expirationDate = expiresDate.ToString ("dd/MM/yyyy HH:mm:ss"),
                    accessToken = tokenHandler.WriteToken (token)
            };

        }

    }
}