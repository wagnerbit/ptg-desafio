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
using Newtonsoft.Json;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Middlewares;
using WebApi.Services;

namespace WebApi.Controllers {
    [Authorize]
    [ApiController]
    [Route ("[controller]")]
    public class UsersController : ControllerBase {
        private IUserService _userService;
        private IPhoneService _phoneService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController (
            IUserService userService,
            IPhoneService phoneService,
            IMapper mapper,
            IOptions<AppSettings> appSettings) {
            _userService = userService;
            _phoneService = phoneService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] UserDto userDto) {
            var user = _userService.Authenticate (userDto.Email, userDto.Password);

            if (user == null) {
                return BadRequest (new { message = "Invalid e-mail or password", statusCode = 401 });
            }

            var objToken = _userService.GenerateToken (user);
            var lastLogin = user.Last_Login == default (DateTime) ? DateTime.Now : user.Last_Login;
            user.Last_Login = DateTime.Now;
            _userService.Update (user, null, true);

            var phones = _phoneService.GetById (user.Id);

            //Return user info (without password) plus token to store in the client side
            return Ok (new {
                message = "User logged sucessfully!",
                    Id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    phones = phones,
                    created_at = user.Created_At.ToString ("dd/MM/yyyy HH:mm:ss"),
                    last_login = lastLogin.ToString ("dd/MM/yyyy HH:mm:ss"),
                    Token = objToken
            });
        }

        [AllowAnonymous]
        [HttpPost ("register")]
        public IActionResult Register ([FromBody] UserDto userDto) {
            // Map dto to entity
            var user = _mapper.Map<User> (userDto);

            try {

                // Create User 
                _userService.Create (user, userDto.Password);
                return Ok (new { message = $"User {user.FirstName} Created!", statusCode = 201 });

            } catch (AppException ex) {

                return BadRequest (new { message = ex.Message });

            }
        }

        [UserIdentityValidatorsMiddleware]
        [HttpGet]
        public IActionResult GetAll () {
            var users = _userService.GetAll ();
            var userDtos = _mapper.Map<IList<UserDto>> (users);
            return Ok (userDtos);
        }

        [UserIdentityValidatorsMiddleware]
        [HttpGet ("{id}")]
        public IActionResult GetById (int id) {
            var user = _userService.GetById (id);
            var phones = _phoneService.GetById (id);

            //Return user info (without password) plus token to store in the client side
            return Ok (new {
                    Id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    phones = phones                    
            });
        }

        [UserIdentityValidatorsMiddleware]
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
                    message = "User Updated!",
                        statusCode = 201,
                        userDtos.FirstName,
                        userDtos.LastName,
                        userDtos.Email
                });
            } catch (AppException ex) {
                // It will return an error message if there are an exception
                return BadRequest (new { message = ex.Message });
            }
        }

        [UserIdentityValidatorsMiddleware]
        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            _userService.Delete (id);
            return Ok ();
        }
    }
}