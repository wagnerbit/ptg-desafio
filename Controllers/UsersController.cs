using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers {
    [Authorize]
    [ApiController]
    [Route ("[controller]")]
    public class UsersController : ControllerBase {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController (
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] UserDto userDto) {
            var user = _userService.Authenticate (userDto.Email, userDto.Password);

            if (user == null)
                return BadRequest (new { message = "Email or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {
                new Claim (ClaimTypes.Name, user.Id.ToString ())
                }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            var tokenString = tokenHandler.WriteToken (token);

            // return basic user info (without password) and token to store client side
            return Ok (new {
                Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost ("register")]
        public IActionResult Register ([FromBody] UserDto userDto) {
            // map dto to entity
            var user = _mapper.Map<User> (userDto);

            try {
                // save 
                _userService.Create (user, userDto.Password);
                return Ok (new { message = "User created sucessfully!" });
                //return Ok (user);
            } catch (AppException ex) {
                // return error message if there was an exception
                return BadRequest (new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll () {
            var users = _userService.GetAll ();
            var userDtos = _mapper.Map<IList<UserDto>> (users);
            return Ok (userDtos);
        }

        [HttpGet ("{id}")]
        public IActionResult GetById (int id) {
            var user = _userService.GetById (id);
            var userDto = _mapper.Map<UserDto> (user);
            return Ok (userDto);
        }

        [HttpPut ("{id}")]
        public IActionResult Update (int id, [FromBody] UserDto userDto) {
            // Map DTO to Entity and Set ID
            var user = _mapper.Map<User> (userDto);
            user.Id = id;

            try {
                // save 
                var userResponse = _userService.Update (user, userDto.Password);
                var userDtos = _mapper.Map<UserDto> (userResponse);

                return Ok (new {
                    statusCode = StatusCode (Microsoft.AspNetCore.Http.StatusCodes.Status200OK),
                        userDtos.FirstName,
                        userDtos.LastName,
                        userDtos.Email
                });
            } catch (AppException ex) {
                // It will return an error message if there are an exception
                return BadRequest (new { message = ex.Message });
            }
        }

        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            _userService.Delete (id);
            return Ok ();
        }
    }
}