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
    public interface IPhoneService {

        List<object> GetById (int id);

    }

    public class PhoneService : IPhoneService {
        IMapper mapper;
        private DataContext _context;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public PhoneService (DataContext context, IOptions<AppSettings> appSettings) {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public List<object> GetById (int id) {
            var phones = _context.Phones
                .Where (p => p.UsersFK == id);

            List<object> phoneUpdated = new List<object> ();
            foreach (var phone in phones) {

                var phoneObj = new {
                    number = phone.Number,
                    area_code = phone.Area_code,
                    country_code = phone.Country_code
                };

                phoneUpdated.Add (phoneObj);
            }

            return phoneUpdated;
        }

    }
}