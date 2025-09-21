using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Dtos;
namespace TiendaProyecto.src.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Email inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("Debe contener al menos una mayúscula.")
                .Matches("[a-z]").WithMessage("Debe contener al menos una minúscula.")
                .Matches("[0-9]").WithMessage("Debe contener al menos un número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Debe contener al menos un carácter especial.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("La confirmación debe coincidir.");
        }
    }
}