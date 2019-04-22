using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Entities;
using WebApi.Mapping;

namespace WebApi.Middlewares {
    public static class ValidatorConfigurationExtensions {
        public static IServiceCollection AddValidators (this IServiceCollection services) {
            services.AddTransient<IValidator<UserDto>, UserDtoValidator> ();
            services.AddTransient<IValidator<SigninDto>, SigninDtoValidator> ();
            services.AddTransient<IValidator<SignupDto>, SignupDtoValidator> ();
            services.AddTransient<IValidator<PhoneDto>, PhoneDtoValidator> ();
            services.AddTransient<IValidator<User>, UserValidator> ();
            services.AddTransient<IValidator<Phone>, PhoneValidator> ();

            return services;
        }
    }
}